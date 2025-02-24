from asana.api import attachments_api
from fastapi import FastAPI, Request, Response
from fastapi.responses import JSONResponse
from AmazonDeviceFarm.main import main
import uvicorn
import asana
from asana.rest import ApiException
import os
import json
import time
import requests




app = FastAPI()

asana_configuration = asana.Configuration()
asana_configuration.access_token = "1/1"
api_client = asana.ApiClient(asana_configuration)

tasks_api = asana.TasksApi(api_client)
stories_api = asana.StoriesApi(api_client)


CONFIG_FILE_PATH = "AmazonDeviceFarm/configuration.json"
PROJECT_NAME_FIELD = ""
PROJECT_BUNDLE_FIELD = ""
AUTOMATION_STATUS_FIELD = ""


def update_config(project_bundle_value: str, project_name_value: str):
    try:
        with open(CONFIG_FILE_PATH, "r", encoding="utf-8") as f:
            config_data = json.load(f)
    except (FileNotFoundError, json.JSONDecodeError):
        config_data = {}

    config_data["project_bundle"] = project_bundle_value
    config_data["project_name"] = project_name_value

    with open(CONFIG_FILE_PATH, "w", encoding="utf-8") as f:
        json.dump(config_data, f, ensure_ascii=False, indent=2)

def download_file(url: str, local_filename: str) -> None:
    try:
        r = requests.get(url, stream=True)
        r.raise_for_status()
        with open(local_filename, "wb") as f:
            for chunk in r.iter_content(chunk_size=8192):
                f.write(chunk)
    except Exception as e:
        print(f"Ошибка при скачивании файла {url}: {e}")


@app.post("/asana-webhook-endpoint")
async def asana_webhook(request: Request):
    if "X-Hook-Secret" in request.headers:
        secret = request.headers["X-Hook-Secret"]
        return Response(content="", status_code=200, headers={"X-Hook-Secret": secret})

    body = await request.json()
    events = body.get("events", [])

    for event in events:
        change_data = event.get("change")
        if not change_data:
            continue

        if change_data.get("field") == "custom_fields":
            resource = event["resource"]
            if isinstance(resource, dict):
                task_id = resource.get("gid")
            else:
                task_id = resource

            task = tasks_api.get_task(
                task_id,
                {
                    "opt_expand": "this"
                }
            )
            print(task)

            custom_fields = task.get("custom_fields", [])
            print("DEBUG: custom_fields =", custom_fields)

            project_bundle_value = None
            project_name_value = None
            automation_status_value = None

            for cf in custom_fields:
                if cf.get("gid") == PROJECT_BUNDLE_FIELD:
                    project_bundle_value = cf.get("text_value")
                elif cf.get("gid") == PROJECT_NAME_FIELD:
                    project_name_value = cf.get("text_value")
                elif cf.get("gid") == AUTOMATION_STATUS_FIELD:
                    #automation_status_value = cf.get("display_value")
                    automation_status_value = cf.get("enum_value", {}).get("name")
                    print("поместил статус в велью")

            if automation_status_value == "Ready for launch":
                print("Обнаружил изменение поля статуса")

                response = api_client.call_api(
                    f'/tasks/{task_id}/attachments',
                    'GET',
                    {},
                    {'opt_fields': 'name,download_url'},
                    {},
                    {},
                    {},
                    response_type='list'
                )

                attachments = response[0]
                print("Attachments:", attachments)
                found_apk = False

                for attach in attachments:
                    attach_name = attach.get("name", "")
                    download_url = attach.get("download_url", "")
                    if attach_name.lower().endswith(".apk") and download_url:
                        found_apk = True
                        print(f"Найден .apk файл: {attach_name}. Скачиваем...")
                        download_file(download_url, attach_name)

                if not found_apk:
                    print("В задаче нет .apk-файлов. Пропускаем запуск тестов.")
                    continue

                update_config(project_bundle_value, project_name_value)

                time.sleep(3)

                try:
                    lines = main() #возвращает массив строк репорта теста
                    comment_text = "\n".join(lines)
                    stories_api.create_story_for_task(task_id, {"text": comment_text})
                except Exception as e:
                    print("Ошибка при выполнении скрипта:", e)

    return JSONResponse(content={}, status_code=200)

if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=8000)

