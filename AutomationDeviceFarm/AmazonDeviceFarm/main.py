import boto3
import json
import requests
import zipfile
import shutil
import time
import uuid
import os
from requests.adapters import HTTPAdapter
import ssl
from datetime import datetime

configuration_file = os.path.join(os.getcwd(), 'configuration.json')

with open(configuration_file, 'r') as file:
    configuration = json.load(file)

NAME_PREFIX: str = configuration.get('project_name')
PROJECT_BUNDLE_ID = configuration.get('project_bundle')

APPIUM_CONFIG_PATH = "./appium_test_package"  # путь к конфигу аппиума на сервере

temp_data = {"bundle_id": PROJECT_BUNDLE_ID}

with open("./AmazonDeviceFarm/appium_test_package/tests/config.json", 'w') as file:
    json.dump(temp_data, file)



# Конфигурация
TEST_RESULT_REPORT_PATH: str = "D:/JavaScript/Python/temp_extracted/Host_Machine_Files/$DEVICEFARM_LOG_DIR/test_result.log"
PROJECT_ARN: str = "arn:aws"
DEVICE_POOL_ARN: str = "arn:aws:devicefarm:us-west-2"
APK_FILE_PATH: str = "D:/ChromeDownloads/DemoTestApk.apk"
TEST_APPIUM_PACKAGE_PATH: str = "./appium_tests.zip"
REGION_NAME: str = "us-west-2"
RESULT_DIRECTORY: str = ""


client = boto3.client('devicefarm', region_name=REGION_NAME)


class SSLAdapter(HTTPAdapter):
    def __init__(self, ssl_protocol=ssl.PROTOCOL_TLSv1_2, *args, **kwargs):
        self.ssl_protocol = ssl_protocol
        super().__init__(*args, **kwargs)

    def init_poolmanager(self, *args, **kwargs):
        kwargs['ssl_version'] = self.ssl_protocol
        return super().init_poolmanager(*args, **kwargs)


def generate_unique_name(prefix):
    return f"{prefix}-{datetime.now().strftime('%Y-%m-%d')}-{uuid.uuid4().hex[:8]}"


def upload_test_spec(file_path, project_arn):
    unique_name = generate_unique_name("TestSpec")
    return upload_file(file_path, project_arn, "APPIUM_PYTHON_TEST_SPEC", unique_name)


def upload_file(file_path, project_arn, file_type, unique_name):
    with open(file_path, 'rb') as file:
        file_bytes = file.read()

    create_upload_response = client.create_upload(
        projectArn=project_arn,
        name=f"{unique_name}_{os.path.basename(file_path)}",
        type=file_type,
        contentType='application/octet-stream'
    )

    upload = create_upload_response['upload']
    upload_arn = upload['arn']
    upload_url = upload['url']

    session = requests.Session()
    session.mount('https://', SSLAdapter())

    response = session.put(upload_url, data=file_bytes, headers={'Content-Type': 'application/octet-stream'})

    if response.status_code != 200:
        raise Exception(f"Ошибка загрузки {file_path}: {response.status_code} {response.reason}\n{response.text}")
    else:
        print(f"Сборка из {file_path} успешно загружена с ARN: {upload_arn}")

    while True:
        upload_status = client.get_upload(arn=upload_arn)
        status = upload_status['upload']['status']
        if status == 'SUCCEEDED':
            print(f"Сборка успешно загружена")
            break
        elif status == 'FAILED':
            message = upload_status['upload'].get('message', 'Неизвестная ошибка')
            raise Exception(f"Ошибка загрузки сборки: {message}")
        else:
            print("Ожидание загрузки сборки...")
            time.sleep(5)
    return upload_arn


def upload_test_package(file_path, project_arn):
    unique_name = generate_unique_name("TestBundle")
    return upload_file(file_path, project_arn, "APPIUM_PYTHON_TEST_PACKAGE", unique_name)


def schedule_run(project_arn, app_arn, device_pool_arn, test_arn, test_spec_arn, unique_name):
    response = client.schedule_run(
        projectArn=project_arn,
        appArn=app_arn,
        devicePoolArn=device_pool_arn,
        name=unique_name,
        test={
            'type': 'APPIUM_PYTHON',
            'testPackageArn': test_arn,
            'testSpecArn': test_spec_arn
        }
    )
    run = response['run']
    run_arn = run['arn']
    print(f"Запуск {unique_name} теста. ARN запуска: {run_arn}")
    return run_arn


def wait_for_run_completion(run_arn):
    print("Ожидание завершения теста...")
    while True:
        response = client.get_run(arn=run_arn)
        status = response['run']['status']
        result = response['run']['result']
        if status in ['COMPLETED', 'ERRORED']:
            print(f"Тест завершен со статусом: {status}, результат: {result}")
            break
        time.sleep(10)


