using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
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
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using LogType = SelectTest.Config.LogType;
using SeleniumExtras.WaitHelpers;

namespace SelectTest.PageMethods
{
    class ConfirmYourOrder : ReportsGenerationClass
    {
        private IWebDriver driver;
        private ExtentTest test;
        private WebDriverWait wait;

        public ConfirmYourOrder(IWebDriver driver, ExtentTest test, WebDriverWait wait)
        {
            this.driver = driver;
            this.test = test ?? throw new ArgumentNullException(nameof(test));
            this.wait = wait;
        }

        public void fillingDetailsandOrderComplete(string firstName, string lastName,string emailAddress, string staffNumber = "",string postCode = "", string updateFirstName="",string updateLastName = "", string updateStaffNumber = "",string updateEmail = "",string AddressLine1="",string AddressLine2 = "", string CityTown = "",string County = "") //successful entry
        {
            checkResult result = new checkResult();
            bool newDesign = false;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            try
            {
                CheckWarningsDisplayed(result, js);
                // fill the detail fields
                FillYourDetails(firstName, lastName, staffNumber, emailAddress);
                if (driver.FindElements(By.CssSelector("button.primary-button.next-button")).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(3));
                if (driver.FindElements(By.CssSelector("button.primary-button.next-button")).Count() > 0)
                {
                    newDesign = true;
                }
                if (!newDesign)
                {
                    FillYourDetails(updateFirstName, updateLastName, updateStaffNumber, updateEmail);
                    bool addressLookup = AddressLookupInitiate(result, postCode, AddressLine1, AddressLine2, CityTown, County);
                    CheckAddressFieldsPopulatedCorrectly(addressLookup, result, js);
                    //----------------------------------------------------
                    //-------  TBD : Manual Address Input Check ----------
                    //----------------------------------------------------
                    CheckTandCOldDesign(result);
                    PrivacyPolicyCheckOldDesign(result);
                    CheckSecondaryCheckBox(result, js);
                    changeOrderTestOldDesing(result, js);
                }
                if (newDesign)
                {
                    ClickNext(result);
                    CheckYourDetailsPreserved(result, ref firstName, ref lastName, ref staffNumber, ref emailAddress);
                    // update  values from Your Details
                    FillYourDetails(updateFirstName, updateLastName, updateStaffNumber, updateEmail);
                    ClickNext(result);
                    CheckYourDetailsPreserved(result, ref updateFirstName, ref updateLastName, ref updateStaffNumber, ref updateEmail);
                    ClickNext(result);
                    bool addressLookup = AddressLookupInitiate(result, postCode, AddressLine1, AddressLine2, CityTown, County);
                    CheckAddressFieldsPopulatedCorrectly(addressLookup, result, js);
                    //----------------------------------------------------
                    //-------  TBD : Manual Address Input Check ----------
                    //----------------------------------------------------
                    TandCLinkCheckNewDesign(result);
                    PrivacyPolicyCheckNewDesign(result);
                    CheckChangeOrder(result);
                    //check if there's secondary checkbox                     
                }

                IWebElement placeOrderBtn = CheckTandCCheckBox(result);
                placeOrderBtn.Click();

                //Check if confirm popup appears
                //if (driver.FindElements(By.XPath(ConfirmCheckoutPopupPath)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 2));
                //else
                //{
                //    // take a screenie
                //    result.log = "Confirm Popup Screenshot taken for review";
                //    result.logType = LogType.SUCCESS;
                //    insertLog(result);
                //    IWebElement goBackBtn = driver.FindElement(By.CssSelector(ConfirmPopupGoBackBtnPath));
                //    goBackBtn.ClickAction(driver); // Check Go Back works
                //}
                //placeOrderBtn = driver.FindElement(By.XPath(PlaceOrderBtnAfterPath));
                //placeOrderBtn.ClickAction(driver);
                //Check Confirm Btn Working

                if (driver.FindElements(By.XPath(Locator.ConfirmCheckoutPopupPath)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 2));
                else
                {
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(Locator.ConfirmPopupConfirmBtnPath)));
                    driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);
                    IList<IWebElement> goBackCheck = driver.FindElements(By.CssSelector(Locator.ConfirmPopupGoBackBtnPath));
                    if(goBackCheck.Count>0)
                    {
                        goBackCheck[0].ClickAction(driver);
                    }
                    placeOrderBtn.Click();
                    IWebElement confirmBtn = driver.FindElement(By.XPath("//button[@form='confirmationForm']"));
                    confirmBtn.Click();
                    // As an alternative click and submit method
                    //js.ExecuteScript("arguments[0].click();", confirmBtn); // For some reason, confirm button can't be clicked. Bypassed ClickAction as it fails with the button being "Submit" type. 
                    //js.ExecuteScript("document.getElementById('confirmationForm').submit();");
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    result.log = "Order Confirm process completed successfully.";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, false);
                }
                // Check if Order Complete Page is displayed successfully
                IList<IWebElement> codeListeGift = driver.FindElements(By.XPath("//*[@id='reactRoot']/div/div/div[@class='code-group e-gifts']"));
                IList<IWebElement> codeListGiftCard = driver.FindElements(By.XPath("//*[@id='reactRoot']/div/div/div[@class='code-group giftCards']"));
                if ((codeListeGift != null && codeListeGift.Count > 0) || (codeListGiftCard != null && codeListGiftCard.Count > 0))
                {
                    //Check if there's Demo Session Popup
                    if (driver.FindElements(By.XPath("//div[@class='noty_body']")).Count() > 0)
                    {
                        //CLOSE THE POPUP
                        IWebElement closeBtn = driver.FindElement(By.CssSelector("#notificationBox > div.noty_body > div > button"));
                        closeBtn.ClickAction(driver);
                    }

                    //-------------------------------------------------------------------------------
                    //   TBD: Check if the print button works properly on Order Complete Page
                    //------------------------------------------------------------------------------   

                    result.log = "Order Completed. Check the Order Complete Page Screenshot";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, true);
                }
                else
                {
                    // this means page is not loaded successfully and we are still in confirm order page. This code is temporary,until devs correct the Confirm Button functionality
                    driver.Navigate().Refresh(); // refresh the page.

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log += ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsTrue(false);
            }

            void FillYourDetails(string firstNameParam, string lastNameParam, string staffNumberParam, string emailAddressParam)
            {
                UpdateField(Locator.FirstNamePath, firstNameParam);
                UpdateField(Locator.LastNamePath, lastNameParam);
                UpdateField(Locator.StaffNumberPath, staffNumberParam);
                UpdateField(Locator.EmailAddressPath, emailAddressParam);
                UpdateField(Locator.ConfirmEmailAddressPath, emailAddressParam);

                void UpdateField(string path,string fieldname)
                {
                    if (driver.FindElements(By.CssSelector(path)).Count() > 0)
                    {
                        IWebElement el = driver.FindElement(By.CssSelector(path));
                        if (el.GetAttribute("value").Length != 0)
                        {
                            el.Clear();
                            el.SendKeys(Keys.Tab);
                        }
                        el.SendKeys(fieldname);
                    }
                }
            }
        }

        private void changeOrderTestOldDesing(checkResult result, IJavaScriptExecutor js)
        {
            //Change Order Test
            bool slickUse = false; //for fashioncadeau
            IWebElement changeOrder = driver.FindElement(By.XPath(Locator.ChangeOrderBtnPath));
            if (changeOrder != null && changeOrder.Enabled)
            {
                changeOrder.ClickAction(driver);
                if (driver.FindElements(By.CssSelector(Locator.DisplayedBasketPath)).Count() > 0)
                {
                    result.log = " Change Order initiated successfully. Basket displayed";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, true);
                }
                // removal of the last product
                IList<IWebElement> productRemoves = driver.FindElements(By.CssSelector("div.basketBottom > div.basketGroup > div.basketLineItem"));
                int size = productRemoves.Count;
                IWebElement lastProductRemove = productRemoves[size - 1].FindElement(By.CssSelector("button[aria-label='remove item'"));
                lastProductRemove.ClickAction(driver);
                double orderValue = double.Parse(driver.FindElement(By.CssSelector(Locator.TotalValuePath)).Text.Remove(0, 1));
                if(driver.FindElements(By.CssSelector("div.closePanel > div.closePanelArea > button")).Count()>0)
                    driver.FindElement(By.CssSelector("div.closePanel > div.closePanelArea > button")).ClickAction(driver);
                // adding a new product
                double remainingValue = 0;
                double maximumOrderLimit = 0;
                //Check whether there is maximumOrderLimit
                IList<IWebElement> orderLimit = driver.FindElements(By.CssSelector(Locator.MaximumOrderLimitPath));
                if (orderLimit.Count > 0) maximumOrderLimit = double.Parse(Extensions.extractMaximum(orderLimit[0].Text)); // there is a max order limit which we need to check

                if (driver.FindElements(By.CssSelector("div.basketPanel.open > div.closePanel > div.closePanelArea > button")).Count() > 0)
                    driver.FindElement(By.CssSelector("div.basketPanel.open > div.closePanel > div.closePanelArea > button")).ClickAction(driver);
                IList<IWebElement> productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                if(productList.Count<=0) //for fashioncadeau and similar sites, above statement doesn't bring products. So use the below one instead (Slick Container)
                {
                    productList = driver.FindElements(By.XPath(Locator.SlickVisibleProductsPath));
                    if(productList.Count>0) slickUse = true;
                    else
                    {
                        result.log = " Products are not loaded or detected.";
                        result.logType = LogType.FATAL;
                        insertLog(test, driver, result, true);
                        Assert.IsTrue(false);
                    }
                }
                Random rnd = new Random();
                int productIndex = rnd.Next(1, productList.Count);
                IWebElement parent = null;
                IWebElement openProductViewBtn = null ;
                if (!slickUse)
                {
                    parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", productList[productIndex]));
                    parent.ClickAction(driver);
                }
                else
                {
                    IWebElement slickSlider = driver.FindElement(By.CssSelector("div.slick-container > div > button.slick-arrow.slick-next"));
                    //check if element is in the visibility scope
                    while (!productList[productIndex].Displayed)
                    {
                        slickSlider.ClickAction(driver);
                    }
                    productList[productIndex].ClickAction(driver);
                }
                Thread.Sleep(TimeSpan.FromSeconds(3));
                IWebElement remaining;
                if (driver.FindElements(By.CssSelector(Locator.RemainingValuePath)).Count() > 0)
                {
                    remaining = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                    remainingValue = double.Parse(remaining.Text.Remove(0, 1));
                    if (remainingValue > 0 && (maximumOrderLimit == 0 || (maximumOrderLimit > 0 && orderValue.CompareTo(maximumOrderLimit) < 0)))
                    {
                        double customValue = 0;
                        double allowedMaximum = 0;
                        IList<IWebElement> customFind;
                        if(!slickUse)
                         customFind = driver.FindElements(By.CssSelector(Locator.CustomValueField));
                        else
                        customFind = driver.FindElements(By.CssSelector("div.tab.open > div > div > div.custom-value-area > input"));
                        if (customFind.Count > 0)
                        {
                            while (remainingValue > 0 && (orderValue == 0 || ((maximumOrderLimit > 0 && orderValue.CompareTo(maximumOrderLimit) != 0) || maximumOrderLimit == 0)))
                            {
                                customValue = remainingValue;
                                customFind[0].SendKeys(remainingValue.ToString());
                                IWebElement message;
                                if (slickUse) Locator.CustomValueWarningMessage = "div.tab.open > div > div > div.custom-message-area>p";
                                while (driver.FindElements(By.CssSelector(Locator.CustomValueWarningMessage)).Count() > 0)
                                {
                                    message = driver.FindElement(By.CssSelector(Locator.CustomValueWarningMessage));
                                    string messageText = message.Text;
                                    allowedMaximum = double.Parse(Extensions.extractMaximum(messageText));
                                    if ((maximumOrderLimit > 0 && (allowedMaximum + orderValue).CompareTo(maximumOrderLimit) == 0)|| (maximumOrderLimit == 0 & allowedMaximum.CompareTo(remainingValue)<=0))
                                        customValue = allowedMaximum;
                                    else
                                        continue;
                                    int length = customFind[0].GetAttribute("value").Length;
                                    for (int j = 0; j < length; j++)
                                    {
                                        customFind[0].SendKeys(Keys.Backspace);
                                        if (customFind[0].GetAttribute("value").Length == 0) break;
                                    }
                                    customFind[0].SendKeys(customValue.ToString()); // adjusted custom value placed.
                                }
                                IWebElement addButton = null;
                                try
                                {
                                    addButton = driver.FindElement(By.CssSelector(Locator.AddProductButton));
                                }
                                catch (Exception ex)
                                {
                                    string alternativeAddButtonPath = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > button";
                                    addButton = driver.FindElement(By.CssSelector(alternativeAddButtonPath));
                                }
                                addButton.ClickAction(driver);
                                break;
                            }
                        }
                        // Close Product Panel
                        IList<IWebElement> buttonSearch = driver.FindElements(By.CssSelector(Locator.ProductCloseButton));
                        if (buttonSearch.Count() > 0)
                        {
                            buttonSearch[0].ClickAction(driver);
                        }
                        //Checkout
                        if (driver.FindElements(By.CssSelector(Locator.CheckoutBtnPath)).Count() > 0)
                            driver.FindElement(By.CssSelector(Locator.CheckoutBtnPath)).ClickAction(driver);
                    }
                }
            }
        }

        private void CheckSecondaryCheckBox(checkResult result, IJavaScriptExecutor js)
        {
            // Check if there's any secondary checkbox
            if(driver.FindElements(By.CssSelector("input[type=checkbox][name=secondaryTermsCheckbox]")).Count()>0)
            {
                IList<IWebElement> checkBoxes = driver.FindElements(By.CssSelector("input[type=checkbox][name=secondaryTermsCheckbox]"));
                IWebElement parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", checkBoxes[0]));
                //check if there's any link within secondary box text
                if (parent.FindElements(By.CssSelector("p>a")).Count() > 0)
                {
                    IList<IWebElement> links = parent.FindElements(By.CssSelector("p>a"));
                    foreach (IWebElement link in links)
                    {
                        link.ClickAction(driver);
                        string linktext = link.GetAttribute("href");
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        var browserTabs = driver.WindowHandles;
                        driver.SwitchTo().Window(browserTabs[1]);

                        result.log = "Link in secondary checkbox opened: " + linktext;
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);

                        driver.Close();
                        driver.SwitchTo().Window(browserTabs[0]);
                    }
                    //click secondary checkbox
                    driver.FindElement(By.CssSelector("input[type=checkbox][name=secondaryTermsCheckbox]")).ClickAction(driver);
                }
            }
        }

        private void CheckYourDetailsPreserved(checkResult result1, ref string firstNameCheck, ref string lastNameCheck, ref string staffNumberCheck, ref string emailAddressCheck)
        {
            if (driver.FindElements(By.CssSelector(Locator.YourDetailsSummaryPath)).Count() <= 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            else
            {
                if (driver.FindElements(By.CssSelector(Locator.YourDetailsEditButtonPath)).Count() > 0)
                {
                    driver.FindElement(By.CssSelector(Locator.YourDetailsEditButtonPath)).ClickAction(driver);
                }
                // Check if all previously input details are kept
                if (driver.FindElements(By.CssSelector("#confirmationForm > div.largeWrap > div.details > div:nth-child(1)")).Count() < 0) Thread.Sleep(TimeSpan.FromSeconds(3));
                else
                {
                    result1.logType = LogType.SUCCESS;
                    result1.log = "";
                    if (driver.FindElements(By.CssSelector("input[name=FirstNameInput]")).Count() > 0)
                    {
                        if (driver.FindElement(By.CssSelector("input[name=FirstNameInput]")).GetAttribute("value") != firstNameCheck)
                        {
                            result1.logType = LogType.FAIL;
                            result1.log += "First name is not preserved correctly when editing" + Environment.NewLine;
                        }
                    }
                    if (driver.FindElements(By.CssSelector("input[name=LastNameInput]")).Count() > 0)
                    {
                        if (driver.FindElement(By.CssSelector("input[name=LastNameInput]")).GetAttribute("value") != lastNameCheck)
                        {
                            result1.logType = LogType.FAIL;
                            result1.log += "Last name is not preserved correctly when editing" + Environment.NewLine;
                        }
                    }
                    if (driver.FindElements(By.CssSelector("input[name=StaffNumberInput]")).Count() > 0)
                    {
                        if (driver.FindElement(By.CssSelector("input[name=StaffNumberInput]")).GetAttribute("value") != staffNumberCheck)
                        {
                            result1.logType = LogType.FAIL;
                            result1.log += "Staff number is not preserved correctly when editing" + Environment.NewLine;
                        }
                    }
                    if (driver.FindElements(By.CssSelector("input[name=EmailAddressInput]")).Count() > 0)
                    {
                        if (driver.FindElement(By.CssSelector("input[name=EmailAddressInput]")).GetAttribute("value") != emailAddressCheck)
                        {
                            result1.logType = LogType.FAIL;
                            result1.log += "Email Address is not preserved correctly when editing" + Environment.NewLine;
                        }
                    }

                    if (driver.FindElements(By.CssSelector("input[name=ConfirmEmailAddressInput]")).Count() > 0)
                    {
                        if (driver.FindElement(By.CssSelector("input[name=ConfirmEmailAddressInput]")).GetAttribute("value") != emailAddressCheck)
                        {
                            result1.logType = LogType.FAIL;
                            result1.log += "Staff number is not preserved correctly when editing" + Environment.NewLine;
                        }
                    }
                    if (result1.logType == LogType.SUCCESS)
                        result1.log = "'Your details' data preserved when tried to be updated via update button (pencil icon)";
                    insertLog(test, driver, result1, true);
                }
            }
        }

        private void ClickNext(checkResult result2)
        {
            if (driver.FindElements(By.CssSelector("button.primary-button.next-button")).Count() > 0)
            {
                IWebElement el = driver.FindElement(By.CssSelector("button.primary-button.next-button"));
                if (el.Enabled) el.ClickAction(driver);
                else
                {
                    result2.logType = LogType.FAIL;
                    result2.log = "NEXT button is not enabled although all fields are populated.";
                    insertLog(test, driver, result2, true);
                    Assert.IsTrue(false);
                }
            }
        }

        private void CheckWarningsDisplayed(checkResult resultWarning, IJavaScriptExecutor js)
        {
            // check if warnings are displayed for all mandatory fields
            IList<IWebElement> warningList = driver.FindElements(By.CssSelector("div.inputGroup > span.error"));
            IList<IWebElement> nameList = driver.FindElements(By.CssSelector("div.inputGroup  > div.iconWrap > span.focus-label"));
            int charLocationList;
            int charLocationName;
            string listItemText;
            string nameItemText;
            bool areEqual = false;

            if (warningList.Count == 0) // If warnings are not displayed on page load, trigger them
            {
                if (driver.FindElements(By.CssSelector("input[name=FirstNameInput]")).Count() > 0)
                {
                    IWebElement firstNameTrigger = driver.FindElement(By.CssSelector("input[name=FirstNameInput]"));
                    firstNameTrigger.SendKeys("A");
                    firstNameTrigger.SendKeys(Keys.Tab);
                    firstNameTrigger.SendKeys(Keys.Backspace);
                }
                warningList = driver.FindElements(By.CssSelector("div.inputGroup > span.error"));
                if (warningList.Count > 0)
                {
                    // Take a screenshot of the warnings
                    resultWarning.log = "Warning messages under each field are displayed. Screenshot taken for review";
                    resultWarning.logType = LogType.SUCCESS;
                    insertLog(test, driver, resultWarning, true);
                }
            }

            for (int i = 0; i < warningList.Count; i++)
            {
                try
                {
                    IWebElement parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", warningList[i]));
                    IWebElement sibling = parent.FindElement(By.CssSelector("div.iconWrap"));
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (String.IsNullOrEmpty(warningList[i].Text)) continue; //for some reason, some warning text elements are hidden and not populated. That;s why skipping them.
                charLocationList = warningList[i].Text.IndexOf("must", StringComparison.Ordinal) - 1;
                if (charLocationList > 0)
                {
                    listItemText = warningList[i].Text.Substring(0, charLocationList);
                    for (int j = 0; j < nameList.Count; j++)
                    {
                        charLocationName = nameList[j].Text.IndexOf("*", StringComparison.Ordinal);
                        nameItemText = nameList[j].Text.Substring(0, charLocationName);
                        areEqual = String.Equals(listItemText, nameItemText, StringComparison.OrdinalIgnoreCase);
                        if (areEqual)
                        {
                            break;
                        }
                    }
                    if (!areEqual)
                    {
                        resultWarning.log = "Warning message text and field not matching! Check screenshot.";
                        resultWarning.logType = LogType.FAIL;
                        insertLog(test, driver, resultWarning, true);
                        Assert.IsTrue(false);
                    }
                }
            }
        }

        private void CheckAddressFieldsPopulatedCorrectly(bool addressLook, checkResult result, IJavaScriptExecutor js)
        {
            if (addressLook)
            {
                // Check if every 'mandatory' address field is populated 
                result.log = "";
                result.logType = LogType.SUCCESS;
                CheckMandatoryAddressFieldPopulated(js, Locator.AddressLine1Path, result);
                CheckMandatoryAddressFieldPopulated(js, Locator.AddressLine2Path, result);
                CheckMandatoryAddressFieldPopulated(js, Locator.CityTownPath, result);
                CheckMandatoryAddressFieldPopulated(js, Locator.CountyPath, result);
                CheckMandatoryAddressFieldPopulated(js, Locator.PostcodePath, result);
                CheckMandatoryAddressFieldPopulated(js, Locator.CountryPath, result);
                if (result.logType == LogType.SUCCESS) result.log += "Address fields are populated correctly via address lookup.";
                insertLog(test, driver, result, true);
            }
        }

        private void CheckMandatoryAddressFieldPopulated(IJavaScriptExecutor executor, string addresslinePath, checkResult result)
        {
            if (driver.FindElements(By.CssSelector(addresslinePath)).Count() > 0 && String.IsNullOrEmpty(driver.FindElement(By.CssSelector(addresslinePath)).GetAttribute("value")))
            {
                IWebElement el = driver.FindElement(By.CssSelector(addresslinePath));
                IWebElement parent = (IWebElement)(executor.ExecuteScript(
                               "return arguments[0].parentNode;", el));
                if (parent.FindElements(By.CssSelector("//following-sibling::span[@class='error']")).Count() > 0)
                {
                    result.log += "Mandatory Address input not populated by Address Lookup.";
                    result.logType = LogType.FAIL;
                }
            }
        }

        private void TandCLinkCheckNewDesign(checkResult resultTC)
        {
            // Terms and Conditions Link Check
            if (driver.FindElements(By.CssSelector(Locator.TandCsPathNewDesign)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 5));
            if (driver.FindElements(By.CssSelector(Locator.TandCsPathNewDesign)).Count() <= 0)
            {
                resultTC.log = " Terms and Conditions hyperlink cannot be found on page.";
                resultTC.logType = LogType.FAIL;
                insertLog(test, driver, resultTC, false);
            }
            else
            {
                string tandclink = driver.FindElement(By.CssSelector(Locator.TandCsPathNewDesign)).GetAttribute("href");
                IWebElement el = driver.FindElement(By.CssSelector(Locator.TandCsPathNewDesign));
                el.ClickAction(driver);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                var browserTabs = driver.WindowHandles;
                driver.SwitchTo().Window(browserTabs[1]);
                if (driver.Title == "Hawk Select | T&Cs")
                {
                    resultTC.log = "T&C page opened successfully. Link of the T&Cs page : " + tandclink;
                    resultTC.logType = LogType.SUCCESS;
                    insertLog(test, driver, resultTC, true);
                }
                driver.Close();
                driver.SwitchTo().Window(browserTabs[0]);
            }
        }

        private void PrivacyPolicyCheckNewDesign(checkResult resultPP)
        {
            //Cheking Privacy Policy
            if (driver.FindElements(By.CssSelector(Locator.PrivacyPolicyPathNewDesign)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 5));
            else
            {
                string privacyLink = driver.FindElement(By.CssSelector(Locator.PrivacyPolicyPathNewDesign)).GetAttribute("href");
                IWebElement el = driver.FindElement(By.CssSelector(Locator.PrivacyPolicyPathNewDesign));
                el.ClickAction(driver);
                Thread.Sleep(TimeSpan.FromSeconds(3));
                var browserTabs = driver.WindowHandles;
                driver.SwitchTo().Window(browserTabs[1]);
                if (driver.Title == "Blackhawk Network - Privacy Notice")
                {
                    resultPP.log = "Privacy Policy page opened successfully. Link of the privacy policy page : " + privacyLink;
                    resultPP.logType = LogType.SUCCESS;
                    insertLog(test, driver, resultPP, true);
                }
                driver.Close();
                driver.SwitchTo().Window(browserTabs[0]);
            }
        }

        private void PrivacyPolicyCheckOldDesign(checkResult resultPP)
        {
            bool alternativePathUse = false;
            string alternativeTandCPath = "input[type = checkbox][name=termsCheckbox]";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //Cheking Privacy Policy
            if (driver.FindElements(By.CssSelector(Locator.TandCsPathOldDesign)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 5));
            if (driver.FindElements(By.CssSelector(Locator.TandCsPathOldDesign)).Count() <= 0)
            {
                try
                {

                    if (driver.FindElements(By.CssSelector(alternativeTandCPath)).Count() <= 0)
                    {
                        resultPP.log = " Privacy policy/ T & C Checkbox cannot be found on page.";
                        resultPP.logType = LogType.FAIL;
                        insertLog(test, driver, resultPP, false);
                    }
                    else
                    {
                        alternativePathUse = true;
                    }
                }
                catch (Exception ex)
                {
                    resultPP.log = " Terms and Conditions hyperlink cannot be found on page.";
                    resultPP.logType = LogType.FAIL;
                    insertLog(test, driver, resultPP, false);
                }
            }
            else
            {
                alternativePathUse = false;
            }
            string privacyPolicyLinkText = String.Empty;
            IWebElement checkBox;
            IWebElement privacyPolicyLink;
            if (alternativePathUse)
            {
                checkBox = driver.FindElement(By.CssSelector(alternativeTandCPath));
                IWebElement parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", checkBox));
                privacyPolicyLink = parent.FindElement(By.CssSelector("div>p>b:nth-child(3)>a"));
                privacyPolicyLinkText = privacyPolicyLink.GetAttribute("href");
            }
            else
            {
                privacyPolicyLink = driver.FindElement(By.CssSelector(Locator.TandCsPathOldDesign));
                privacyPolicyLinkText = privacyPolicyLink.GetAttribute("href");
                checkBox = driver.FindElement(By.CssSelector(Locator.TandCsPathOldDesign));
            }

            privacyPolicyLink.ClickAction(driver);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var browserTabs = driver.WindowHandles;
            driver.SwitchTo().Window(browserTabs[1]);

            resultPP.log = "Privacy policy page opened successfully. Link of the Privacy Policy page : " + privacyPolicyLinkText;
            resultPP.logType = LogType.SUCCESS;
            insertLog(test, driver, resultPP, false);

            driver.Close();
            driver.SwitchTo().Window(browserTabs[0]);
        }

        private void CheckTandCOldDesign(checkResult result)
        {
            
            bool alternativePathUse = false;
            string alternativeTandCPath = "input[type = checkbox][name=termsCheckbox]";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            // Terms and Conditions Link Check
            if (driver.FindElements(By.CssSelector(Locator.TandCsPathOldDesign)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 5));
            if (driver.FindElements(By.CssSelector(Locator.TandCsPathOldDesign)).Count() <= 0)
            {
                try
                {

                    if (driver.FindElements(By.CssSelector(alternativeTandCPath)).Count() <= 0)
                    {
                        result.log = " Terms and Conditions hyperlink cannot be found on page.";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                    else
                    {
                        alternativePathUse = true;
                    }
                }
                catch (Exception ex)
                {
                    result.log = " Terms and Conditions hyperlink cannot be found on page.";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, false);
                }
            }
            else
            {
                alternativePathUse = false;
            }
            string tandclinkText = String.Empty;
            IWebElement checkBox;
            IWebElement tandcLink;
            if (alternativePathUse)
            {
                checkBox = driver.FindElement(By.CssSelector(alternativeTandCPath));
                IWebElement parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", checkBox));
                tandcLink = parent.FindElement(By.CssSelector("p>b:nth-child(1)>a"));
                tandclinkText = tandcLink.GetAttribute("href");
            }
            else
            {
                tandcLink = driver.FindElement(By.CssSelector(Locator.TandCsPathOldDesign));
                tandclinkText = tandcLink.GetAttribute("href");
                checkBox = driver.FindElement(By.CssSelector(Locator.TandCsPathOldDesign));
            }
                
            checkBox.ClickAction(driver);
            tandcLink.ClickAction(driver);
            Thread.Sleep(TimeSpan.FromSeconds(5));
            var browserTabs = driver.WindowHandles;
            driver.SwitchTo().Window(browserTabs[1]);

            result.log = "T&C page opened successfully. Link of the T&Cs page : " + tandclinkText;
            result.logType = LogType.SUCCESS;
            insertLog(test, driver, result, false);

            driver.Close();
            driver.SwitchTo().Window(browserTabs[0]);

            IWebElement tandcCheckBox;
            if (driver.FindElements(By.CssSelector(alternativeTandCPath)).Count() > 0)
            {
                tandcCheckBox = driver.FindElement(By.CssSelector(alternativeTandCPath));
                if (tandcCheckBox != null)
                {
                    IWebElement placeOrder = driver.FindElement(By.XPath(Locator.PlaceOrderBtnPath));
                    //Check if Place Order is disabled without ticking this and gets enabled after ticking.
                    if (!tandcCheckBox.Selected && placeOrder.Enabled)
                    {
                        result.log = " Place order is enabled without checking the Terms and Conditions";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    driver.FindElement(By.CssSelector(alternativeTandCPath)).ClickAction(driver);
                    result.log = "T&C Checkbox is checked successfully.";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, false);
                }
            }
            else
            {
                result.log = "T&C checkbox not detected!";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
            }
        }

        private IWebElement CheckTandCCheckBox(checkResult result)
        {
            // Tick T&Cs
            // Check the status of the Checkbox and Place Order Buttons
            Thread.Sleep(new TimeSpan(0, 0, 2)); // Just for page load
            IWebElement placeOrderBtn = driver.FindElement(By.XPath(Locator.PlaceOrderBtnPath));
            IWebElement checkBox = driver.FindElement(By.CssSelector(Locator.TandCCheckBoxPath));
            if (!checkBox.Selected && placeOrderBtn.Enabled)
            {
                result.log = "Place Order button is enabled while the T&C checkbox is not ticked!";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
                Assert.IsTrue(false);
            }
            checkBox.ClickAction(driver);
            if (checkBox.Selected && !placeOrderBtn.Enabled)
            {
                result.log = "Place Order button is not enabled although the T&C checkbox is ticked!";
                result.logType = LogType.FAIL;
                insertLog(test, driver, result, true);
                Assert.IsTrue(false);
            }

            return placeOrderBtn;
        }

        private void CheckChangeOrder(checkResult result)
        {
            //Check "Change Order" button functionality
            IWebElement changeOrderBtn = driver.FindElement(By.XPath(Locator.ChangeOrderBtnPath));
            changeOrderBtn.ClickAction(driver);
            // Check if CHoose Page opens
            if (driver.FindElements(By.CssSelector(Locator.ChoocePageIdentifier)).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(3));
            else
            {
                // Check if basket is open
                if (driver.FindElements(By.CssSelector(Locator.DisplayedBasketPath)).Count() == 0) // is basket displayed now?
                {
                    IWebElement myBasket = driver.FindElement(By.CssSelector(Locator.OpenBasketButtonPath));
                    myBasket.ClickAction(driver);
                }
                // check total value is greater than 0 (meaning basket retrieved successfully)
                IWebElement total = driver.FindElement(By.CssSelector(Locator.TotalValuePath));
                string totalVal = total.Text.Substring(1, total.Text.Length - 1);
                double totalFormatted = double.Parse(totalVal);
                if (totalFormatted > 0)
                {
                    result.log = "Basket and products preserved during 'Change Order' process.";
                    result.logType = LogType.SUCCESS;
                    insertLog(test, driver, result, true);
                }

                //---------------------------------------------------------------------
                //TBD: Write code to change order and check whether it is updated
                //---------------------------------------------------------------------

                // Checkout again
                IWebElement checkoutBtn = driver.FindElement(By.CssSelector(Locator.CheckoutBtnPath));
                checkoutBtn.ClickAction(driver);
            }
        }

        private bool AddressLookupInitiate(checkResult result, string postCode,string addressLine1param, string addressLine2param,string cityparam,string countyparam)
        {
            try
            {
                //AddressInputCheck
                if (driver.FindElements(By.CssSelector(Locator.AddressInputFieldPath)).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(3));
                if(driver.FindElements(By.CssSelector(Locator.AddressInputFieldPath)).Count() <= 0)
                {
                    result.log = " Address input not available. There might be no physical product in the basket. Please check";
                    result.logType = LogType.WARNING;
                    insertLog(test, driver, result, false);
                    return false;
                }
                else
                {
                    IWebElement elm = driver.FindElement(By.CssSelector(Locator.AddressInputFieldPath));
                    elm.SendKeys(postCode);
                }
                bool addressFound = true;
                while(driver.FindElements(By.CssSelector(Locator.AddressResultPopup)).Count() <= 0)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    if (driver.FindElements(By.XPath("//div[contains(@class,'menu-notice--no-options')]")).Count() > 0)
                    {
                        IList<IWebElement> el = driver.FindElements(By.XPath("//div[contains(@class,'menu-notice--no-options')]"));
                        result.log = "Address lookup hasn't returned a result. Popup text : " + el[0].Text + " . Check if the postcode is valid. Otherwise, addresslookup might be not working";
                        result.logType = LogType.FAIL;
                        insertLog(test,driver,result,true);
                        addressFound = false;
                        break;
                    }
                }
                if(addressFound == false)
                {
                    //try manual address input
                    IWebElement manuelEntryBtn = driver.FindElement(By.CssSelector("a.secondary-action-link"));
                    manuelEntryBtn.Click();
                    if(driver.FindElements(By.CssSelector("input[name=AddressLine1Input]")).Count()>0)
                        driver.FindElement(By.CssSelector("input[name=AddressLine1Input]")).SendKeys(addressLine1param);
                    if(driver.FindElements(By.CssSelector("input[name=AddressLine2Input]")).Count()>0)
                        driver.FindElement(By.CssSelector("input[name=AddressLine2Input]")).SendKeys(addressLine2param);
                    if(driver.FindElements(By.CssSelector("input[name=CityTownInput]")).Count()>0)
                        driver.FindElement(By.CssSelector("input[name=CityTownInput]")).SendKeys(cityparam);
                    if (driver.FindElements(By.CssSelector("input[name=CountyInput]")).Count() > 0)
                        driver.FindElement(By.CssSelector("input[name=CountyInput]")).SendKeys(countyparam);
                    if (driver.FindElements(By.CssSelector("input[name=PostcodeInput]")).Count() > 0)
                        driver.FindElement(By.CssSelector("input[name=PostcodeInput]")).SendKeys(postCode);
                }    
                IList <IWebElement> addressList = driver.FindElements(By.CssSelector(Locator.AddressResultPopup));
                addressList[0].ClickAction(driver);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log += ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsTrue(false);
                return false;
            }
        }
    }
}
