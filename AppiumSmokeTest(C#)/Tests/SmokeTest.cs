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
using AppiumSmoke.Pages;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.DevTools.V123.Performance;
using AppiumSmoke.DriverOptions;

namespace AppiumSmoke.Tests
{
    [TestFixture, Category ("Smoke")]
    public class SmokeTest : DriverSetup
    {
        private MainActivityPage _mainActivityPage;

        [SetUp]
        public void TestSetup()
        {
            _mainActivityPage = new MainActivityPage(_driver);
            _mainActivityPage.RunApp();
        }



        [Test, Category("Banner")]
        public void BannerAdTest()
        {
            _mainActivityPage.ShowBanner();

            if (_mainActivityPage.IsAdElementVisible(By.XPath
                ("//android.widget.FrameLayout[@resource-id=\"android:id/content\"]/android.widget.FrameLayout[2]/android.widget.FrameLayout")))
            {
                Console.WriteLine("Ad banner is displayed.");
            }
            else
            {
                Console.WriteLine("Ad banner is not displayed.");
            }

            _mainActivityPage.CloseBanner();

            if (_mainActivityPage.IsAdElementInvisible(By.XPath
                ("//android.widget.FrameLayout[@resource-id=\"android:id/content\"]/android.widget.FrameLayout[2]/android.widget.FrameLayout")))
            {
                Console.WriteLine("Ad banner is closed.");
            }
            else
            {
                Console.WriteLine("Ad banner is still displayed.");
            }

        }


        [Test, Category ("Inter")]
        public void InterstitialAdTest()
        {
            _mainActivityPage.ShowInterstitial();

            if(_mainActivityPage.IsAdElementVisible(By.XPath("//android.view.View[@text=\"i\"]")))
            {
                Console.WriteLine("IronSource Demo is displayed");
            }
            else
            {
                Console.WriteLine("No IronSource Demo");
            }
            System.Threading.Thread.Sleep(16000);

            _mainActivityPage.CloseAdVideo();
            if (_mainActivityPage.IsMainScreenDisplayed())
            {
                Console.WriteLine("Inter's closed success");
            }
            else
            {
                Console.WriteLine("Inter's not closed");
            }
           
        }


        [Test, Category ("Reward")]
        public void RewardAdTest()
        {
            _mainActivityPage.ShowReward();

            if (_mainActivityPage.IsAdElementVisible(By.XPath("//android.view.View[@text=\"i\"]")))
            {
                Console.WriteLine("IronSource Demo is displayed");
            }
            else
            {
                Console.WriteLine("No IronSource Demo");
            }
            System.Threading.Thread.Sleep(16000);

            _mainActivityPage.CloseAdVideo();
            if (_mainActivityPage.IsMainScreenDisplayed())
            {
                Console.WriteLine("Reward's closed success");
            }
            else
            {
                Console.WriteLine("Reward's not closed");
            }

        }
    }
}
