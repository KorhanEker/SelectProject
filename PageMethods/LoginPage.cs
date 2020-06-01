using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelectTest.HelperMethods;
using System.Data;
using SelectTest.Config;
using AventStack.ExtentReports;
using OpenQA.Selenium.Chrome;
using System.Threading;
using LogType = SelectTest.Config.LogType;

namespace SelectTest.PageMethods
{
    class LoginPage : ReportsGenerationClass
    {
        private IWebDriver driver;
        private ExtentTest test;
        private WebDriverWait wait;



        checkResult result = new checkResult();

        public LoginPage(IWebDriver driver, ExtentTest test,WebDriverWait wait)
        {
            this.driver = driver;
            this.test = test ?? throw new ArgumentNullException(nameof(test));
            this.wait = wait;
        }
        public bool goToPage(string url)
        {
            checkResult result = new checkResult();
            if (!url.StartsWith("https://")) url = "https://" + url;
            if (Extensions.URLCheckTest(url))
            {
                driver.Navigate().GoToUrl(url);
                return true;
            }
            else
            {
                result.log = "URL is not existing : " + url;
                result.logType = LogType.FATAL;
                insertLog(test,driver,result,false);
                return false;
            }
            
        }

        public void clickCookieP()
        {
            if (driver.FindElements(By.CssSelector(Locator.cookiePolicy)).Count > 0)
                driver.FindElement(By.CssSelector(Locator.cookiePolicy)).ClickAction(driver);
            else
            {
                result.log = "Cookie policy frame was not existing on the page.";
                result.logType = LogType.WARNING;
                insertLog(test,driver,result,true);
            }
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        public void enterFieldValue(string text, string fieldLocation)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            if(!String.IsNullOrEmpty(text) && driver.FindElements(By.Name(fieldLocation)).Count()>0)
            {
                IWebElement element = driver.FindElement(By.Name(fieldLocation));
                element.SendKeys(text);
                if (String.IsNullOrEmpty(element.GetAttribute("value")) || element.GetAttribute("value") != text)
                {
                    switch(fieldLocation)
                    {
                        case Locator.codeInput:
                            result.log = "Code can't be input into Code Entry field correctly. Please check the code value";
                            break;
                        case Locator.pinInput:
                            result.log = "Pin can't be input into Pin Entry field correctly. Please check the pin value";
                            break;
                        case Locator.cv2Input:
                            result.log = "CVV can't be input into CVV Entry field correctly. Please check the cvv value";
                            break;
                        case Locator.expiryDateInput:
                            result.log = "Expiry date can't be input into corresponding field correctly. Please check the expiry date value";
                            break;
                    }
                    result.logType = LogType.FAIL;
                    insertLog(test,driver,result,true);
                }
            }
            else
            {
                switch(fieldLocation)
                {
                    case Locator.codeInput:
                        result.log = "Code Entry field is not found.";
                        result.logType = LogType.FATAL;
                        insertLog(test,driver,result,true);
                        break;
                    case Locator.pinInput:
                        result.log = "Pin Entry field is not found/used";
                        result.logType = LogType.INFO;
                        insertLog(test,driver,result,false);
                        break;
                    case Locator.cv2Input:
                        result.log = "CVV2 Entry field is not found/used";
                        result.logType = LogType.INFO;
                        insertLog(test,driver,result,false);
                        break;
                    case Locator.expiryDateInput:
                        result.log = "Expiry Date field is not found/used";
                        result.logType = LogType.INFO;
                        insertLog(test,driver,result,false);
                        break;
                }
                
            }
        }

        public void checkErrorNotification()
        {
            var result = new checkResult(); 
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            if (driver.FindElements(By.CssSelector("div.noty_bar.noty_type__error.noty_theme__mint.noty_close_with_click.noty_close_with_button")).Count() > 0)
            {
                IList<IWebElement> errors = driver.FindElements(By.CssSelector("div.noty_bar.noty_type__error.noty_theme__mint.noty_close_with_click.noty_close_with_button"));
                result.log += "Tested site Url : " + driver.Url +" : Invalid input detected. One or more input fields empty or wrong input. Error message displayed : ";
                foreach (IWebElement element in errors)
                {
                    result.log += element.Text.ToString();
                }
                result.logType = LogType.FAIL;
            }
            else if(driver.FindElements(By.CssSelector("span.error")).Count()>0)
            {
                result.log = "Missing input field detected : ";
                result.logType = LogType.FAIL;
                IList<IWebElement> list = driver.FindElements(By.CssSelector("span.error"));
                foreach (IWebElement el in list)
                {
                    result.log += el.Text + " - ";
                }
            }
            else
            {
                result.logType = LogType.SUCCESS;
                result.log += "Tested site URL : " + driver.Url + " - Successfully logged in.";
            }
            insertLog(test,driver,result,true);
        }

        public void clickbtnContinue()
        {
            if (driver.FindElements(By.XPath(Locator.btnContinue)).Count() > 0 && driver.FindElement(By.XPath(Locator.btnContinue)).Enabled)
            {
                result.log = "Code input fields are populated. Screenshot taken for reference";
                result.logType = LogType.INFO;
                insertLog(test,driver,result,true);
                driver.FindElement(By.XPath(Locator.btnContinue)).ClickAction(driver);
            }
            else
            {
                result.log = "Continue Button can't be found on the page. Check screenshot and site";
                result.logType = LogType.FATAL;
                insertLog(test,driver,result,true);
            }
        }

        public Boolean verifyElement(string key, selectorType type)
        {
            Boolean res = false;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            if (type == selectorType.Id) 
            {
                if (driver.FindElements(By.Id(key)).Count() > 0)
                {
                    // Check whether it's chooce page informational notification
                    if(driver.FindElements(By.XPath("//*[@class='catalogWrap']")).Count()<=0)                      
                        res = driver.FindElement(By.Id(key)).Displayed;
                }
            }
            else if (type == selectorType.XPath)
            {
                if(driver.FindElements(By.XPath(key)).Count()>0)
                {
                    res = driver.FindElement(By.XPath(key)).Displayed;
                }
            }
            return res;
        }

        public void closeBrowser()
        {
            driver.Close();
        }
    }

}