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
using NUnit.Framework;
using AppiumSmoke.DriverOptions;

namespace AppiumSmoke.Tests
{
    [TestFixture]
    public class InstallApp : DriverSetup
    {
        private readonly string APK_PATH = @"C:\Users\egork\source\repos\TestProject1\TestProject1\testApk\AmazonSDK.apk";
        [Test, Category ("Install")]
        public void InstallAppTest()
        {
            if (_driver != null)
            {
                try
                {

                    _driver.InstallApp(APK_PATH);
                    Console.WriteLine("APK installed success.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Driver is not initialized.");
            }

        }
    }
}
