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
using System.Threading;
using OpenQA.Selenium.Interactions;
using NUnit.Framework;
using LogType = SelectTest.Config.LogType;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
//using OpenQA.Selenium.DevTools.Debugger;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using javax.xml.transform;
using com.sun.xml.@internal.bind.v2.runtime.unmarshaller;
using OpenQA.Selenium.DevTools.Debugger;
using com.sun.org.apache.xerces.@internal.impl.dv.xs;
using com.sun.xml.@internal.bind.v2.model.core;
using javax.swing.text.html;
using System.Globalization;
using java.time;
using OpenQA.Selenium.DevTools.Page;

namespace SelectTest.PageMethods
{
    class PromotionalSite : ReportsGenerationClass
    {
        private IWebDriver driver;
        private ExtentTest test;
        private WebDriverWait wait;

        string loremIpsumLongText = LoremIpsum(90, 120, 10, 20, 1, 630);//"Lorem ipsum dolor sit amet, consectetur adipiscing elit.In facilisis odio diam.Phasellus sit amet augue hendrerit, vehicula massa vitae, placerat erat. Phasellus condimentum varius dolor sed viverra. Donec at leo in magna vulputate dictum ut a tellus. Vestibulum ac vehicula libero. Etiam rutrum lacinia orci eget varius. Aliquam urna erat, laoreet nec sagittis vel, eleifend eu magna.Phasellus at lobortis metus. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut ultricies dictum turpis, non bibendum elit maximus id.Proin sit amet mauris nec ipsum mattis hendrerit. Vestibulum porttitor eros et odio interdum gravida.";
        string loremIpsumValidText = LoremIpsum(15, 40, 5, 20, 2, 150);//"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin nec finibus est. Nunc tempor, justo ac gravida commodo, magna mi ultricies est.";

        public PromotionalSite(IWebDriver driver, ExtentTest test, WebDriverWait wait)
        {
            this.driver = driver;
            this.test = test ?? throw new ArgumentNullException(nameof(test));
            this.wait = wait;
        }

