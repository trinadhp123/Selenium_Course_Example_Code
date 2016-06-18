﻿using System;
using System.Configuration;
using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Tests
{
    [TestFixture]
    class BaseTest
    {
        public IWebDriver Driver;
        public static string ApplicationBaseUrl;
        protected static string BrowserName;
        protected static string VendorDirectory;
        protected static string Host;
        protected static string BrowserVersion;
        protected static string Platform;

        private void LoadConfigValues()
        {
            var ConfigReader    = new AppSettingsReader();
            Host                = (string)ConfigReader.GetValue("Host", typeof(string));
            BrowserName         = (string)ConfigReader.GetValue("BrowserName", typeof(string));
            BrowserVersion      = (string)ConfigReader.GetValue("BrowserVersion", typeof(string));
            Platform            = (string)ConfigReader.GetValue("Platform", typeof(string));
            ApplicationBaseUrl  = (string)ConfigReader.GetValue("ApplicationBaseUrl", typeof(string));
            VendorDirectory     = System.IO.Directory.GetParent(
                                    System.IO.Path.GetDirectoryName(
                                    typeof(Tests.BaseTest).Assembly.Location)).
                                    Parent.FullName + @"\Vendor";
        }

        [SetUp]
        protected void SetUp()
        {
            LoadConfigValues();
            switch (Host.ToLower())
            {
                case "localhost":
                    switch (BrowserName.ToLower())
                    {
                        case "firefox":
                            Driver = new FirefoxDriver();
                            break;
                        case "chrome":
                            Driver = new ChromeDriver(VendorDirectory);
                            break;
                    }
                    break;
                case "saucelabs":
                    DesiredCapabilities caps = new DesiredCapabilities();
                    caps.SetCapability(CapabilityType.BrowserName, BrowserName);
                    caps.SetCapability(CapabilityType.Version, BrowserVersion);
                    caps.SetCapability(CapabilityType.Platform, Platform);
                    caps.SetCapability("username", System.Environment.GetEnvironmentVariable("SAUCE_USERNAME"));
                    caps.SetCapability("accessKey", System.Environment.GetEnvironmentVariable("SAUCE_ACCESS_KEY"));
                    caps.SetCapability("name", TestContext.CurrentContext.Test.Name);
                    Driver = new RemoteWebDriver(new Uri("http://ondemand.saucelabs.com:80/wd/hub"), caps);
                    break;
            }
        }

        [TearDown]
        protected void TearDown()
        {
            Driver.Quit();
        }
    }
}