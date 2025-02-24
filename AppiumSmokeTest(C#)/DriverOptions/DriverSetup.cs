using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;

namespace AppiumSmoke.DriverOptions
{
    public class DriverSetup
    {
        public AndroidDriver _driver;
        [SetUp]
        public void Setup()
        {
            var serverUri = new Uri("http://127.0.0.1:4723");


            var driverOptions = new AppiumOptions()
            {
                AutomationName = AutomationName.AndroidUIAutomator2,
                PlatformName = "Android",
                DeviceName = "Mumu12",
            };

            driverOptions.AddAdditionalAppiumOption(MobileCapabilityType.Udid, "127.0.0.1:7555"); //MuMu Player 12
            driverOptions.AddAdditionalAppiumOption("noReset", true);
            try
            {
                _driver = new AndroidDriver(serverUri, driverOptions, TimeSpan.FromSeconds(180));
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

        }
        [TearDown]
        public void TearDown()
        {
            _driver.TerminateApp("com.amazon.woodturning");
            _driver.Dispose();
        }

    }
}