        public bool openSite(string url)
        {
            checkResult result = new checkResult();
            try
            {
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
                    insertLog(test, driver, result, false);
                    return false;
                }
            }
            catch (Exception ex)
            {
                result.log = " Site can't be opened : URL = " + url + " error message : " + ex.Message;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
                return false;
            }
        }
        public void clickCookieP()
        {
            checkResult result = new checkResult();
            try
            {
                if (driver.FindElements(By.CssSelector(Locator.cookiePolicy)).Count > 0)
                    driver.FindElement(By.CssSelector(Locator.cookiePolicy)).ClickAction(driver);
                else
                {
                    result.log = "Cookie policy frame was not existing on the page.";
                    result.logType = LogType.WARNING;
                    insertLog(test, driver, result, true);
                }
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                result.log = " An exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void orderNowBtnTest()
        {
            checkResult result = new checkResult();
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                if (driver.FindElements(By.CssSelector(Locator.orderNowBtn_1)).Count() > 0)
                {
                    driver.FindElement(By.CssSelector(Locator.orderNowBtn_1)).ClickAction(driver); //Click the button
                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
                    wait.Timeout = TimeSpan.FromSeconds(10);
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locator.navigation_bar)));
                    if (driver.FindElements(By.CssSelector(Locator.navigation_bar)).Count() > 0) // Check whether the template page is loaded
                    {
                        result.log = "Order button is clicked. Template Customisation page is loaded";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void insertLogoTest(string imagePath,bool fullTest)
        {
            checkResult result = new checkResult();
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.logodrop)));
                if (driver.FindElements(By.CssSelector(Locator.logodrop)).Count() > 0)
                {
                    IWebElement element = driver.FindElement(By.CssSelector(Locator.logodrop));
                    if (String.IsNullOrEmpty(imagePath))
                    {
                        if (fullTest)
                        {
                            CheckImageDisplayed(resoucePath_Images_InvalidSize, element, result);
                            CheckImageDisplayed(resourcePath_Images_InvalidDimension, element, result);
                            CheckImageDisplayed(resourcePath_Images_Valid, element, result);
                        }
                    }
                    else
                    {
                        // Only enables uploading image from the project's directory itself. 
                        var path = Assembly.GetCallingAssembly().CodeBase;
                        var actualPath = path.Substring(0, path.LastIndexOf("bin"));
                        var projectPath = new Uri(actualPath).LocalPath;
                        CheckImageDisplayed(Path.Combine(projectPath, imagePath), element, result);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        private void CheckImageDisplayed(string imagePath, IWebElement element, checkResult result)
        {
            element.SendKeys(imagePath);
            Actions actions = new Actions(driver);
            actions.MoveToElement(element).Perform();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            if (driver.FindElements(By.CssSelector(Locator.logoTitlePath)).Count() > 0)
            {
                result.log = "Valid icon is uploaded successfully";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                //check if warning message is given
                if (driver.FindElements(By.CssSelector(Locator.logoErrorMessage)).Count() > 0)
                {
                    result.log = "Icon can't be uploaded. Upload image location: " + resoucePath_Images_InvalidSize;
                    result.logType = LogType.WARNING;
                    insertLog(test, driver, result, true);
                }
            }
        }

        public void secureTradingTest(DataRow row)
        {
            // I haven't added css locators to the locator class as most of them are by ID and very short.
            checkResult result = new checkResult();
            try
            {
                if (driver.FindElements(By.CssSelector(Locator.confirmationPageCheckoutModalPopup)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(Locator.confirmationPageCheckoutBtn)).ClickAction(driver);
                    if (driver.FindElements(By.CssSelector(Locator.confirmationPageCheckOutModalContinueBtn)).Count() > 0)
                        driver.FindElement(By.CssSelector(Locator.confirmationPageCheckOutModalContinueBtn)).ClickAction(driver);
                }

                if (driver.FindElements(By.CssSelector(Locator.confirmationPageCheckoutModalPopup)).Count() > 0)
                {
                    driver.FindElement(By.CssSelector(Locator.confirmationPageCheckOutModalContinueBtn)).ClickAction(driver);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.secureTradingStandardDetails)));
                    if (!String.IsNullOrEmpty(row["STCardType"].ToString()))
                    {
                        CardType card = determineCard(row);
                        switch (card)
                        {
                            case CardType.VisaCredit:
                                driver.FindElement(By.CssSelector("#VISA > input.paymentcard")).ClickAction(driver);
                                break;
                            case CardType.VisaDebit:
                                driver.FindElement(By.CssSelector("#DELTA > input.paymentcard")).ClickAction(driver);
                                break;
                            case CardType.VisaPurchasing:
                                driver.FindElement(By.CssSelector("#PURCHASING > input.paymentcard")).ClickAction(driver);
                                break;
                            case CardType.Mastercard:
                                driver.FindElement(By.CssSelector("#MASTERCARD > input.paymentcard")).ClickAction(driver);
                                break;
                            case CardType.MastercardDebit:
                                driver.FindElement(By.CssSelector("#MASTERCARDDEBIT > input.paymentcard")).ClickAction(driver);
                                break;
                            default:
                                driver.FindElement(By.CssSelector("#DELTA > input.paymentcard")).ClickAction(driver);
                                break;
                        }
                    }
                    else
                        driver.FindElement(By.CssSelector("#DELTA > input.paymentcard")).ClickAction(driver); // select visa debit for test

                    // Check billing details
                    // TBD

                    // Input Payment Details
                    //Card Number Input
                    if (!String.IsNullOrEmpty(row["Card Number (Valid)"].ToString()))
                        driver.FindElement(By.CssSelector("#st-pan-textfield")).SendKeys(row["Card Number (Valid)"].ToString());
                    else
                        driver.FindElement(By.CssSelector("#st-pan-textfield")).SendKeys("4111110000000211");
                    // Expiry Month Input
                    var monthPicker = driver.FindElement(By.XPath("//*[@id='st-expirymonth-dropdown']"));
                    var monthSelect = new SelectElement(monthPicker);
                    int m = new Random().Next(1, 12);
                    monthSelect.SelectByValue(m.ToString());
                    // Expiry Year Input
                    DateTime dt = DateTime.Today;
                    int myYear = dt.AddYears(1).Year;
                    var yearPicker = driver.FindElement(By.CssSelector("#st-expiryyear-dropdown"));
                    var yearSelect = new SelectElement(yearPicker);
                    yearSelect.SelectByValue(myYear.ToString());
                    // CVV Input
                    if (!String.IsNullOrEmpty(row["CVV Valid"].ToString()))
                        driver.FindElement(By.CssSelector("#st-securitycode-textfield")).SendKeys(row["CVV Valid"].ToString());
                    else
                        driver.FindElement(By.CssSelector("#st-securitycode-textfield")).SendKeys("123");

                    IWebElement submitBtn = driver.FindElement(By.CssSelector("#submit"));
                    if (!submitBtn.Enabled)
                    {
                        result.log = "Secure Trading: Pay Securely button is not enabled";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                    else
                    {
                        submitBtn.ClickAction(driver);
                        if(driver.FindElements(By.CssSelector(Locator.secureTradingSubmitErrorMessage)).Count()>0)
                        {

                            // check for failed value
                            string elementColour, lookupVal = "#FFC6C7"; //securetrading error highlighting colour
                            IList<IWebElement> billingGroup = driver.FindElements(By.CssSelector(Locator.secureTradingBillingGroup));
                            foreach(IWebElement group in billingGroup)
                            {
                                IList<IWebElement> fields = group.FindElements(By.CssSelector("div>input"));
                                foreach(IWebElement field in fields)
                                {
                                    elementColour = field.GetAttribute("background-color");
                                    if (!String.IsNullOrEmpty(elementColour) && elementColour.Equals(lookupVal))
                                    {
                                        string elementName = field.GetAttribute("name");
                                        if (elementName == "billingfirstname") field.SendKeys(row["First Name"].ToString());
                                        if (elementName == "billinglastname") field.SendKeys(row["Last Name"].ToString());
                                        if (elementName == "billingpremise") field.SendKeys(row["AddressFirstLine"].ToString());
                                        if (elementName == "billingstreet") field.SendKeys(row["AddressSecondLine"].ToString());
                                        if (elementName == "billingtown") field.SendKeys(row["AddressTown"].ToString());
                                        if (elementName == "billingcounty") field.SendKeys(row["AddressCounty"].ToString());
                                        if (elementName == "billingpostcode") field.SendKeys(row["AddressPostCode"].ToString());
                                        if (elementName == "billingemail") field.SendKeys(row["Email Address"].ToString());
                                        if (elementName == "billingtelephone") field.SendKeys(row["Phone Number"].ToString());
                                    }
                                }
                            }
                            submitBtn.ClickAction(driver);
                        }
                        wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.PaymentConfirmationPageIndicator)));
                        if(driver.FindElements(By.CssSelector(Locator.PaymentConfirmationPageIndicator)).Count()>0)
                        {
                            result.log = " Payment Confirmation Page has been accessed successfully";
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, true);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        private static CardType determineCard(DataRow row)
        {
            string cardType = row["STCardType"].ToString();
            CardType card = CardType.VisaDebit;
            if (cardType.Equals("Visa Credit")) card = CardType.VisaCredit;
            else if (cardType.Equals("Visa Debit")) card = CardType.VisaDebit;
            else if (cardType.Equals("Visa Purchasing")) card = CardType.VisaPurchasing;
            if (cardType.Equals("Mastercard")) card = CardType.Mastercard;
            if (cardType.Equals("Mastercard Debit")) card = CardType.MastercardDebit;
            return card;
        }

        public void confirmationPageChecksTest(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                //check the status of the Checkout Button and checkboxes
                IWebElement checkbox1 = driver.FindElement(By.CssSelector("#checkbox1"));
                IWebElement checkbox2 = driver.FindElement(By.CssSelector("#checkbox2"));
                IWebElement checkbox3 = driver.FindElement(By.CssSelector("#checkbox3"));

                bool checkBox1Mandatory = false, checkBox2Mandatory = false, checkBox3Mandatory = false;
                if (!String.IsNullOrEmpty(row["Checkbox1Mandatory"].ToString()))
                    checkBox1Mandatory = (row["Checkbox1Mandatory"].ToString() == "Y");
                if(!String.IsNullOrEmpty(row["Checkbox2Mandatory"].ToString()))
                    checkBox2Mandatory = (row["Checkbox2Mandatory"].ToString() == "Y");
                if (!String.IsNullOrEmpty(row["Checkbox3Mandatory"].ToString()))
                    checkBox3Mandatory = (row["Checkbox3Mandatory"].ToString() == "Y");

                IWebElement checkOutBtn = driver.FindElement(By.CssSelector(Locator.confirmationPageCheckoutBtn));
                if((checkBox1Mandatory && !checkbox1.Selected) ||
                   (checkBox2Mandatory && !checkbox2.Selected) ||
                   (checkBox3Mandatory && !checkbox3.Selected)) 
                {
                    if(checkOutBtn.Enabled)
                    {
                        result.log = "Confirmation Page : Checkout Button is enabled without checking mandatory checkboxes";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }

                // Check the checkboxes

                if (!checkbox1.Selected && checkBox1Mandatory) checkbox1.ClickAction(driver);
                if (!checkbox2.Selected && checkBox2Mandatory) checkbox2.ClickAction(driver);
                if (!checkbox3.Selected && checkBox3Mandatory) checkbox3.ClickAction(driver);

                // Check the state of Checkout
                checkOutBtn = driver.FindElement(By.CssSelector(Locator.confirmationPageCheckoutBtn));
                if (!checkOutBtn.Enabled)
                {
                    result.log = "Confirmation Page : Checkout Button is not being enabled after checking all mandatory checkboxes";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
                else
                    checkOutBtn.ClickAction(driver);

                // Check if Secure Trading Popup Opens. Close it then reopen it
                if(driver.FindElements(By.CssSelector(Locator.confirmationPageCheckoutModalPopup)).Count()>0)
                {
                    driver.FindElement(By.CssSelector(Locator.confirmationPageCheckOutModalCancelBtn)).ClickAction(driver);
                }
                // check if modal still visible and checkout is available
                checkOutBtn = driver.FindElement(By.CssSelector(Locator.confirmationPageCheckoutBtn));
                if (driver.FindElements(By.CssSelector(Locator.confirmationPageCheckoutModalPopup)).Count() > 0 || !checkOutBtn.Enabled)
                {
                    result.log = " Confirmation Page - Checkout Confirmation Modal - Go Back button is not working";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
                else
                    checkOutBtn.ClickAction(driver);

            

            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        public void deliveryPageBackAndNextTest()
        {
            checkResult result = new checkResult();
            try
            {
               // ---------------------- test go back functionality -------------------------------------- 
               if(!driver.FindElement(By.CssSelector(Locator.deliveryPageGoBackBtn)).Enabled)
               {
                    result.log = "Delivery page : Back to recipients button is not enabled";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
               }
               else
               {
                    driver.FindElement(By.CssSelector(Locator.deliveryPageGoBackBtn)).ClickAction(driver);
                    //Check if recipients preserved (table should be visible)
                    if(driver.FindElements(By.CssSelector(Locator.recipientsTable)).Count()>0 && driver.FindElement(By.CssSelector(Locator.recipientsTable)).Displayed)
                    {
                        result.log = "Delivery Page : Go Back Button funct - recipients are preserved";
                        result.logType = LogType.INFO;
                        insertLog(test, driver, result, false);
                    }
                    else
                    {
                        result.log = "Delivery Page : Go Back Button funct - recipients are not preserved. Recipient Table is not visible";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }

                    driver.FindElement(By.CssSelector(Locator.recipientsPageContinueBtn)).ClickAction(driver);
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locator.deliveryPageIndicator)));

                    // ---------------------- test go back functionality --------------------------------------
                    if (!driver.FindElement(By.CssSelector(Locator.deliveryPageNextBtn)).Enabled)
                    {
                        result.log = "Delivery page : Next button is not enabled";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    else
                    {
                        driver.FindElement(By.CssSelector(Locator.deliveryPageNextBtn)).ClickAction(driver);
                        //check if confirmation page is displayed
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        if(driver.FindElements(By.CssSelector(Locator.confirmationPageIndicator)).Count()<=0)
                        {
                            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locator.confirmationPageIndicator)));
                            if (driver.FindElements(By.CssSelector(Locator.confirmationPageIndicator)).Count() <= 0)
                            {
                                result.log = "Confirmation Page is not displayed";
                                result.logType = LogType.FAIL;
                                insertLog(test, driver, result, false);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        public void deliveryDetailsTimeTest(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                int timeOption;
                if (!String.IsNullOrEmpty(row["Time Option"].ToString()))
                    timeOption = int.Parse(row["Time Option"].ToString().Substring(0, 1));
                else
                    timeOption = new Random().Next(1, 3);
                timeOption += 5; //options start from 6, goes to 9.
                driver.FindElement(By.CssSelector(Locator.deliveryPageTimeBtn)).ClickAction(driver);
                driver.FindElement(By.CssSelector("#mat-option-" + timeOption.ToString() + ">span.mat-option-text")).ClickAction(driver);
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        public void deliveryDetailsDateTest(DataRow row)
        {
            checkResult result = new checkResult();
            int day,month,year;
            try
            {
                CultureInfo culture = new CultureInfo("en-GB");
                if (!String.IsNullOrEmpty(row["Delivery Date"].ToString()))
                {
                    DateTime dt = Convert.ToDateTime(row["Delivery Date"].ToString(), culture);
                    day = dt.Day;
                    month = dt.Month;
                    year = dt.Year;
                }
                else
                {
                    DateTime dt = DateTime.Today;
                    var theDayAfter = dt.AddDays(2);
                    day = theDayAfter.Day;
                    month = theDayAfter.Month;
                    year = theDayAfter.Year;
                }

                driver.FindElement(By.CssSelector(Locator.deliveryPageDateBtn)).ClickAction(driver);
                // check whether month and year matching
                string monthYear = driver.FindElement(By.CssSelector(Locator.deliveryPageCalendarMonthYear)).Text;
                int monthDigit = DateTime.ParseExact(monthYear.Substring(0,3), "MMM", CultureInfo.InvariantCulture).Month;
                int yearDigit = int.Parse(monthYear.Substring(4, 4));
                if(!(monthDigit == month && yearDigit == year))
                {
                    if(yearDigit == year)
                    {
                        while (monthDigit < month)
                        {
                            driver.FindElement(By.CssSelector(Locator.deliveryPageCalendarNextButton)).ClickAction(driver);
                            monthYear = driver.FindElement(By.CssSelector(Locator.deliveryPageCalendarMonthYear)).Text;
                            monthDigit = DateTime.ParseExact(monthYear.Substring(0, 3), "MMM", CultureInfo.InvariantCulture).Month;
                        }
                    }
                }

                IList <IWebElement> calendarRows = driver.FindElements(By.CssSelector(Locator.deliveryPageCalendarRows));
                foreach (IWebElement calRow in calendarRows)
                {
                    IList<IWebElement> dates = calRow.FindElements(By.CssSelector("td:not([class *= 'disabled']):not([class *= 'mat-calendar-body-label'])"));
                    if (dates.Count > 0)
                    {
                        foreach(IWebElement dateEl in dates)
                        {
                            if(dateEl.FindElement(By.CssSelector("div[class ^= 'mat-calendar-body-cell-content']")).Text == day.ToString())
                            {
                                dateEl.FindElement(By.CssSelector("div[class ^= 'mat-calendar-body-cell-content']")).ClickAction(driver);
                                return;
                            }
                        }
                    }
                    else
                        continue;
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        public void addressManualEntryTest(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement manualEntryBtn = driver.FindElement(By.CssSelector(Locator.deliveryPageManualAddressBtn));
                if (!manualEntryBtn.Enabled)
                {
                    result.log = "Delivery Page : Manual entry button is not enabled";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
                else
                {
                    manualEntryBtn.ClickAction(driver);
                    // Check if address fields are visible and enabled
                    if(driver.FindElements(By.CssSelector("#addressLine1")).Count() <= 0 ||
                       driver.FindElements(By.CssSelector("#addressLine2")).Count() <= 0 ||
                       driver.FindElements(By.CssSelector("#town")).Count() <= 0 ||
                       driver.FindElements(By.CssSelector("#county")).Count() <= 0 ||
                       driver.FindElements(By.CssSelector("#postCode")).Count() <= 0
                       )
                    {
                        result.log = "Delivery Page : Manual address entry fields - Unavailable field";
                        result.logType = LogType.WARNING;
                        insertLog(test, driver, result, true);
                    }
                    else
                    {
                        if(!driver.FindElement(By.CssSelector("#addressLine1")).Enabled ||
                           !driver.FindElement(By.CssSelector("#addressLine2")).Enabled ||
                           !driver.FindElement(By.CssSelector("#town")).Enabled ||
                           !driver.FindElement(By.CssSelector("#county")).Enabled ||
                           !driver.FindElement(By.CssSelector("#postCode")).Enabled)
                        {
                            result.log = "Delivery Page : Manual address entry fields - Uneditable field detected (except Country)";
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, true);
                        }
                        else
                        {
                            // string address1_old, address2_old, town_old, county_old, postcode_old;
                            string address1_new, address2_new, town_new, county_new, postcode_new;

                            //address1_old = driver.FindElement(By.CssSelector("#addressLine1")).GetAttribute("value");
                            //address2_old = driver.FindElement(By.CssSelector("#addressLine2")).GetAttribute("value");
                            //town_old = driver.FindElement(By.CssSelector("#town")).GetAttribute("value");
                            //county_old = driver.FindElement(By.CssSelector("#county")).GetAttribute("value");
                            //postcode_old = driver.FindElement(By.CssSelector("#postCode")).GetAttribute("value");
                            

                            // AddressLine1 update
                            fieldManuelUpdate("#addressLine1",row["AddressFirstLineOverride"].ToString(), out address1_new);
                            //AddressLine2 Update
                            fieldManuelUpdate("#addressLine2", row["AddressSecondLineOverride"].ToString(), out address2_new);
                            //Town/City Update
                            fieldManuelUpdate("#town", row["AddressTownOverride"].ToString(), out town_new);
                            //County Update
                            fieldManuelUpdate("#county", row["AddressCountyOverride"].ToString(),out county_new);
                            //postCode Update
                            fieldManuelUpdate("#postCode", row["AddressPostCodeOverride"].ToString(), out postcode_new);
                            driver.FindElement(By.CssSelector(Locator.deliveryPageManualAddressBtn)).ClickAction(driver);
                            //Check if everything is updated
                            string addressTxt = driver.FindElement(By.CssSelector(Locator.deliveryPageManualAddressTxtArea)).Text;

                            if (!addressTxt.Contains(address1_new)) { result.log += "Delivery Page: Manual Address Input - Address Line 1 can't be updated"; }
                            if (!addressTxt.Contains(address2_new)) { result.log += "Delivery Page: Manual Address Input - Address Line 2 can't be updated"; }
                            if (!addressTxt.Contains(town_new)) { result.log += "Delivery Page: Manual Address Input - City/Town can't be updated"; }
                            if (!addressTxt.Contains(county_new)) { result.log += "Delivery Page: Manual Address Input - County can't be updated"; }
                            if (!addressTxt.Contains(postcode_new)) { result.log += "Delivery Page: Manual Address Input - Postcode can't be updated"; }
                            if(!String.IsNullOrEmpty(result.log))
                            {
                                result.logType = LogType.FAIL;
                                insertLog(test, driver, result, true);
                            }
                            
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
            
        }

        public void fillContactDetails(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                //trigger messages
                driver.FindElement(By.CssSelector("#companyName")).SendKeys(Keys.Tab);
                driver.FindElement(By.CssSelector("#firstName")).SendKeys(Keys.Tab);
                driver.FindElement(By.CssSelector("#lastName")).SendKeys(Keys.Tab);
                driver.FindElement(By.CssSelector("#emailAddress")).SendKeys(Keys.Tab);
                driver.FindElement(By.CssSelector("#phoneNumber")).SendKeys(Keys.Tab);
                // check for warning messages
                if (driver.FindElements(By.CssSelector(Locator.deliveryCompanyNameWarning)).Count() <= 0 ||
                   (driver.FindElements(By.CssSelector(Locator.deliveryCompanyNameWarning)).Count() >0 && !driver.FindElement(By.CssSelector(Locator.deliveryCompanyNameWarning)).Displayed)) { result.log += " Delivery Page :  Company Name required warning is not displayed"; }
                if (driver.FindElements(By.CssSelector(Locator.deliveryFirstNameWarning)).Count() <= 0 ||
                   (driver.FindElements(By.CssSelector(Locator.deliveryFirstNameWarning)).Count() > 0 && !driver.FindElement(By.CssSelector(Locator.deliveryFirstNameWarning)).Displayed)) { result.log += "\nDelivery Page :  First Name required warning is not displayed"; }
                if (driver.FindElements(By.CssSelector(Locator.deliveryLastNameWarning)).Count() <= 0 ||
                   (driver.FindElements(By.CssSelector(Locator.deliveryLastNameWarning)).Count() > 0 && !driver.FindElement(By.CssSelector(Locator.deliveryLastNameWarning)).Displayed)) { result.log += "\nDelivery Page :  Last Name required warning is not displayed"; }
                if (driver.FindElements(By.CssSelector(Locator.deliveryEmailAddressWarning)).Count() <= 0 ||
                   (driver.FindElements(By.CssSelector(Locator.deliveryEmailAddressWarning)).Count() > 0 && !driver.FindElement(By.CssSelector(Locator.deliveryEmailAddressWarning)).Displayed)) { result.log += "\nDelivery Page :  Email Address required warning is not displayed"; }
                
                if(!String.IsNullOrEmpty(result.log))
                {
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }

                //Get fields
                IWebElement companyName = driver.FindElement(By.CssSelector("#companyName"));
                IWebElement firstName = driver.FindElement(By.CssSelector("#firstName"));
                IWebElement lastName = driver.FindElement(By.CssSelector("#lastName"));
                IWebElement email = driver.FindElement(By.CssSelector("#emailAddress"));
                IWebElement phoneNo = driver.FindElement(By.CssSelector("#phoneNumber"));

                // populate from file, if not available, with predetermined values
                if (!String.IsNullOrEmpty(row["Company Name"].ToString()))
                    companyName.SendKeys(row["Company Name"].ToString());
                else
                    companyName.SendKeys("ActionPack LTD");
                if (!string.IsNullOrEmpty(row["First Name"].ToString()))
                    firstName.SendKeys(row["First Name"].ToString());
                else
                    firstName.SendKeys("Liam");
                if (!string.IsNullOrEmpty(row["Last Name"].ToString()))
                    lastName.SendKeys(row["Last Name"].ToString());
                else
                    lastName.SendKeys("Neeson");
                if (!string.IsNullOrEmpty(row["Email Address"].ToString()))
                    email.SendKeys(row["Email Address"].ToString());
                else
                    email.SendKeys("lneeson@action.com");
                if (!string.IsNullOrEmpty(row["Phone Number"].ToString()))
                    phoneNo.SendKeys(row["Phone Number"].ToString());
                else
                    phoneNo.SendKeys("01234 567 891");

                //check if there is any warning
                if ((driver.FindElements(By.CssSelector(Locator.deliveryCompanyNameWarning)).Count() > 0 && driver.FindElement(By.CssSelector(Locator.deliveryCompanyNameWarning)).Displayed) ||
                    (driver.FindElements(By.CssSelector(Locator.deliveryFirstNameWarning)).Count() > 0 && driver.FindElement(By.CssSelector(Locator.deliveryFirstNameWarning)).Displayed) ||
                    (driver.FindElements(By.CssSelector(Locator.deliveryLastNameWarning)).Count() > 0 && driver.FindElement(By.CssSelector(Locator.deliveryLastNameWarning)).Displayed) ||
                    (driver.FindElements(By.CssSelector(Locator.deliveryEmailAddressWarning)).Count() > 0 && driver.FindElement(By.CssSelector(Locator.deliveryEmailAddressWarning)).Displayed))
                { 
                    result.log = "Delivery Page :  Delivery Contact Details - one of the details giving warning. Please check";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        private void fieldManuelUpdate(string updatePath, string updateVal, out string fieldUpdatedText)
        {
            IWebElement field = driver.FindElement(By.CssSelector(updatePath));
            string fieldText = field.GetAttribute("value");
            field.SendKeys(Keys.Control + "a");
            field.SendKeys(Keys.Delete);
            if (!String.IsNullOrEmpty(updateVal))
            {
                field.SendKeys(updateVal);
                fieldUpdatedText = updateVal;
            }
            else
            {
                if (updatePath.Contains("addressLine"))
                {
                    int val = extractValueFromText(fieldText);
                    int newVal;
                    if (val > 0)
                    {
                        newVal = val + 5;
                        fieldUpdatedText = fieldText.Replace(val.ToString(), newVal.ToString());
                        field.SendKeys(fieldUpdatedText);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(fieldText))
                            fieldUpdatedText = fieldText + " - Updated";
                        else
                            fieldUpdatedText = LoremIpsum(1, 3, 1, 1, 1, 20);
                        field.SendKeys(fieldUpdatedText);
                    }
                }
                else if (updatePath.Contains("town") || updatePath.Contains("county"))
                {
                    if (!String.IsNullOrEmpty(fieldText))
                        fieldUpdatedText = fieldText + " - Updated";
                    else
                        fieldUpdatedText = LoremIpsum(1, 3, 1, 1, 1, 20);
                    field.SendKeys(fieldUpdatedText);
                }
                else if (updatePath.Contains("postCode"))
                {
                    string regionIdentifier = String.Empty, updatedPostCode = String.Empty;

                    if (!String.IsNullOrEmpty(fieldText))
                    {
                        regionIdentifier = fieldText.Substring(0, 3);
                        fieldUpdatedText = RandomPostCode(regionIdentifier);
                    }
                    else
                        fieldUpdatedText = RandomPostCode();
                    field.SendKeys(fieldUpdatedText);
                }
                else
                    fieldUpdatedText = String.Empty;
            }
        }

        public void insertLogo(string imagePath)
        {
            checkResult result = new checkResult();
            try
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.logodrop)));
                IWebElement element = driver.FindElement(By.CssSelector(Locator.logodrop));
                var path = Assembly.GetCallingAssembly().CodeBase;
                var actualPath = path.Substring(0, path.LastIndexOf("bin"));
                var projectPath = new Uri(actualPath).LocalPath;
                CheckImageDisplayed(Path.Combine(projectPath, imagePath), element, result);
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void removeLogo()
        {
            checkResult result = new checkResult();
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.logodrop)));
                IWebElement element = driver.FindElement(By.CssSelector(Locator.logodrop));
                Actions actions = new Actions(driver);
                actions.MoveToElement(element).Perform();
                if (driver.FindElements(By.CssSelector(Locator.removeLogoBtn)).Count() > 0)
                {
                    string imageTitle = driver.FindElement(By.CssSelector(Locator.logoTitlePath)).Text;
                    driver.FindElement(By.CssSelector(Locator.removeLogoBtn)).ClickAction(driver);
                    string divText = driver.FindElement(By.CssSelector(Locator.logoAreaforTextSearch)).Text;
                    if (!String.IsNullOrEmpty(imageTitle) && divText.Contains(imageTitle))
                    {
                        result.log = "Logo image cannot be removed";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    else
                    {
                        result.log = "Logo image removed successfully";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                }
                else
                {
                    result.log = "Remove button cannot be found";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void logoOrientationTest(string logoAlignment, bool fullTest) // cam be improved a little bit with correct check of alignment. TODO!
        {
            checkResult result = new checkResult();
            try
            {
                if (driver.FindElements(By.CssSelector(Locator.logoOrientationButtons)).Count() > 0)
                {
                    IList<IWebElement> buttons = driver.FindElements(By.CssSelector(Locator.logoOrientationButtons));
                    bool failed = false;
                    if (!String.IsNullOrEmpty(logoAlignment))
                    {
                        if (logoAlignment.Equals("left", StringComparison.OrdinalIgnoreCase))
                        {
                            buttons[0].ClickAction(driver);
                            if (driver.FindElements(By.CssSelector(Locator.logoOrientationLeft)).Count() <= 0)
                                failed = true;
                        }
                        else if (logoAlignment.Equals("center", StringComparison.OrdinalIgnoreCase))
                        {
                            buttons[1].ClickAction(driver);
                            if (driver.FindElements(By.CssSelector(Locator.logoOrientationMiddle)).Count() <= 0)
                                failed = true;
                        }
                        else if (logoAlignment.Equals("right", StringComparison.OrdinalIgnoreCase))
                        {
                            buttons[1].ClickAction(driver);
                            if (driver.FindElements(By.CssSelector(Locator.logoOrientationRight)).Count() <= 0)
                                failed = true;
                        }
                    }
                    else
                    {
                        if (fullTest)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                buttons[i].ClickAction(driver);
                                Thread.Sleep(TimeSpan.FromSeconds(2));
                                if ((i == 0 && driver.FindElements(By.CssSelector(Locator.logoOrientationLeft)).Count() <= 0) ||
                                   (i == 1 && driver.FindElements(By.CssSelector(Locator.logoOrientationMiddle)).Count() <= 0) ||
                                   (i == 2 && driver.FindElements(By.CssSelector(Locator.logoOrientationRight)).Count() <= 0))
                                {
                                    result.log = "Logo Orientation is failed";
                                    result.logType = LogType.FAIL;
                                    insertLog(test, driver, result, true);
                                    failed = true;
                                }
                            }
                        }
                    }
                    if (!failed)
                    {
                        result.log = "Logo Orientation option(s) work successfully";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                }
                else
                {
                    throw new ArgumentException("Orientation buttons can't be located");
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void templateImageSelectionTest()
        {
            checkResult result = new checkResult();
            try
            {
                if (driver.FindElements(By.XPath(Locator.templateImageHover)).Count() > 0)
                {
                    IWebElement templateImage = driver.FindElement(By.CssSelector(Locator.templateImagePath));
                    IWebElement templateHover = driver.FindElement(By.XPath(Locator.templateImageHover));
                    string imageURL = templateImage.GetAttribute("src");
                    Actions actions = new Actions(driver);
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", templateImage);
                    actions.MoveToElement(templateImage).Perform();
                    templateHover.ClickAction(driver);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(Locator.templateList + ">div")));
                    IList<IWebElement> templateList = driver.FindElements(By.CssSelector(Locator.templateList + ">div"));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                    if (templateList.Count > 0)
                    {
                        // Select a different template
                        IWebElement selected = driver.FindElement(By.CssSelector(Locator.selectedTemplate));
                        string selectedTemplateName = selected.FindElement(By.CssSelector("span.templateName")).Text;
                        int i = new Random().Next(1, templateList.Count - 1);
                        while (templateList[i].FindElement(By.CssSelector("span.templateName")).Text == selectedTemplateName) i = new Random().Next(1, templateList.Count - 1);
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", templateList[i]);
                        string newTemplateName = templateList[i].FindElement(By.CssSelector("span.templateName")).Text;
                        templateList[i].FindElement(By.CssSelector("div>img")).ClickAction(driver); // For some reason, click action doesn't work from time to time. Below checks are for extra caution.
                        //Check whether the template is selected and gone back to template edit page
                        if (driver.FindElements(By.CssSelector(Locator.templateSelectorModal)).Count() > 0)
                        {
                            result.log = "Template cannot be selected. Template selection  modal is still open";
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, true);
                        }
                        else
                        {
                            result.log = "Template has been changed. Previous template : " + selectedTemplateName + ", new template : " + newTemplateName;
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                    }
                    else
                    {
                        result.log = "Template list is empty";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "An exception occured. Message :" + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
                Assert.IsTrue(false);
            }
        }
        public static string findAttribute(string attributeSearch, string realTextStyle)
        {
            int start = realTextStyle.IndexOf(attributeSearch) + attributeSearch.Length;
            int end = realTextStyle.IndexOf(";", start);
            if (end == -1) end = realTextStyle.Length;
            return realTextStyle.Substring(start, end - start);
        }
        public void headerMessageSizeFormattingTest(string sizeText)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement sizeBtn = null;
                string presetSize = String.Empty, selectedSize = String.Empty;
                int selectedSizeValue = 0;
                if (driver.FindElements(By.CssSelector(Locator.headerFormatSizeBtn)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).ClickAction(driver);
                }
                sizeBtn = driver.FindElement(By.CssSelector(Locator.headerFormatSizeBtn));
                presetSize = sizeBtn.Text;
                sizeBtn.ClickAction(driver);
                IList<IWebElement> sizeOptions = driver.FindElements(By.CssSelector(Locator.headerFormatSizes));
                if (sizeOptions.Count > 0)
                {
                    if (!String.IsNullOrEmpty(sizeText))
                    {
                        foreach (IWebElement sizeOption in sizeOptions)
                        {
                            int sizeVal = extractValueFromText(findAttribute("font-size: ", sizeOption.FindElement(By.CssSelector("a")).GetAttribute("style")));
                            if ((sizeText == "Small" && sizeVal == 12) || (sizeText == "Medium" && sizeVal == 16) || (sizeText == "Large" && sizeVal == 26))
                            {
                                if (!sizeOption.Displayed) sizeBtn.ClickAction(driver);
                                CheckSize(result, out sizeBtn, out selectedSize, out selectedSizeValue, sizeOption);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (IWebElement size in sizeOptions)
                        {
                            if (!size.Displayed) sizeBtn.ClickAction(driver);
                            CheckSize(result, out sizeBtn, out selectedSize, out selectedSizeValue, size);
                        }
                        // after trying all options, return back to medium text size for better visual display
                        if (!sizeOptions[1].Displayed) sizeBtn.ClickAction(driver);
                        sizeOptions[1].ClickAction(driver);
                    }
                }
                else
                {
                    result.log = "Header Text Formatting - Text size options can't be retrieved";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
            catch (Exception ex)
            {
                result.log = " Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        private void CheckSize(checkResult result, out IWebElement sizeBtn, out string selectedSize, out int selectedSizeValue, IWebElement size)
        {
            selectedSize = size.FindElement(By.CssSelector("a")).Text;
            selectedSizeValue = extractValueFromText(findAttribute("font-size: ", size.FindElement(By.CssSelector("a")).GetAttribute("style")));
            size.ClickAction(driver);
            sizeBtn = driver.FindElement(By.CssSelector(Locator.headerFormatSizeBtn));
            if (!sizeBtn.Text.Equals(selectedSize, StringComparison.OrdinalIgnoreCase))
            {
                result.log = "Size cannot be changed to " + selectedSize;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            else
            {
                IWebElement headerTextArea = driver.FindElement(By.CssSelector(Locator.headerMessageTextArea));
                string textstyle = headerTextArea.GetAttribute("style");
                string fontSizeText = findAttribute("font-size: ", textstyle);
                int fontSize = extractValueFromText(fontSizeText);
                if (fontSize == selectedSizeValue)
                {
                    result.log = "Header message font size is changed to " + selectedSize + " size value : " + fontSize + " px";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, false);
                }
                else
                {
                    result.log = "Size cannot be changed to " + selectedSize;
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
        }
        public void headerMessageBoldFormatTest(string boldText)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement boldBtn = null;
                bool boldSelected = false;
                string textAreaStyle, formerfontWeigth, latterfontWeight;
                textAreaStyle = formerfontWeigth = latterfontWeight = String.Empty;

                if (driver.FindElements(By.CssSelector(Locator.headerFormatBoldBtn)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).ClickAction(driver);
                }
                boldBtn = driver.FindElement(By.CssSelector(Locator.headerFormatBoldBtn));
                string boldClass = boldBtn.GetAttribute("class");
                string pattern = @"\bactive\b";
                Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
                Match m = rx.Match(boldClass);
                findWeight(Locator.headerMessageTextArea, out formerfontWeigth);
                boldSelected = checkBoldState(formerfontWeigth, m);
                if (!String.IsNullOrEmpty(boldText))
                {
                    if ((boldText == "YES" && boldSelected == false) || (boldText == "NO" && boldSelected == true))
                    {
                        boldBtn.ClickAction(driver);
                        boldClass = boldBtn.GetAttribute("class");
                        findWeight(Locator.headerMessageTextArea, out latterfontWeight);
                        m = rx.Match(boldClass);
                        if (checkBoldState(latterfontWeight, m))
                        {
                            result.log = "Header Text set as bold successfully";
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                        else
                        {
                            result.log = "Header Text set as normal successfully";
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                    }
                }
                else
                {
                    if (!boldSelected)
                    {
                        boldBtn.ClickAction(driver);
                        boldClass = boldBtn.GetAttribute("class");
                        findWeight(Locator.headerMessageTextArea, out latterfontWeight);
                        m = rx.Match(boldClass);
                        if (checkBoldState(latterfontWeight, m))
                        {
                            result.log = "Header Text set as bold successfully";
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                        //returning back to normal case for better visual
                        boldBtn.ClickAction(driver);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }

            bool checkBoldState(string weightString, Match m)
            {
                if (m.Success && !weightString.Equals("bold", StringComparison.OrdinalIgnoreCase))
                {
                    result.log = "Header text is not set as 'Bold' although selection is bold";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                    return true;
                }
                else if (!m.Success && weightString.Equals("bold", StringComparison.OrdinalIgnoreCase))
                {
                    result.log = "Header text is set as 'Bold' although button is not clicked";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                    return false;
                }
                else if (m.Success && weightString.Equals("bold", StringComparison.OrdinalIgnoreCase))
                {
                    result.log = "Header text is set as 'Bold' successfully";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, false);
                    return true;
                }
                else if (!m.Success && !weightString.Equals("bold", StringComparison.OrdinalIgnoreCase))
                {
                    result.log = "Header text is set as 'Normal' from bold successfully";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, false);
                    return false;
                }
                else
                    return false;
            }
        }
        public void findWeight(string textPath, out string fontWeight)
        {
            string textAreaStyle = driver.FindElement(By.CssSelector(textPath)).GetAttribute("style");
            int start = textAreaStyle.IndexOf("font-weight: ") + 13;
            int end = textAreaStyle.IndexOf(";", start);
            fontWeight = textAreaStyle.Substring(start, end - start);
        }

        public static bool isBold(string textStyle)
        {
            string fontWeight = String.Empty;
            int start = textStyle.IndexOf("font-weight: ") + 13;
            int end = textStyle.IndexOf(";", start);
            fontWeight = textStyle.Substring(start, end - start);
            if (fontWeight == "bold") return true;
            else
                return false;
        }
        public void headerMessageChangeBackgroundColourTest(string colour)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement backGroundColourBtn = null, textColourBtn = null;
                string presetColour, selectedColour, presetTextColour;
                presetColour = selectedColour = presetTextColour = String.Empty;
                if (driver.FindElements(By.CssSelector(Locator.headerFormatBackgroundClrBtn)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).ClickAction(driver);
                }
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                backGroundColourBtn = driver.FindElement(By.CssSelector(Locator.headerFormatBackgroundClrBtn));
                string backgroundColourStyle = backGroundColourBtn.FindElement(By.CssSelector("div")).GetAttribute("style");
                presetColour = defineColour(backgroundColourStyle);

                textColourBtn = driver.FindElement(By.CssSelector(Locator.headerFormatColourBtn));
                string textcolourStyle = textColourBtn.FindElement(By.CssSelector("span:nth-child(1)")).GetAttribute("style");
                presetTextColour = defineColour(textcolourStyle);

                backGroundColourBtn.ClickAction(driver);
                Thread.Sleep(TimeSpan.FromSeconds(2));
                IList<IWebElement> backgroundList = driver.FindElements(By.CssSelector(Locator.headerFormatBgrColourList));
                if (backgroundList.Count > 0)
                {
                    if (!String.IsNullOrEmpty(colour))
                    {
                        colour = colour.Replace(" ", "");
                        foreach (IWebElement element in backgroundList)
                        {
                            if (defineColour(element.FindElement(By.CssSelector("a>div")).GetAttribute("style")).Equals(colour, StringComparison.OrdinalIgnoreCase))
                            {
                                element.ClickAction(driver);
                                selectedColour = defineColour(element.FindElement(By.CssSelector("a>div")).GetAttribute("style"));
                                break;
                            }
                        }
                    }
                    else
                    {
                        int colourIndex = new Random().Next(1, backgroundList.Count - 1);
                        backgroundColourStyle = backgroundList[colourIndex].FindElement(By.CssSelector("a>div")).GetAttribute("style");
                        selectedColour = defineColour(backgroundColourStyle);
                        if (presetColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase) || presetTextColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase))
                        {
                            if (colourIndex != backgroundList.Count - 1) colourIndex++;
                            else colourIndex--;
                            backgroundColourStyle = backgroundList[colourIndex].FindElement(By.CssSelector("a>div")).GetAttribute("style");
                            selectedColour = defineColour(backgroundColourStyle);
                        }
                        backgroundList[colourIndex].ClickAction(driver);
                    }
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    IWebElement textArea = driver.FindElement(By.CssSelector(Locator.headerMessageTextArea));
                    IWebElement valueArea = driver.FindElement(By.CssSelector(Locator.headerValueArea));
                    string realTextStyle = textArea.GetAttribute("style");
                    string valueAreaStyle = valueArea.GetAttribute("style");
                    string realColour = findAttribute("background-color: ", realTextStyle).Replace(" ", "");
                    string valueColour = findAttribute("background-color: ", valueAreaStyle).Replace(" ", "");
                    if (!realColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase) || !valueColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase))
                    {
                        result.log = "Header message background text colour can't be changed";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    else
                    {
                        result.log = "Header message background text colour has successfully been changed";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                    if (String.IsNullOrEmpty(colour))
                    {
                        // setting a white colour background for visual clarity
                        backGroundColourBtn.ClickAction(driver);
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        backgroundList = driver.FindElements(By.CssSelector(Locator.headerFormatBgrColourList));
                        if (backgroundList.Count > 0)
                        {
                            string whiteColour = "rgb(255, 255, 255)";
                            for (int i = 0; i < backgroundList.Count; i++)
                            {
                                backgroundColourStyle = backgroundList[i].FindElement(By.CssSelector("a>div")).GetAttribute("style");
                                selectedColour = defineColour(backgroundColourStyle);
                                if (selectedColour.Equals(whiteColour, StringComparison.Ordinal))
                                {
                                    backgroundList[i].ClickAction(driver);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    result.log = "Background colour list can't be retrieved for header message";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void bottomMessageToolTipCapture()
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement bottomSection = driver.FindElement(By.CssSelector(Locator.bottomMessageSection));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", bottomSection);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-300)"); //adjusting view
                if (driver.FindElements(By.CssSelector(Locator.bottomMessageTooltipIcon)).Count() > 0)
                {
                    IWebElement tooltipIcon = driver.FindElement(By.CssSelector(Locator.bottomMessageTooltipIcon));
                    Actions actions = new Actions(driver);
                    actions.MoveToElement(tooltipIcon).Perform();
                    result.log = "Bottom message tooltip is displayed. Tooltip message: " + tooltipIcon.GetAttribute("title");
                    result.logType = LogType.INFO;
                    insertLog(test, driver, result, false);
                }
                else
                {
                    result.log = " Bottom message tooltip icon couldn't be located ";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void textInputTest(string sectionName, string message, string messageSectionPath, string pulsatingIconPath, string messageTextPath, string messageCharWarningPath = null, int charLimitVal = 0, string messageHeaderAreaPath = null)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement messageArea = null, messageTextArea = null, pulsatingIcon = null, messageHeaderArea = null;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                if (driver.FindElements(By.CssSelector(messageSectionPath)).Count() > 0)
                {
                    messageArea = driver.FindElement(By.CssSelector(messageSectionPath));
                    if (!String.IsNullOrEmpty(messageHeaderAreaPath)) messageHeaderArea = driver.FindElement(By.CssSelector(messageHeaderAreaPath));
                    pulsatingIcon = driver.FindElement(By.CssSelector(pulsatingIconPath));
                }
                else
                {
                    result.log = sectionName + " is not visible";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", pulsatingIcon);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-150)"); //adjusting view
                messageTextArea = driver.FindElement(By.CssSelector(messageTextPath));
                //check if text formatting bar visible
                if (messageTextArea.GetAttribute("class").Contains("untouched") && messageArea.FindElements(By.CssSelector("div.customiseBar")).Count() > 0)
                {
                    if (messageArea.FindElement(By.CssSelector("div.customiseBar")).Displayed)
                    {
                        result.log = "Formatting bar is enabled prior to activating text area for " + sectionName;
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(messageCharWarningPath))
                    {
                        //check if character remaining text is visible
                        if (driver.FindElements(By.CssSelector(messageCharWarningPath)).Count() > 0)
                        {
                            result.log = "Remaining char warning is visible without activating text area for " + sectionName;
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, true);
                        }
                        messageTextArea.ClickAction(driver);
                        if (driver.FindElements(By.CssSelector(messageCharWarningPath)).Count() > 0)
                        {
                            result.log = sectionName + " remaining char text is displayed properly";
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                        else
                        {
                            result.log = sectionName + " remaining char text is not displayed properly. Please check!";
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, true);
                        }
                        if (!String.IsNullOrEmpty(message))
                            messageTextArea.SendKeys(message);
                        else
                        {
                            messageTextArea.SendKeys(loremIpsumLongText);
                        }
                        string charWarningText = driver.FindElement(By.CssSelector(messageCharWarningPath)).Text;
                        if (charLimitVal > 0)
                        {
                            if (messageTextArea.GetAttribute("value").Length > charLimitVal)
                            {
                                result.log = sectionName + " text limit is not restricted to " + charLimitVal.ToString() + " characters as indicated.";
                                result.logType = LogType.FAIL;
                                insertLog(test, driver, result, true);
                            }
                            else if (messageTextArea.GetAttribute("value").Length <= message.Length)
                            {
                                result.log = sectionName + "  text is cut from the " + charLimitVal.ToString() + " chars";
                                result.logType = LogType.INFO;
                                insertLog(test, driver, result, false);
                            }
                        }
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(message))
                        {
                            messageTextArea.SendKeys(message);
                        }
                        else
                        {
                            messageTextArea.SendKeys(loremIpsumLongText);
                            messageTextArea.Clear();
                            messageTextArea.SendKeys(loremIpsumValidText);
                            if (messageTextArea.GetAttribute("value") == loremIpsumValidText)
                            {
                                result.log = sectionName + " text is populted correctly.";
                                result.logType = LogType.SUCCESS;
                                insertLog(test, driver, result, false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception caught. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void textFontFormattingTest(string sectionName, string font, string fontBtnPath, string fontListPath, string messageTextAreaPath, string secondaryAreaPath)
        {
            checkResult result = new checkResult();
            try
            {
                if (driver.FindElements(By.CssSelector(fontBtnPath)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(messageTextAreaPath)).ClickAction(driver);
                }
                IWebElement fontBtn = driver.FindElement(By.CssSelector(fontBtnPath));
                string presetFont, selectedFont, presetFontText, selectedFontText;
                presetFont = selectedFont = presetFontText = selectedFontText = String.Empty;
                presetFont = fontBtn.GetAttribute("style");
                presetFontText = FindFont(presetFont);
                fontBtn.ClickAction(driver);
                IList<IWebElement> fontList = driver.FindElements(By.CssSelector(fontListPath));
                if (fontList.Count > 0)
                {
                    if (!String.IsNullOrEmpty(font))
                    {
                        foreach (IWebElement fontElement in fontList)
                        {
                            if (FindFont(fontElement.FindElement(By.CssSelector("a")).GetAttribute("style")).Equals(font, StringComparison.Ordinal))
                            {
                                fontElement.ClickAction(driver);
                                selectedFontText = FindFont(fontElement.FindElement(By.CssSelector("a")).GetAttribute("style"));
                                break;
                            }
                        }
                    }
                    else
                    {
                        int randomFont = new Random().Next(0, fontList.Count - 1);
                        while (fontList[randomFont].FindElement(By.CssSelector("a")).GetAttribute("style").Equals(presetFont, StringComparison.Ordinal))
                        {
                            randomFont = new Random().Next(0, fontList.Count - 1);
                        }
                        selectedFont = fontList[randomFont].FindElement(By.CssSelector("a")).GetAttribute("style");
                        selectedFontText = FindFont(selectedFont);
                        fontList[randomFont].ClickAction(driver);
                    }
                    //recheck selected font
                    fontBtn = driver.FindElement(By.CssSelector(fontBtnPath));
                    selectedFont = fontBtn.GetAttribute("style");
                    selectedFontText = FindFont(selectedFont);
                    if (presetFontText.Equals(selectedFontText, StringComparison.OrdinalIgnoreCase) && !presetFontText.Equals(font, StringComparison.OrdinalIgnoreCase))
                    {
                        result.log = sectionName + " font type couln't be changed successfully.";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                    else
                    {
                        IWebElement textArea = driver.FindElement(By.CssSelector(messageTextAreaPath));
                        IWebElement secondaryArea = null;
                        string secondaryAreaStyle = null, realFontSecondaryArea = null;
                        if (!String.IsNullOrEmpty(secondaryAreaPath))
                        {

                            secondaryArea = driver.FindElement(By.CssSelector(secondaryAreaPath));
                            secondaryAreaStyle = secondaryArea.GetAttribute("style");
                            realFontSecondaryArea = findAttribute("font-family: ", secondaryAreaStyle).Replace("\"", "");
                        }
                        string realTextStyle = textArea.GetAttribute("style");
                        string realFont = findAttribute("font-family: ", realTextStyle).Replace("\"", "");
                        if (realFont.Equals(selectedFontText, StringComparison.OrdinalIgnoreCase) && ((!String.IsNullOrEmpty(secondaryAreaPath) && realFontSecondaryArea.Equals(selectedFontText, StringComparison.OrdinalIgnoreCase)) || String.IsNullOrEmpty(secondaryAreaPath)))
                        {
                            result.log = sectionName + " font type is changed successfully. Previous font : " + presetFontText + " - new font : " + selectedFontText;
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                        else
                        {
                            result.log = sectionName + " font type couln't be changed successfully.";
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, false);
                        }
                    }
                }
                else
                {
                    result.log = "Fonts can't be loaded in " + sectionName + " menu.";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
            catch (Exception ex)
            {
                result.log = "An exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void textChangeTextColourTest(string sectionName, string textColour, string colourBtnPath, string pulsatingIconPath, string messageTextAreaPath, string colourListPath, string secondaryAreaPath)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement textColourBtn = null, pulsatingIcon = null;
                string presetColour, selectedColour;
                presetColour = selectedColour = String.Empty;

                pulsatingIcon = driver.FindElement(By.CssSelector(pulsatingIconPath));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", pulsatingIcon);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-150)"); //adjusting view

                if (driver.FindElements(By.CssSelector(colourBtnPath)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(messageTextAreaPath)).ClickAction(driver);
                }

                textColourBtn = driver.FindElement(By.CssSelector(colourBtnPath));
                string colourStyle = textColourBtn.FindElement(By.CssSelector("span:nth-child(1)")).GetAttribute("style");
                presetColour = defineColour(colourStyle);
                textColourBtn.ClickAction(driver);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                IList<IWebElement> colourList = driver.FindElements(By.CssSelector(colourListPath));
                if (colourList.Count > 0)
                {
                    if (!String.IsNullOrEmpty(textColour))
                    {
                        string elementColour = String.Empty;
                        textColour = textColour.Replace(" ", "");
                        foreach (IWebElement colourElement in colourList)
                        {
                            elementColour = defineColour(colourElement.FindElement(By.CssSelector("a>div")).GetAttribute("style"));
                            if (elementColour.Equals(textColour, StringComparison.OrdinalIgnoreCase))
                            {
                                colourElement.ClickAction(driver);
                                selectedColour = elementColour;
                                break;
                            }
                        }
                    }
                    else
                    {
                        int colourIndex = new Random().Next(1, colourList.Count - 1);
                        colourStyle = colourList[colourIndex].FindElement(By.CssSelector("a>div")).GetAttribute("style");
                        selectedColour = defineColour(colourStyle);
                        if (presetColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase))
                        {
                            if (colourIndex != colourList.Count - 1) colourIndex++;
                            else colourIndex--;
                            colourStyle = colourList[colourIndex].FindElement(By.CssSelector("a>div")).GetAttribute("style");
                            selectedColour = defineColour(colourStyle);
                        }
                        colourList[colourIndex].ClickAction(driver);
                    }
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    IWebElement textArea = driver.FindElement(By.CssSelector(messageTextAreaPath));
                    IWebElement secondaryArea = null;
                    string secondaryAreaStyle = null, secondaryAreaColour = null;
                    if (!String.IsNullOrEmpty(secondaryAreaPath))
                    {
                        secondaryArea = driver.FindElement(By.CssSelector(secondaryAreaPath));
                        if (sectionName.Equals("Bottom Message", StringComparison.OrdinalIgnoreCase))
                        {
                            secondaryArea = (IWebElement)(((IJavaScriptExecutor)driver).ExecuteScript(
                                   "return arguments[0].parentNode;", secondaryArea));
                        }
                        secondaryAreaStyle = secondaryArea.GetAttribute("style");
                        secondaryAreaColour = findAttribute("color: ", secondaryAreaStyle).Replace(" ", "");
                    }
                    string realTextStyle = textArea.GetAttribute("style");
                    string realTextColour = findAttribute("color: ", realTextStyle).Replace(" ", "");
                    if (realTextColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase) && ((!String.IsNullOrEmpty(secondaryAreaPath) && secondaryAreaColour.Equals(selectedColour, StringComparison.OrdinalIgnoreCase)) || String.IsNullOrEmpty(secondaryAreaPath)))
                    {
                        result.log = sectionName + " text colour has successfully been changed";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                    else
                    {
                        result.log = sectionName + " text colour can't be changed";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }
                else
                {
                    result.log = sectionName + " text colours can't be retrieved.";
                    result.logType = LogType.FATAL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void textAlignmentTest(string sectionName, string alignment, string alignmentBtnPath, string pulsatingIconPath, string messageTextAreaPath, string secondaryAreaPath)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement pulsatingIcon = driver.FindElement(By.CssSelector(pulsatingIconPath));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", pulsatingIcon);
                ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-150)"); //adjusting view
                if (driver.FindElements(By.CssSelector(alignmentBtnPath)).Count() <= 0)
                {
                    driver.FindElement(By.CssSelector(messageTextAreaPath)).ClickAction(driver);
                }
                IList<IWebElement> alignmentButtons = driver.FindElements(By.CssSelector(alignmentBtnPath));
                if (alignmentButtons.Count > 0)
                {
                    if (!String.IsNullOrEmpty(alignment))
                    {
                        foreach (IWebElement alignmentElement in alignmentButtons)
                        {
                            if (findAttribute("fa-align-", alignmentElement.FindElement(By.CssSelector("i")).GetAttribute("class")).Equals(alignment, StringComparison.OrdinalIgnoreCase))
                            {
                                alignmentElement.ClickAction(driver);
                                AlignmentCheck(sectionName, messageTextAreaPath, secondaryAreaPath, result, alignmentElement);
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (IWebElement button in alignmentButtons)
                        {
                            button.ClickAction(driver);
                            AlignmentCheck(sectionName, messageTextAreaPath, secondaryAreaPath, result, button);
                        }
                    }
                }
                else
                {
                    result.log = sectionName + " alignment buttons list can't be retrieved";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        private void AlignmentCheck(string sectionName, string messageTextAreaPath, string secondaryAreaPath, checkResult result, IWebElement button)
        {
            IWebElement textArea = null, secondaryArea = null;
            string realTextStyle = null, secondaryAreaStyle = null, textAlignment = null, secondaryTextAlignment = null, buttonClass = null, buttonAlignment = null;
            textArea = driver.FindElement(By.CssSelector(messageTextAreaPath));
            if (!String.IsNullOrEmpty(secondaryAreaPath))
            {
                secondaryArea = driver.FindElement(By.CssSelector(secondaryAreaPath));
                if (sectionName.Equals("Bottom Message", StringComparison.OrdinalIgnoreCase))
                {
                    secondaryArea = (IWebElement)(((IJavaScriptExecutor)driver).ExecuteScript(
                    "return arguments[0].parentNode;", secondaryArea));
                }
                secondaryAreaStyle = secondaryArea.GetAttribute("style");
                secondaryTextAlignment = findAttribute("text-align: ", secondaryAreaStyle);
            }
            realTextStyle = textArea.GetAttribute("style");
            textAlignment = findAttribute("text-align: ", realTextStyle);
            buttonClass = button.FindElement(By.CssSelector("i")).GetAttribute("class");
            buttonAlignment = findAttribute("fa-align-", buttonClass);
            if (buttonAlignment.Equals(textAlignment, StringComparison.OrdinalIgnoreCase) && ((!String.IsNullOrEmpty(secondaryAreaPath) && buttonAlignment.Equals(secondaryTextAlignment, StringComparison.OrdinalIgnoreCase)) || String.IsNullOrEmpty(secondaryAreaPath)))
            {
                result.log = sectionName + " " + buttonAlignment + " alignment was successful";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = sectionName + " " + buttonAlignment + " alignment failed";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }
        public void nameAreaInputTest(string name)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement nameInputArea = null, pulsatingIcon = null, nameSection = null;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                if (driver.FindElements(By.CssSelector(Locator.nameAreaTextInput)).Count() > 0)
                {
                    nameInputArea = driver.FindElement(By.CssSelector(Locator.nameAreaTextInput));
                    pulsatingIcon = driver.FindElement(By.CssSelector(Locator.nameAreaPulsatingIcon));
                    nameSection = driver.FindElement(By.CssSelector(Locator.nameSection));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", pulsatingIcon);
                    ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0,-150)"); //adjusting view
                }
                else
                {
                    result.log = " Name input section is not visible";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
                if (nameInputArea.GetAttribute("class").Contains("untouched") && nameSection.FindElements(By.CssSelector("div.customiseBar")).Count() > 0)
                {
                    if (nameSection.FindElement(By.CssSelector("div.customiseBar")).Displayed)
                    {
                        result.log = "Formatting bar is enabled prior to activating text area for name inout section";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }
                else
                {
                    nameInputArea.SendKeys(name);
                    if (nameInputArea.GetAttribute("value").Equals(name, StringComparison.Ordinal))
                    {
                        result.log = "Name input text is populated correctly.";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void continueRecipientsPage()
        {
            checkResult result = new checkResult();
            try
            {
                if (driver.FindElements(By.CssSelector(Locator.templateContinueBtn)).Count() > 0)
                {
                    IWebElement continueBtn = driver.FindElement(By.CssSelector(Locator.templateContinueBtn));
                    if (continueBtn.Enabled) continueBtn.ClickAction(driver);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                    var element = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.recipientsPageIndicator)));
                    if (element.Displayed)
                    {
                        result.log = " Recipients page is opened successfully";
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void backtoTemplateandCheck(bool checkTemplateStructure, PromotionalTemplate template)
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement backToTemplateBtn = driver.FindElement(By.CssSelector(Locator.CustomiseTemplateBtn));
                if (backToTemplateBtn.Enabled && backToTemplateBtn.Displayed)
                {
                    backToTemplateBtn.ClickAction(driver);
                    // confirm that customise template page is reopen
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locator.TemplatePageIndicator)));
                    if (driver.FindElements(By.CssSelector(Locator.templateContainer)).Count() > 0)
                    {
                        if (checkTemplateStructure)
                        {
                            //preliminary preparation
                            string headerTextAreaStyle, headerValueAreaStyle, bottomTextAreaStyle, bottomHeaderStyle, nameAreaStyle;
                            headerTextAreaStyle = driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).GetAttribute("style");
                            headerValueAreaStyle = driver.FindElement(By.CssSelector(Locator.headerValueArea)).GetAttribute("style");
                            bottomTextAreaStyle = driver.FindElement(By.CssSelector(Locator.bottomMessageText)).GetAttribute("style");
                            IWebElement secondaryArea = (IWebElement)(((IJavaScriptExecutor)driver).ExecuteScript(
                            "return arguments[0].parentNode;", driver.FindElement(By.CssSelector(Locator.bottomMessageSection))));
                            bottomHeaderStyle = secondaryArea.GetAttribute("style");
                            nameAreaStyle = driver.FindElement(By.CssSelector(Locator.nameAreaTextInput)).GetAttribute("style");

                            //checking the logo
                            LogoCheck(template.logoTitle, result);
                            //check Logo orientation
                            LogoOrientationCheck(template.logoOrientation, result);
                            // check header message text
                            textInputCheck("Header Message", template.headerText, Locator.headerMessageTextArea, 150, result);
                            // check header Font
                            textFontCheck("Header Message", template.headerFont, headerTextAreaStyle, headerValueAreaStyle, result);
                            // check header size
                            headerSizeCheck(template.headerSize, result);
                            // check Bold State
                            headerBoldCheck(template.headerBoldState, result);
                            // check header font colour
                            textFontColourCheck("Header Message", template.headerFontColour, headerTextAreaStyle, headerValueAreaStyle, result);
                            // check header  background colour
                            headerBackgroundColourCheck(template.headerBackGroundColour, result, headerTextAreaStyle, headerValueAreaStyle);
                            // check header message orientation
                            textAlignmentCheck("Header Message", template.headerOrientation, headerTextAreaStyle, headerValueAreaStyle, result);
                            // check bottom message text
                            textInputCheck("Bottom Message", template.bottomText, Locator.bottomMessageText, 500, result);
                            // check bottom message font
                            textFontCheck("Bottom Message", template.bottomFont, bottomTextAreaStyle, bottomHeaderStyle, result);
                            // check bottom message text colour
                            textFontColourCheck("Bottom Message", template.bottomFontColour, bottomTextAreaStyle, bottomHeaderStyle, result);
                            // check botom text alignment
                            textAlignmentCheck("Bottom Message", template.bottomTextOrientation, bottomTextAreaStyle, bottomHeaderStyle, result);
                            // check name area text
                            textInputCheck("Name Area", template.nameText, Locator.nameAreaTextInput, 0, result);
                            // check name area font
                            textFontCheck("Name Area", template.nameFont, nameAreaStyle, null, result);
                            // check name area text colour
                            textFontColourCheck("Name Area", template.nameFontColour, nameAreaStyle, null, result);
                            //check name area text alignment
                            textAlignmentCheck("Name Area", template.nameOrientation, nameAreaStyle, null, result);
                            //----------------------------
                            // Final note regarding the template page and returning back to recipients.
                            result.log = "Template page is opened successfully";
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                        driver.FindElement(By.CssSelector(Locator.templateContinueBtn)).ClickAction(driver);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occured. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        private void LogoOrientationCheck(Orientation logoOrient, checkResult result)
        {
            if ((logoOrient == Orientation.LEFT && driver.FindElements(By.CssSelector(Locator.logoOrientationLeft)).Count() <= 0) ||
               (logoOrient == Orientation.CENTER && driver.FindElements(By.CssSelector(Locator.logoOrientationMiddle)).Count() <= 0) ||
               (logoOrient == Orientation.RIGHT && driver.FindElements(By.CssSelector(Locator.logoOrientationRight)).Count() <= 0))
            {
                result.log = "Logo Orientation is not preserved. Orientation should be " + logoOrient + " aligned!";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
            else
            {
                result.log = "Logo Orientation is preserved successfully.";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
        }
        private void LogoCheck(string imageTitle, checkResult result)
        {
            IWebElement logo = driver.FindElement(By.CssSelector(Locator.logoTitleAlternative));
            string val = logo.GetAttribute("title");
            int length = imageTitle.Length;
            int index = imageTitle.LastIndexOf("\\") + 1;
            imageTitle = imageTitle.Substring(index, length - index);
            if (imageTitle.Equals(val, StringComparison.OrdinalIgnoreCase))
            {
                result.log = "Logo is preserved";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = "Logo has been changed during returning back to template customization";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
        }
        private void textAlignmentCheck(string sectionName, Orientation textAlignment, string textAreaStyle, string secondaryAreaStyle, checkResult result)
        {
            string textAlignmentString = String.Empty;
            string textAreaAlignment = findAttribute("text-align: ", textAreaStyle);
            string secondaryAreaAlignment = String.Empty;
            if (!String.IsNullOrEmpty(secondaryAreaStyle)) secondaryAreaAlignment = findAttribute("text-align: ", secondaryAreaStyle);
            if (textAlignment == Orientation.LEFT) textAlignmentString = "left";
            else if (textAlignment == Orientation.CENTER) textAlignmentString = "center";
            else if (textAlignment == Orientation.RIGHT) textAlignmentString = "right";
            if (textAreaAlignment.Equals(textAlignmentString, StringComparison.OrdinalIgnoreCase) && (!String.IsNullOrEmpty(secondaryAreaAlignment) && secondaryAreaAlignment.Equals(textAlignmentString, StringComparison.OrdinalIgnoreCase)) || String.IsNullOrEmpty(secondaryAreaAlignment))
            {
                result.log = sectionName + " text alignment has been preserved successfully";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = sectionName + " text alignment couldn't be preserved. Expected : " + textAlignment + " Actual : " + textAreaAlignment;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }
        private void headerBackgroundColourCheck(string headerBackColour, checkResult result, string headerTextAreaStyle, string headerValueAreaStyle)
        {
            string textAreaBGColour = findAttribute("background-color: ", headerTextAreaStyle).Replace(" ", "");
            string valueAreaBGColour = findAttribute("background-color: ", headerValueAreaStyle).Replace(" ", "");
            if (!textAreaBGColour.Equals(headerBackColour, StringComparison.OrdinalIgnoreCase) || !valueAreaBGColour.Equals(headerBackColour, StringComparison.OrdinalIgnoreCase))
            {
                result.log = "Header message background text colour couldn't be preserved";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
            else
            {
                result.log = "Header message background text colour has successfully been preserved";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
        }
        private void textFontColourCheck(string sectionName, string rowTextColour, string textAreaStyle, string secondaryAreaStyle, checkResult result)
        {
            rowTextColour = rowTextColour.Replace(" ", "");
            string secondaryAreaColour = String.Empty;
            string textAreaColour = findAttribute("color: ", textAreaStyle).Replace(" ", "");
            if (!String.IsNullOrEmpty(secondaryAreaStyle)) secondaryAreaColour = findAttribute("color: ", secondaryAreaStyle).Replace(" ", "");
            if (textAreaColour.Equals(rowTextColour, StringComparison.OrdinalIgnoreCase) && (!String.IsNullOrEmpty(secondaryAreaColour) && secondaryAreaColour.Equals(rowTextColour, StringComparison.OrdinalIgnoreCase)) || String.IsNullOrEmpty(secondaryAreaColour))
            {
                result.log = sectionName + " text colour has successfully been preserved";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = sectionName + " text colour couldn't be preserved. Expected : " + rowTextColour + " Actual : " + textAreaColour;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }
        private void headerBoldCheck(bool headerBoldState, checkResult result)
        {
            string fontWeight = String.Empty;
            findWeight(Locator.headerMessageTextArea, out fontWeight);
            if ((fontWeight == "bold" && headerBoldState) || (fontWeight != "bold" && !headerBoldState))
            {
                result.log = "Header bold status is preserved successfully";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = "Header bold status is not preserved. Expected : " + headerBoldState + " Actual : " + fontWeight;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }
        private void headerSizeCheck(int size, checkResult result)
        {
            int actualHeaderTextSize = extractValueFromText(findAttribute("font-size: ", driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).GetAttribute("style")));
            if (size == actualHeaderTextSize)
            {
                result.log = "Header font size is preserved succesfully";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = "Header font size is not preserved. Expected size: " + size + " Actual Size: " + actualHeaderTextSize;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }
        private void textFontCheck(string sectionName, string textFont, string textAreaStyle, string secondaryAreaStyle, checkResult result)
        {
            string actualTextFont = findAttribute("font-family: ", textAreaStyle).Replace("\"", "");
            string actualSecondaryAreaFont = String.Empty;
            if (!String.IsNullOrEmpty(secondaryAreaStyle)) actualSecondaryAreaFont = findAttribute("font-family: ", secondaryAreaStyle).Replace("\"", "");
            if (actualTextFont.Equals(textFont, StringComparison.OrdinalIgnoreCase) && (!String.IsNullOrEmpty(actualSecondaryAreaFont) && actualSecondaryAreaFont.Equals(textFont, StringComparison.OrdinalIgnoreCase)) || String.IsNullOrEmpty(actualSecondaryAreaFont))
            {
                result.log = sectionName + " font type is preserved successfully.";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = sectionName + " font type is not preserved successfully. Expected Font: " + textFont + " Actual Font: " + actualTextFont;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
        }
        private void textInputCheck(string sectionName, string inputText, string actualMessagePath, int textLengthLimit, checkResult result)
        {
            string actualMessageText = driver.FindElement(By.CssSelector(actualMessagePath)).GetAttribute("value");
            if (textLengthLimit != 0 && inputText.Length > textLengthLimit) inputText = inputText.Substring(0, inputText.Length - 1);
            if (inputText.Equals(actualMessageText, StringComparison.OrdinalIgnoreCase))
            {
                result.log = sectionName + " text is preserved successfully";
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
            }
            else
            {
                result.log = sectionName + " text is not preserved successfully. Expected message : " + inputText + " - Actual Message : " + actualMessageText;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }

        public void downloadSampleTest(string filename)
        {
            checkResult result = new checkResult();
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(Locator.downloadSampleBtn)));
                driver.FindElement(By.CssSelector(Locator.downloadSampleBtn)).ClickAction(driver);
                // check if the popup display opens
                if (driver.FindElements(By.CssSelector(Locator.downloadSampleModal)).Count() > 0)
                {
                    // first close the dialogue to check if it can be repoened again
                    driver.FindElement(By.CssSelector(Locator.downloadSampleModalCancelBtn)).ClickAction(driver);
                    // reopen modal
                    driver.FindElement(By.CssSelector(Locator.downloadSampleBtn)).ClickAction(driver);
                    // download this time
                    driver.FindElement(By.CssSelector(Locator.downloadSampleModalDownloadBtn)).ClickAction(driver);
                    Task.Delay(TimeSpan.FromSeconds(2)).Wait(); // waiting some time till the download is completed
                    string Path = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads"; //expected download location
                    string[] filePaths = Directory.GetFiles(Path);
                    bool found = false;
                    foreach (string p in filePaths)
                    {
                        if (p.Contains(filename))
                        {
                            FileInfo thisFile = new FileInfo(p);
                            //Check the file that are downloaded in the last 3 minutes
                            if (thisFile.LastWriteTime.ToShortTimeString() == DateTime.Now.ToShortTimeString() ||
                            thisFile.LastWriteTime.AddMinutes(1).ToShortTimeString() == DateTime.Now.ToShortTimeString() ||
                            thisFile.LastWriteTime.AddMinutes(2).ToShortTimeString() == DateTime.Now.ToShortTimeString() ||
                            thisFile.LastWriteTime.AddMinutes(3).ToShortTimeString() == DateTime.Now.ToShortTimeString())
                            {
                                result.log = "Sample file is downloaded successfully";
                                result.logType = LogType.SUCCESS;
                                insertLog(test, driver, result, false);
                                found = true;
                                File.Delete(p);
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        result.log = "Sample file cannot be located after download so can't be verified";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                }
                else
                {
                    result.log = "Download sample modal is not opened";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void downloadSampleInfoIconTest()
        {
            checkResult result = new checkResult();
            try
            {
                IWebElement info = driver.FindElement(By.CssSelector(Locator.downloadSampleInfoIcon));
                Actions actions = new Actions(driver);
                actions.MoveToElement(info).Perform();
                // check if it is displayed
                if (driver.FindElements(By.CssSelector(Locator.downloadSampleTooltip)).Count() > 0)
                {
                    result.log = "Tooltip is displayed";
                    result.logType = LogType.SUCCESS;
                }
                else
                {
                    result.log = "Donwload tooltip is not displayed properly.";
                    result.logType = LogType.FAIL;
                }
                string tooltipText = info.GetAttribute("title");

                if (!String.IsNullOrEmpty(tooltipText)) result.log += " - Tooltip text : " + tooltipText;
                insertLog(test, driver, result, false);
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void importSpreadsheetTest(string fileDirectory, string validfileName, string faultyFileName)
        {
            checkResult result = new checkResult();
            try
            {
                checkFileUpload(fileDirectory, validfileName, result);
                checkFileUpload(fileDirectory, faultyFileName, result);
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        private void checkFileUpload(string fileDirectory, string fileName, checkResult result)
        {
            if (IsExist(fileDirectory, fileName))
            {
                IWebElement importList = driver.FindElement(By.CssSelector(Locator.importSpreadsheetBtn));
                string fullPath = Path.Combine(fileDirectory, fileName);
                importList.SendKeys(fullPath);
                //check if it's imported successfully
                if (driver.FindElements(By.CssSelector(Locator.importErrorBanner)).Count() > 0)
                {
                    string errorText = driver.FindElement(By.CssSelector(Locator.importBannerText)).Text;
                    result.log = " Error received during valid file upload. Uploaded file : " + fileName + " - error text : " + errorText;
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
                else if (driver.FindElements(By.CssSelector(Locator.importSuccessBanner)).Count() > 0)
                {
                    string successText = driver.FindElement(By.CssSelector(Locator.importBannerText)).Text;
                    if (driver.FindElements(By.CssSelector(Locator.recipientsTable)).Count() > 0)
                    {
                        IList<IWebElement> recipientList = driver.FindElements(By.CssSelector(Locator.recipientsTable));
                        if (recipientList.Count >= 0)
                        {
                            result.log = " Successful file upload. Uploaded file : " + fileName + " - success message : " + successText;
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                    }
                }

            }
        }

        public void amendUploadedRecipientTest(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                //change random recipient from list
                IList<IWebElement> recipientListVisible = driver.FindElements(By.CssSelector(Locator.recipientRecords));
                if (recipientListVisible.Count > 0)
                {
                    int randomRecipientIndex = new Random().Next(0, recipientListVisible.Count - 1);
                    //capture recipient details
                    string firstName_Old = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-firstName']")).Text;
                    string lastName_Old = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-lastName']")).Text;
                    string emailAddress_Old = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-emailAddress']")).Text;
                    string value_Old = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-awardValue']")).Text;
                    string personalMessage_Old = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-personalMessage']")).Text;
                    string firstName_New, lastName_New, emailAddress_New, value_New, personalMessage_New;
                    IWebElement editButton = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-edit'] > button"));
                    IWebElement deleteButton = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-delete'] > button"));

                    // edit with new values
                    editButton.ClickAction(driver);
                    //check if modal popup opens
                    if (driver.FindElements(By.CssSelector(Locator.editRecipientModal)).Count() > 0)
                    {
                        //take a screenie
                        result.log = "";
                        result.logType = LogType.INFO;
                        insertLog(test, driver, result, true);
                        //first close the modal by cancel option, then reopen it
                        IWebElement cancelBtn = driver.FindElement(By.CssSelector(Locator.editRecipientCancelBtn));
                        if (cancelBtn.Enabled) cancelBtn.ClickAction(driver);
                        editButton.ClickAction(driver);
                        // define fields
                        IWebElement firstName = driver.FindElement(By.CssSelector("#firstName"));
                        IWebElement lastName = driver.FindElement(By.CssSelector("#lastName"));
                        IWebElement email = driver.FindElement(By.CssSelector("#emailAddress"));
                        IWebElement value = driver.FindElement(By.CssSelector("#awardValue"));
                        IWebElement message = driver.FindElement(By.CssSelector(Locator.editRecipientMessage));

                        // clear fields
                        firstName.Clear();
                        lastName.Clear();
                        email.Clear();
                        value.Clear();
                        // // message.Clear();  -> for some reason, clear method doesn't reset the character counter. That's why below sequence is used
                        // message.SendKeys(Keys.Control + "a");
                        // message.SendKeys(Keys.Delete);
                        //  refill with new values
                        if (!String.IsNullOrEmpty(row["RecipientFirstName"].ToString()))
                            firstName.SendKeys(row["RecipientFirstName"].ToString());
                        else
                            firstName.SendKeys("David");
                        firstName_New = firstName.GetAttribute("value");
                        if (!String.IsNullOrEmpty(row["RecipientLastName"].ToString()))
                            lastName.SendKeys(row["RecipientLastName"].ToString());
                        else
                            lastName.SendKeys("Letterman");
                        lastName_New = lastName.GetAttribute("value");
                        if (!String.IsNullOrEmpty(row["RecipientEmail"].ToString()))
                            email.SendKeys(row["RecipientEmail"].ToString());
                        else
                            email.SendKeys("lswdl@aol.com");
                        emailAddress_New = email.GetAttribute("value");
                        awardValueInputTest(value, row["RecipientValue"].ToString(), out value_New, result);
                        recipientMessageInputTest(row["RecipientMessage"].ToString(), message, out personalMessage_New, result);

                        IWebElement saveBtn = driver.FindElement(By.CssSelector(Locator.editRecipientSaveBtn));
                        if (saveBtn.Displayed && saveBtn.Enabled)
                        {
                            saveBtn.ClickAction(driver);
                        }
                        else
                        {
                            result.log = "Edit Recipient Modal : Save button is not enabled/visible even though all fields are updated";
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, true);
                        }
                        // check if values are amended as intended


                        result.log = String.Empty; result.logType = LogType.INFO;
                        recipientListVisible = driver.FindElements(By.CssSelector(Locator.recipientRecords));
                        bool firstNameUpdate = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-firstName']")).Text.Equals(firstName_New,StringComparison.OrdinalIgnoreCase);
                        bool lastNameUpdate = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-lastName']")).Text.Equals(lastName_New, StringComparison.OrdinalIgnoreCase);
                        bool emailUpdate = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-emailAddress']")).Text.Equals(emailAddress_New, StringComparison.OrdinalIgnoreCase);
                        bool valueUpdate = extractValueFromText(recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-awardValue']")).Text).ToString().Equals(value_New, StringComparison.OrdinalIgnoreCase);
                        bool messageUpdate = recipientListVisible[randomRecipientIndex].FindElement(By.CssSelector("mat-cell[class *= 'mat-column-personalMessage']")).Text.Equals(personalMessage_New, StringComparison.OrdinalIgnoreCase);
                        if (!firstNameUpdate) { result.log += " First Name cannot be updated. Old Name : " + firstName_Old + " - New Name: " + firstName_New; }
                        if (!lastNameUpdate) { result.log += " Last Name cannot be updated. Old Name : " + lastName_Old + " - New Name: " + lastName_New; }
                        if (!emailUpdate) { result.log += " Email address cannot be updated. Old Address : " + emailAddress_Old + " - New Address: " + emailAddress_New; }
                        if (!valueUpdate) { result.log += " Value cannot be updated. Old value : " + value_Old + " - New Value: " + value_New; }
                        if (!messageUpdate) { result.log += " Message cannot be updated. Old Message : " + personalMessage_Old + " - New Message: " + personalMessage_New; }
                        if (!String.IsNullOrEmpty(result.log))
                        {
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, false);
                        }
                    }
                    else
                    {
                        result.log = "Edit recipient modal is not opened successfully for the following recipient - Line : " + randomRecipientIndex + 1;
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                }
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void addNewRecipientTest(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                driver.FindElement(By.CssSelector(Locator.filldetailsOnlineBtn)).ClickAction(driver);
                addRecipient(row, result,true);
                row["RecipientValue"] = new Random().Next(1, 100) * 5;
                row.AcceptChanges();
                addRecipient(row, result, false);
            }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }
        public void continueDeliveryPage()
        {
            checkResult result = new checkResult();
            try
            {
                if(driver.FindElements(By.CssSelector(Locator.recipientsPageContinueBtn)).Count()>0)
                {
                    IWebElement continueBtn = driver.FindElement(By.CssSelector(Locator.recipientsPageContinueBtn));
                    if (!continueBtn.Enabled)
                    {
                        result.log = " Recipients Page > Continue Button is not enabled";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    else
                        continueBtn.ClickAction(driver);
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locator.deliveryPageIndicator)));
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        public void addressAutoCompleteTest(DataRow row)
        {
            checkResult result = new checkResult();
            try
            {
                string addressSent = String.Empty;
                if (!String.IsNullOrEmpty(row["AddressPostCode"].ToString()))
                    driver.FindElement(By.CssSelector("#address")).SendKeys(row["AddressPostCode"].ToString());
                else
                    driver.FindElement(By.CssSelector("#address")).SendKeys("OX14XL");


                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(Locator.deliveryPageAddressSearchResults)));
                if (!String.IsNullOrEmpty(row["AddressFirstLine"].ToString()))
                    addressSent = row["AddressFirstLine"].ToString();
                else
                    addressSent = "11 Gordon Woodward Way";

                bool found = false;
                string autoCompleteText = String.Empty;
                
                IList<IWebElement> addressReturn = driver.FindElements(By.CssSelector(Locator.deliveryPageAddressSearchResults));
                foreach (IWebElement address in addressReturn)
                {
                    // Check if returned addresses contain first line of address
                    if((address.FindElement(By.CssSelector("span>span")).Text).IndexOf(addressSent, StringComparison.OrdinalIgnoreCase)>=0)
                    {
                        address.FindElement(By.CssSelector("span>span")).ClickAction(driver);
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    autoCompleteText = driver.FindElement(By.CssSelector(Locator.deliveryPageAutoCompleteTextArea)).GetAttribute("value").ToString();
                    if(String.IsNullOrEmpty(autoCompleteText) || !autoCompleteText.Contains(addressSent))
                    {
                        result.log = " There might be an issue with the address finder. Please check the functionality";
                        result.logType = LogType.WARNING;
                        insertLog(test, driver, result, true);
                    }
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        private void addRecipient(DataRow row, checkResult result,bool addAnother)
        {
            if (driver.FindElements(By.CssSelector(Locator.editRecipientModal)).Count() > 0)
            {
                IWebElement firstName = driver.FindElement(By.CssSelector("#firstName"));
                IWebElement lastName = driver.FindElement(By.CssSelector("#lastName"));
                IWebElement email = driver.FindElement(By.CssSelector("#emailAddress"));
                IWebElement awardValueCustomField = driver.FindElement(By.CssSelector("#awardValue"));
                IWebElement messageField = driver.FindElement(By.CssSelector(Locator.editRecipientMessage));

                //Check first if warning messages and tooltips displayed properly 
                checkMessageWarnings(result);
                checkFieldWarnings(result, firstName, lastName, email, awardValueCustomField);

                //add recipient info
                string firstNameText = row["RecipientFirstName"].ToString();
                string lastNameText = row["RecipientLastName"].ToString();
                string emailText = row["RecipientEmail"].ToString();
                string recipientValueText = row["RecipientValue"].ToString();
                string recipientMessageText = row["RecipientMessage"].ToString();

                firstName.SendKeys((!String.IsNullOrEmpty(firstNameText)) ? firstNameText : "Muhammed");
                lastName.SendKeys((!String.IsNullOrEmpty(lastNameText)) ? lastNameText : "Ali");
                email.SendKeys((!String.IsNullOrEmpty(emailText)) ? emailText : "muhammed.ali@legend.com");
                if (!String.IsNullOrEmpty(recipientValueText))
                {
                    //check if value is one of denoms
                    bool found = false;
                    IList<IWebElement> listOfDenoms = driver.FindElements(By.CssSelector(Locator.editRecipientDenoms));
                    foreach (IWebElement denom in listOfDenoms)
                    {
                        if (extractValueFromText(denom.FindElement(By.CssSelector("button")).Text) == int.Parse(recipientValueText))
                        {
                            denom.FindElement(By.CssSelector("button")).ClickAction(driver);
                            found = true;
                            break;
                        }
                    }
                    if (!found) awardValueCustomField.SendKeys(recipientValueText);
                }
                else
                    awardValueCustomField.SendKeys((new Random().Next(1, 100) * 5).ToString());
                if(messageField.GetAttribute("value").ToString().Length > 0)
                {
                    messageField.SendKeys(Keys.Control + "a");
                    messageField.SendKeys(Keys.Delete);
                }
                messageField.SendKeys((!String.IsNullOrEmpty(recipientMessageText)) ? recipientMessageText : loremIpsumValidText);

                // check if there is any warning text given
                if (driver.FindElements(By.CssSelector(Locator.editRecipientFirstNameWarning)).Count() > 0 ||
                    driver.FindElements(By.CssSelector(Locator.editRecipientLastNameWarning)).Count() > 0 ||
                    driver.FindElements(By.CssSelector(Locator.editRecipientEmailWarning)).Count() > 0 ||
                    extractValueFromText(driver.FindElement(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Text) == 0)
                {
                    result.log = " There is incorrect input in Add Recipient Modal Popup";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                    if (driver.FindElement(By.CssSelector(Locator.editRecipientSaveBtn)).Enabled)
                    {
                        result.log = " Save button is enabled eventhough there's wrong input";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                }
                else if (!driver.FindElement(By.CssSelector(Locator.editRecipientSaveBtn)).Enabled)
                {
                    result.log = " Save button is not enabled eventhough there's no wrong input";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                }
                driver.FindElement(By.CssSelector(Locator.editRecipientSaveBtn)).ClickAction(driver);
                //check if "I'm done" and "add another" buttons are visible
                if (driver.FindElements(By.CssSelector(Locator.editRecipientModalDoneBtn)).Count() > 0 &&
                    driver.FindElements(By.CssSelector(Locator.editRecipientModalAddAnotherBtn)).Count() > 0)
                {
                    if (addAnother) driver.FindElement(By.CssSelector(Locator.editRecipientModalAddAnotherBtn)).ClickAction(driver);
                    else driver.FindElement(By.CssSelector(Locator.editRecipientModalDoneBtn)).ClickAction(driver);
                }
            }
        }

        private void checkFieldWarnings(checkResult result, IWebElement firstName, IWebElement lastName, IWebElement email, IWebElement awardValueCustomField)
        {
            firstName.ClickAction(driver);
            firstName.SendKeys(Keys.Tab);
            if (driver.FindElements(By.CssSelector(Locator.editRecipientFirstNameWarning)).Count() <= 0)
            {
                result.log = "Add Recipient Modal - First Name missing warning is not displayed";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            lastName.ClickAction(driver);
            lastName.SendKeys(Keys.Tab);
            if (driver.FindElements(By.CssSelector(Locator.editRecipientLastNameWarning)).Count() <= 0)
            {
                result.log = "Add Recipient Modal - Last Name missing warning is not displayed";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            email.ClickAction(driver);
            email.SendKeys(Keys.Tab);
            if (driver.FindElements(By.CssSelector(Locator.editRecipientEmailWarning)).Count() <= 0)
            {
                result.log = "Add Recipient Modal - Email missing warning is not displayed";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            awardValueCustomField.ClickAction(driver);
            awardValueCustomField.SendKeys(Keys.Tab);
            if (driver.FindElements(By.CssSelector(Locator.editRecipientValueMissingWarning)).Count() <= 0)
            {
                result.log = "Add Recipient Modal - Award Value missing warning is not displayed";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
        }

        private void checkMessageWarnings(checkResult result)
        {
            if (driver.FindElements(By.CssSelector(Locator.editRecipientLimitWarning)).Count() > 0)
            {
                if (!driver.FindElement(By.CssSelector(Locator.editRecipientLimitWarning)).Displayed)
                {
                    result.log = "Add Recipient Modal - Maximum value per recipient warning is not displayed";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
            else
            {
                result.log = "Add Recipient Modal - Maximum value per recipient warning is not found in DOM";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            //check if placeholder is displayed within message
            string placeHolderText = driver.FindElement(By.CssSelector(Locator.editRecipientMessage)).GetAttribute("placeholder");
            if (String.IsNullOrEmpty(placeHolderText))
            {
                result.log = "Add Recipient Modal - Message placeholder is not displayed/not found";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            //check if character limit is displayed
            if (driver.FindElements(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Count() > 0)
            {
                if (!driver.FindElement(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Displayed)
                {
                    result.log = "Add Recipient Modal - Message character limit warning is found but not displayed";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
            else
            {
                result.log = "Add Recipient Modal - Message character limit warning is not found in DOM";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
            
        }

        private void recipientMessageInputTest(string row_recipientMessage, IWebElement message,out string messageNew,checkResult result)
        {
            try
            {
                int charVal = 0, maxcharVal = 0;
                if (driver.FindElements(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Count() > 0)
                {
                    charVal = extractValueFromText(driver.FindElement(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Text);
                    message.SendKeys(Keys.Control + "a");
                    message.SendKeys(Keys.Delete);
                    maxcharVal = extractValueFromText(driver.FindElement(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Text);
                }

                // check for higher value
                string replacemenetMessageAboveLimit = LoremIpsum(30, 150,10, 50, 10, maxcharVal + 50); 
                message.SendKeys(replacemenetMessageAboveLimit);
                string updatedMessage = driver.FindElement(By.CssSelector(Locator.editRecipientMessageRemainingChars)).Text;
                string replacementMessage = driver.FindElement(By.CssSelector(Locator.editRecipientMessage)).GetAttribute("value");
                if (replacementMessage.Length == maxcharVal)
                {
                    result.log = " Message has been cut to match max char. limit";
                    result.logType = LogType.INFO;
                    insertLog(test, driver, result, false);
                }
                //create a new message within limits
                string replacementMessageValid = LoremIpsum(5, 70, 1, 10, 5, maxcharVal);
                message.SendKeys(Keys.Control + "a");
                message.SendKeys(Keys.Delete);
                if (!String.IsNullOrEmpty(row_recipientMessage))
                {
                    message.SendKeys(row_recipientMessage);
                    messageNew = row_recipientMessage;
                }
                else
                {
                    message.SendKeys(replacementMessageValid);
                    messageNew = replacementMessageValid;
                    if(!message.GetAttribute("value").Equals(replacementMessageValid,StringComparison.OrdinalIgnoreCase))
                    {
                        result.log = "edit Recipient Modal - Updating Message : Valid message with max. length can't be inserted into message field.";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }
            }
            catch(Exception ex)
            {
                result.log = "Exception occured : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
                messageNew = String.Empty;
            }
        }

        private void awardValueInputTest(IWebElement valueInputField, string awardValue, out string value_New,checkResult result)
        {
            try
            {
                int val, aboveLimitValue;
                if (!String.IsNullOrEmpty(awardValue))
                {

                    int value = int.Parse(awardValue);
                    IList<IWebElement> valueOptions = driver.FindElements(By.CssSelector(Locator.editRecipientValueRow));
                    if (valueOptions.Count > 0)
                    {
                        bool valfound = false;
                        foreach (IWebElement button in valueOptions)
                        {
                            int buttonVal = int.Parse(button.FindElement(By.CssSelector("button")).Text.Replace("£", ""));
                            if (buttonVal == value)
                            {
                                button.ClickAction(driver);
                                valfound = true;
                                break;
                            }
                        }
                        if (!valfound)
                        {
                            valueInputField.SendKeys(value.ToString());
                        }

                    }
                    value_New = awardValue;
                }
                else
                {
                    // Exceeding max value test
                    aboveLimitValue = extractValueFromText(driver.FindElement(By.CssSelector(Locator.editRecipientLimitWarning)).Text);
                    int inputVal = aboveLimitValue + new Random().Next(1, 20) * 5;
                    valueInputField.SendKeys(inputVal.ToString());
                    // check if warning is displayed
                    if (driver.FindElements(By.CssSelector(Locator.editRecipientAboveLimitError)).Count() > 0)
                    {
                        string warningText = driver.FindElement(By.CssSelector(Locator.editRecipientAboveLimitError)).Text;
                        IWebElement saveBtn = driver.FindElement(By.CssSelector(Locator.editRecipientSaveBtn));
                        if (saveBtn.Enabled)
                        {
                            result.log = " Save button is enabled although the input value is above limit";
                            result.logType = LogType.FAIL;
                            insertLog(test, driver, result, true);
                        }
                        else
                        {
                            result.log = " Above limit entry warning is displayed. Warning text : " + warningText;
                            result.logType = LogType.SUCCESS;
                            insertLog(test, driver, result, false);
                        }
                    }
                    // check valid value input entry
                    valueInputField.Clear();
                    val = new Random().Next(1, aboveLimitValue / 5) * 5;
                    valueInputField.SendKeys(val.ToString());
                    //ceck if warning text is displayed
                    if (driver.FindElements(By.CssSelector(Locator.editRecipientAboveLimitError)).Count() > 0)
                    {
                        result.log = " Above limit entry warning is displayed although input is within valid range ";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    value_New = val.ToString();
                }
            }
            catch (Exception ex)
            {
                value_New = String.Empty;
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, false);
            }
        }

        private static bool IsExist(string fileDirectory, string fileName)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(Path.Combine(fileDirectory));
                bool found = false;
                foreach (string p in filePaths)
                {
                    if (p.Contains(fileName))
                    {
                        found = true;
                        break;
                    }
                }
                return found;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error received. Error message : " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public static int extractValueFromText(string text)
        {
            string b = string.Empty;
            int val = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsDigit(text[i]))
                    b += text[i];
            }

            if (b.Length > 0)
                val = int.Parse(b);
            return val;
        }
        private static string FindFont(string fontText)
        {
            string trueFont = String.Empty;
            int from = fontText.IndexOf("font-family: ") + "font-family: ".Length - 1;
            int to = fontText.Length - from;
            string adjfontText = fontText.Substring(from, to);
            adjfontText = adjfontText.Trim(' ', ';').Replace("\"", "");
            return adjfontText;
        }
        private static string defineColour(string colourStyle)
        {
            string definedColour;
            int start = colourStyle.IndexOf("rgb(");
            int end = colourStyle.IndexOf(";", start);
            definedColour = colourStyle.Substring(start, end - start);
            return definedColour.Replace(" ", "");
        }
        public static bool isElementPresent(IWebElement element)
        {
            bool flag = false;
            try
            {
                if (element.Displayed
                        || element.Enabled)
                    flag = true;
            }
            catch (NoSuchElementException e)
            {
                flag = false;
            }
            catch (StaleElementReferenceException e)
            {
                flag = false;
            }
            return flag;
        }
        public void mouseHoverJScript(IWebElement HoverElement)
        {
            checkResult result = new checkResult();
            try
            {
                if (isElementPresent(HoverElement))
                {
                    String mouseOverScript = "if(document.createEvent){var evObj = document.createEvent('MouseEvents');evObj.initEvent('mouseover', true, false); arguments[0].dispatchEvent(evObj);}else if (document.createEventObject) { arguments[0].fireEvent('onmouseover'); }";
                    ((IJavaScriptExecutor)driver).ExecuteScript(mouseOverScript, HoverElement);
                }
                else
                {
                    result.log = "Element was not visible to hover ";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
            catch (StaleElementReferenceException e)
            {
                result.log = "Element with " + HoverElement + "is not attached to the page document : " + e.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
            catch (NoSuchElementException e)
            {
                result.log = "Element " + HoverElement + " was not found in DOM : " + e.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
            catch (Exception e)
            {
                result.log = "Error occurred while hovering : " + e.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        /// <summary>
        /// A lorem ipsum paragraph generator which will be used to create recipient messages
        /// </summary>
        /// <param name="minWords"></param>
        /// <param name="maxWords"></param>
        /// <param name="minSentences"></param>
        /// <param name="maxSentences"></param>
        /// <param name="numParagraphs"></param>
        /// <returns></returns>
        private static string LoremIpsum(int minWords, int maxWords,int minSentences, int maxSentences,int numParagraphs, int maxCharacter = 0)
        {
            string returnVal = String.Empty;
            var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
            "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
            "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences)
                + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder textresult = new StringBuilder();

            for (int p = 0; p < numParagraphs; p++)
            {
         //       textresult.Append("<p>");
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { textresult.Append(" "); }
                        textresult.Append(words[rand.Next(words.Length)]);
                    }
                    textresult.Append(". ");
                }
                textresult.Append("</p>");
            }
            if (maxCharacter > 0 && textresult.Length > maxCharacter)
            {
                returnVal = textresult.ToString().Substring(0, maxCharacter);
            }
            else
                returnVal = textresult.ToString();
            return returnVal;
        }
        private static string RandomPostCode(string initial = "")
        {
            int length;
            string postCode;
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            if (String.IsNullOrEmpty(initial))
            {
                length = 6;
                postCode = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            else
            {
                length = 3;
                postCode = initial + new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            return postCode;
        }
    }
}