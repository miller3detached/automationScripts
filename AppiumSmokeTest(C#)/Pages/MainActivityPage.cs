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
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Interactions;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;


namespace AppiumSmoke.Pages
{
    public class MainActivityPage
    {
        private AndroidDriver _driver;
        private WebDriverWait _wait;

        public MainActivityPage(AndroidDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }
        public class Point
        {
            public int X { get; }
            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }


        Point ShowBannerButton = new Point(550, 1225); 
        Point HideBannerButton = new Point(880, 1225);
        Point ShowInterstitialButton = new Point(550, 1410);
        Point CloseAdButton = new Point(1370, 63);
        Point ShowRewardButton = new Point(880, 1410);

        string MainScreenPath = "//android.view.View[@content-desc=\"Game view\"]";
        string BannerAdPath = "//android.widget.FrameLayout[@resource-id=\"android:id/content\"]/android.widget.FrameLayout[2]/android.widget.FrameLayout";
        string AdInfoButtonPath = "//android.view.View[@text=\"i\"]";


        public void RunApp()
        {
            _driver.FindElement(By.XPath("//android.widget.TextView[@text=\"external-sdk\"]")).Click();
            
            if (IsMainScreenDisplayed())
            {
                System.Threading.Thread.Sleep(10000);
            }
            else { }
        
            
        }
        public void ClickOnButton(int x, int y)
        {
            var finger = new PointerInputDevice(PointerKind.Touch);
            var action = new ActionSequence(finger, 0);
            action.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.Zero)); 
            action.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            action.AddAction(finger.CreatePause(TimeSpan.FromMilliseconds(200)));
            action.AddAction(finger.CreatePointerUp(MouseButton.Touch));
            _driver.PerformActions(new List<ActionSequence> { action });

            System.Threading.Thread.Sleep(2000);
        }
        public void ShowBanner()
        {
            ClickOnButton(ShowBannerButton.X, ShowBannerButton.Y);
            
        }
        public void CloseBanner()
        {
            ClickOnButton(HideBannerButton.X, HideBannerButton.Y);
        }
        public void ShowInterstitial()
        {
            ClickOnButton(ShowInterstitialButton.X, ShowInterstitialButton.Y);
        }
        public void CloseAdVideo()
        {
            ClickOnButton(CloseAdButton.X, CloseAdButton.Y);
            System.Threading.Thread.Sleep(5000);
            ClickOnButton(CloseAdButton.X, CloseAdButton.Y);

        }
        public void ShowReward()
        {
            ClickOnButton(ShowRewardButton.X, ShowRewardButton.Y);
        }
        public bool IsAdElementVisible(By locator)
        {
            try
            {
                _wait.Until(driver =>
                {
                    try
                    {
                        var element = _driver.FindElement(locator);
                        return element != null && element.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return false;
                    }
                });
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }

        }
        public bool IsAdElementInvisible(By locator)
        {
            try
            {
                _wait.Until(driver =>
                {
                    try
                    {
                        var element = _driver.FindElement(locator);
                        return element == null || !element.Displayed;
                    }
                    catch (NoSuchElementException)
                    {
                        return true;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                });
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            
        }
        public bool IsMainScreenDisplayed()
        {
            try
            {
                _wait.Until(driver => driver.FindElement(By.XPath("//android.view.View[@content-desc=\"Game view\"]")).Displayed);
                Console.WriteLine("Main menu is displayed.");
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Main menu's disabled");
                return false;
            }
        }
    }
    
}