def get_last_run_arn(project_arn) -> str:
    response = client.list_runs(arn=project_arn)
    runs = response['runs']
    if not runs:
        raise Exception("Нет запущенных тестов")
    last_run = runs[0]
    print(f"Последний запущенный тест: {last_run['name']}, ARN: {last_run['arn']}")
    return last_run['arn']


def search_pattern_in_file(file_path, pattern) -> bool:
    with open(file_path, 'r') as file:
        search_sdk = pattern.lower()
        for line_number, line in enumerate(file, start=1):
            if search_sdk in line.strip().lower():
                return True
        return False


def zip_folder(folder_path, output_zip) -> str:
    with zipfile.ZipFile(output_zip, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(folder_path):
            for file in files:
                file_path = os.path.join(root, file)
                zipf.write(file_path, os.path.relpath(file_path, folder_path))
    return output_zip


# Указываю индекс для нужного артефакта, имя артефакта - название файла, которое будет сохранено
def download_artifacts(run_arn, artifact_index, artifact_name, file_format) -> None:
    response = client.list_jobs(arn=run_arn)
    jobs = response.get('jobs', [])
    job_arn = jobs[0].get('arn')

    artifacts_response = client.list_artifacts(
        type="FILE",
        arn=job_arn
    )

    artifacts = artifacts_response.get('artifacts', [])
    logcat_artifact_url = artifacts[artifact_index].get('url')

    response = requests.get(logcat_artifact_url)
    if file_format == "TXT":
        with open(artifact_name, "w", encoding="utf-8") as file:
            file.write(response.text)
    elif file_format == "ZIP":
        with open(artifact_name, "wb") as file:
            file.write(response.content)


def add_sdk_log_info(archive_path, folder_path) -> list[str]:
    temp_dir = "temp_extracted"
    os.makedirs(temp_dir, exist_ok=True)

    with zipfile.ZipFile(archive_path, 'r') as ref:
        ref.extractall(temp_dir)

    new_file_path = os.path.join(temp_dir, folder_path)
    with open(new_file_path, 'a', encoding="utf-8") as file:
        if search_pattern_in_file("logs.txt", "FirebaseApp initialization successful"):
            file.write("Firebase инициализирован\n")
        else:
            file.write("Firebase НЕ инициализирован\n")
        if search_pattern_in_file("logs.txt", "Adjust is initialized"):
            file.write("Adjust инициализирован\n")
        else:
            file.write("Adjust НЕ инициализирован\n")
        if search_pattern_in_file("logs.txt", "AppMetrica: Activate AppMetrica with APIKey"):
            file.write("AppMetrica инициализирована\n")
        else:
            file.write("AppMetrica НЕ инициализирована\n")
        if search_pattern_in_file("logs.txt", "Amplitude"):
            file.write("Запрещенное сдк есть в билде\n")
        else:
            file.write("В билде нет запрещенных сдк\n")


    # возврат массива строк с готовым репортом
    report_file = TEST_RESULT_REPORT_PATH
    with open (report_file, 'r', encoding='utf-8') as file:
        report = file.readlines()
        report = [line.strip() for line in report]

    return report


def main():
    unique_name = generate_unique_name(NAME_PREFIX)
    print(f"Тест: {unique_name}")

    try:
        app_arn = upload_file(
            file_path=APK_FILE_PATH,
            project_arn=PROJECT_ARN,
            file_type='ANDROID_APP',
            unique_name=unique_name
        )
        print(f"Сборка загружена с AppArn: {app_arn}")

        package_path = zip_folder("./appium_test_package", "./appium_tests.zip")

        test_arn = upload_test_package(
            file_path=package_path,
            project_arn=PROJECT_ARN
        )
        print(f"Тестовый пакет Appium загружен с TestArn: {test_arn}")

        test_spec_arn = upload_test_spec(
            file_path="D:/JavaScript/Python/AppiumScriptDemo/test_spec.yml",
            project_arn=PROJECT_ARN
        )
        print(f"Test Spec ARN: {test_spec_arn}")

        run_arn = schedule_run(
            project_arn=PROJECT_ARN,
            app_arn=app_arn,
            device_pool_arn=DEVICE_POOL_ARN,
            test_arn=test_arn,
            test_spec_arn=test_spec_arn,
            unique_name=unique_name
        )

        wait_for_run_completion(run_arn)

        # Скачивание артефактов
        download_artifacts(run_arn, 8, "logs.txt", "TXT")
        download_artifacts(run_arn, 5, "result.zip", "ZIP")

        # Запись в кастомный артефакт скрипта аппиум
        if os.path.exists("logs.txt"):
            report_lines = add_sdk_log_info("result.zip", "Host_Machine_Files/$DEVICEFARM_LOG_DIR/test_result.log")
            return report_lines

    except Exception as e:
        print(f"Ошибка: {e}")


if __name__ == "__main__":
    main()
