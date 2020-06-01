using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Data;
using SelectTest.HelperMethods;
using OpenQA.Selenium.Support.UI;
using SelectTest.RecordingHelper;
using System.Reflection;
using OpenQA.Selenium.Remote;
using AventStack.ExtentReports.Configuration;
using System.Configuration;

namespace SelectTest.Config
{
    [SetUpFixture]
    public abstract class ReportsGenerationClass
    {
        public ExtentReports _extent;
        public ExtentTest _test;
        public IWebDriver _driver;
        public WebDriverWait _wait;
        public DataTable _dtResource;
        public ScreenRecorder _sr;
        public static string resourcePath_Login_Valid;
        public static string resourcePath_Login_Invalid;
        public static string resourcePath_ChoiceRedemptionTest;
        public static string resourcePath_PromotionalSite;
        public static string resourcePath_Images_Valid;
        public static string resourcePath_Images_InvalidDimension;
        public static string resoucePath_Images_InvalidSize;
        public string _testScreenCaptureFileName = string.Empty;
        public static string resourcePath_ScreenshotsRoot;
        public string finalReport = string.Empty;

        [OneTimeSetUp]
        protected void Setup()
        {
            var path = Assembly.GetCallingAssembly().CodeBase;
            var actualPath = path.Substring(0, path.LastIndexOf("bin"));
            var projectPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(projectPath.ToString() + "Reports");
            var reportPath = projectPath + "Reports\\ExtentReport.html";
            var htmlReporter = new ExtentHtmlReporter(reportPath);
            finalReport = projectPath + "Reports\\dashboard.html";
            resourcePath_Login_Valid = Path.Combine(projectPath, "Resource\\Login.xlsx");
            resourcePath_Login_Invalid = Path.Combine(projectPath, "Resource\\InvalidLogin.xlsx");
            resourcePath_ChoiceRedemptionTest = Path.Combine(projectPath, "Resource\\RedemptionTest.xlsx");
            resourcePath_PromotionalSite = Path.Combine(projectPath, "Resource\\PromotionalTest.xlsx");
            resourcePath_ScreenshotsRoot = Path.Combine(projectPath, "Reports\\Screenshots");
            resourcePath_Images_Valid = Path.Combine(projectPath, "Resource\\images\\Logo200x200.png");
            resourcePath_Images_InvalidDimension = Path.Combine(projectPath, "Resource\\images\\Logo860x700.png");
            resoucePath_Images_InvalidSize = Path.Combine(projectPath, "Resource\\images\\LargeImage.jpg");
            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);
            _extent.AddSystemInfo("Host Name", "LocalHost");
            _extent.AddSystemInfo("Environment", "QA");
            _extent.AddSystemInfo("UserName", "TestUser");
            //Delete all screenshots before starting test
            string[] files = Directory.GetFiles(resourcePath_ScreenshotsRoot);
            foreach(string file in files)
            {
                File.Delete(file);
            }
        }
        [OneTimeTearDown]
        protected void TearDown()
        {
            _extent.Flush();
        }
        [SetUp]
        public void BeforeTest()
        {
            // ChromeDriverService service = ChromeDriverService.CreateDefaultService("webdriver.chrome.driver", @"D:\\Automation\\WebDrivers\\chromedriver.exe");
            _driver = new ChromeDriver();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            _driver.Manage().Window.Maximize();
            _test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(5));
            _sr = new ScreenRecorder();
            var recordVideo = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["RecordVideo"]);
            if (recordVideo)
            {
                _sr.SetVideoOutputLocation();
                _sr.StartRecording();
            }
        }
        [TearDown]
        public void AfterTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
            ? "": string.Format("{0}", TestContext.CurrentContext.Result.StackTrace);
            Status logstatus;
            switch (status)
            {
                case TestStatus.Failed:
                    logstatus = Status.Fail;
                    String fileName = "Screenshot_" + DateTime.Now.ToString("h_mm_ss") + ".png";
                    String screenShotPath = Capture((ChromeDriver)_driver);
                    _test.Log(Status.Fail, "Fail");
                    _test.Log(Status.Fail, "Snapshot below: " +_test.AddScreenCaptureFromPath("Screenshots\\" +fileName));
                    break;
                case TestStatus.Inconclusive:
                    logstatus = Status.Warning;
                    break;
                case TestStatus.Skipped:
                    logstatus = Status.Skip;
                    break;
                default:
                    logstatus = Status.Pass;
                    break;
            }
            _test.Log(logstatus, "Test ended with " +logstatus + stacktrace);
            _extent.Flush();
            _sr.StopRecording();
        }
        public IWebDriver GetDriver()
        {
            return _driver;
        }
        public ExtentTest GetTest()
        {
            return _test;
        }
        public WebDriverWait GetWait()
        {
            return _wait;
        }
        public static string Capture(ChromeDriver driver)
        {
            //Dictionary will contain the parameters needed to get the full page screen shot
            Dictionary<string, Object> metrics = new Dictionary<string, Object>();
            metrics["width"] = driver.ExecuteScript("return Math.max(window.innerWidth,document.body.scrollWidth,document.documentElement.scrollWidth)");
            metrics["height"] = driver.ExecuteScript("return Math.max(window.innerHeight,document.body.scrollHeight,document.documentElement.scrollHeight)");
            metrics["deviceScaleFactor"] = (double)driver.ExecuteScript("return window.devicePixelRatio");
            metrics["mobile"] = driver.ExecuteScript("return typeof window.orientation !== 'undefined'");
            //Execute the emulation Chrome Command to change browser to a custom device that is the size of the entire page
            driver.ExecuteChromeCommand("Emulation.setDeviceMetricsOverride", metrics);

            DateTime time = DateTime.Now;
            String fileName = "Screenshot_" + time.ToString("h_mm_ss") + ".png";
            ITakesScreenshot ts = (ITakesScreenshot)driver; 
            Screenshot screenshot = ts.GetScreenshot();
            var pth = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            var actualPath = pth.Substring(0, pth.LastIndexOf("bin"));
            var reportPath = new Uri(actualPath).LocalPath;
            Directory.CreateDirectory(reportPath + "Reports\\" + "Screenshots");
            var finalpth = pth.Substring(0, pth.LastIndexOf("bin")) + "Reports\\Screenshots\\" + fileName;
            var localpath = new Uri(finalpth).LocalPath;
            screenshot.SaveAsFile(localpath, ScreenshotImageFormat.Png);
            //This command will return your browser back to a normal, usable form if you need to do anything else with it.
            driver.ExecuteChromeCommand("Emulation.clearDeviceMetricsOverride", new Dictionary<string, Object>());
            return localpath;
        }
        public void PopulateResource(TestSource source)
        {
            if (source == TestSource.Login)
                _dtResource = ExcelUtil.ExcelToDataTable(resourcePath_Login_Valid);
            else if (source == TestSource.InvalidLogin)
                _dtResource = ExcelUtil.ExcelToDataTable(resourcePath_Login_Invalid);
            else if (source == TestSource.ChoiceRedemption)
                _dtResource = ExcelUtil.ExcelToDataTable(resourcePath_ChoiceRedemptionTest);
            else if (source == TestSource.PromotionalSite)
                _dtResource = ExcelUtil.ExcelToDataTable(resourcePath_PromotionalSite);
        }
        public void insertLog(ExtentTest test, IWebDriver driver,checkResult result, bool takeScreenShot)
        {
            String screenShotPath = String.Empty;
            if (takeScreenShot) screenShotPath = Capture((ChromeDriver)driver);
            if (result.logType == LogType.FAIL)
            {
                test.Log(Status.Fail, "Fail : " + result.log);
                if (takeScreenShot) test.AddScreenCaptureFromPath(screenShotPath);
            }
            else if (result.logType == LogType.SUCCESS)
            {
                test.Log(Status.Pass, "Success : " + result.log);
                if (takeScreenShot) test.AddScreenCaptureFromPath(screenShotPath);
            }
            else if (result.logType == LogType.WARNING)
            {
                test.Log(Status.Warning, "Warning : " + result.log);
                if (takeScreenShot) test.AddScreenCaptureFromPath(screenShotPath);
            }
            else if (result.logType == LogType.INFO)
            {
                test.Log(Status.Info, "Info : " + result.log);
                if (takeScreenShot) test.AddScreenCaptureFromPath(screenShotPath);
            }
            else if (result.logType == LogType.FATAL)
            {
                test.Log(Status.Fatal, "Fatal (Exception) : " + result.log);
                if (takeScreenShot) test.AddScreenCaptureFromPath(screenShotPath);
            }
        }
    }
    public class checkResult
    {
        public  string log;
        public  LogType logType = LogType.SUCCESS;
    }
    public enum LogType
    {
        FAIL = 0,
        SUCCESS = 1,
        WARNING = 2,
        INFO = 3,
        FATAL = 4
    }
}