import cv2
import os
import json
import numpy as np
import unittest
import time
from appium import webdriver
from appium.webdriver.common.appiumby import AppiumBy
from appium.options.android import UiAutomator2Options

config_file = "tests/config.json"

with open(config_file, 'r') as file:
    config = json.load(file)

PROJECT_BUNDLE_ID = config.get('bundle_id')

options = UiAutomator2Options()
options.platform_name = "Android"
options.automation_name = "uiautomator2"
options.app_package = PROJECT_BUNDLE_ID
options.app_activity = "com.unity3d.player.UnityPlayerActivity"


class AppiumTest(unittest.TestCase):
    def setUp(self) -> None:
        time.sleep(5)
        self.driver = webdriver.Remote("http://localhost:4723/wd/hub", options=options)

    def tearDown(self) -> None:
        if self.driver:
            self.driver.quit()

    @classmethod
    def setUpClass(cls):
        log_dir = os.getenv("DEVICEFARM_LOG_DIR", ".")
        cls.log_path = os.path.join(log_dir, 'test_result.log')
        with open(cls.log_path, 'w') as log:
            log.write('Результаты пройденного теста:\n')

    def identify_subscription_screen(self, sub_button_path='tests/sub_button.png', threshold=0.6) -> bool:
        time.sleep(20)
        screenshot_path = "screenshot.png"
        self.driver.save_screenshot(screenshot_path)

        screenshot = cv2.imread(screenshot_path)
        sub_button_template = cv2.imread(sub_button_path)
        # template_height, template_width = sub_button_template.shape[:2]

        methods = [cv2.TM_CCOEFF_NORMED, cv2.TM_CCOEFF, cv2.TM_CCORR_NORMED, cv2.TM_CCORR, cv2.TM_SQDIFF_NORMED]

        sub_found = False

        for method in methods:
            result = cv2.matchTemplate(screenshot, sub_button_template, method)

            if method == cv2.TM_SQDIFF_NORMED:
                locations = np.where(result <= 1 - threshold)
            else:
                locations = np.where(result >= threshold)

            if len(locations[0]) > 0:
                y, x = locations[0][0], locations[1][0]

                # center_x = x + template_width // 2
                # center_y = y + template_height // 2
                sub_found = True
                break
        return sub_found

    def test_app_launch(self):
        time.sleep(5)
        try:
            assert self.driver.current_activity is not None, "Приложение не запустилось"
            with open(self.log_path, 'a') as log:
                log.write('Приложение успешно запускается\n')
        except Exception as e:
            with open(self.log_path, 'a') as log:
                log.write(f'Ошибка: Приложение не запустилось: {e}\n')
            self.fail(f"Приложение не запускается: {e}")

    def test_is_subscription_displayed(self) -> None:
        try:
            time.sleep(20)

            screenshot_path = "screenshot.png"
            self.driver.save_screenshot(screenshot_path)

            is_subscription_displayed = self.identify_subscription_screen()
            assert is_subscription_displayed, 'Подписка не отображается'
            with open(self.log_path, 'a') as log:
                log.write('Подписка успешно отображается\n')

            if os.path.exists(screenshot_path):
                os.remove(screenshot_path)

        except Exception as e:
            with open(self.log_path, 'a') as log:
                log.write(f'Ошибка: Подписка не отображается: {e}\n')
            self.fail(f"Ошибка показа подписки: {e}")


if __name__ == '__main__':
    unittest.main()
