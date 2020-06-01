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
    class ChoosePage : ReportsGenerationClass
    {
        private IWebDriver driver;
        private ExtentTest test;
        private WebDriverWait wait;
        private LoginPage login;
        

        private bool isFilterEnabled = true;

        public ChoosePage(IWebDriver driver, ExtentTest test, WebDriverWait wait,LoginPage login)
        {
            this.driver = driver;
            this.test = test ?? throw new ArgumentNullException(nameof(test));
            this.wait = wait;
            this.login = new LoginPage(driver, test, wait);
        }
        public bool gotoChoosePage(string URL, string code,string pin = "",string cvv = "", string expiryDate = "")
        {
            try
            {
                checkResult result = new checkResult();
                bool validSite = true;
                // with the assumption that login tests performed and we can successfully login to site
                validSite = login.goToPage(URL);
                if (!validSite) return false;
                // check if page opens and every link on the page receives HTTP 200 response or not (TBD : this needs to be a separate test case)
                login.clickCookieP();

                login.enterFieldValue(code, Locator.codeInput);
                login.enterFieldValue(pin, Locator.pinInput);
                login.enterFieldValue(cvv, Locator.cv2Input);
                login.enterFieldValue(expiryDate, Locator.expiryDateInput);
                login.clickbtnContinue();
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(Locator.ChoosePageVerifierPath)));
                //Check if the code entered is valid or not redeemed yet
                // Check if we are on Choose Page
                if (driver.FindElements(By.CssSelector(Locator.ChoosePageVerifierPath)).Count() <= 0) // Still missing after delay
                {
                    if (driver.FindElements(By.CssSelector(Locator.CodeEntryNotificationPath)).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(6));
                    if (driver.FindElements(By.CssSelector(Locator.CodeEntryNotificationPath)).Count() > 0)
                    {
                        result.log = "Code is not accepted. Check the screenshot";
                        insertLog(test,driver,result,true);
                        return false;
                    }
                    return true;
                }
                else
                {
                    result.log = "Code is accepted.";
                    //Check for notification box
                    if (driver.FindElements(By.CssSelector(Locator.CodeEntryNotificationPath)).Count() > 0)
                    {
                        result.log += " We have a notification box with the following content : " + driver.FindElement(By.CssSelector(Locator.CodeEntryNotificationPath)).Text;
                        insertLog(test,driver,result,true);
                        driver.FindElement(By.CssSelector(Locator.CodeEntryNotificationPopupButton)).ClickAction(driver);
                    }
                    else
                        insertLog(test,driver,result,false);
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                    return true;
                }
            }
            catch(Exception exm)
            {
                Console.WriteLine(exm.Message + Environment.NewLine + exm.StackTrace);
                checkResult result = new checkResult();
                result.logType = LogType.FATAL;
                result.log = exm.Message + Environment.NewLine + exm.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(true);
                return true;
            }
        }
        public void filterSection_GiftTypeSelection()
        {
            try
            {
                checkResult result = new checkResult(); //keep test result text
                                                        //check if filtersection visible
                Thread.Sleep(TimeSpan.FromSeconds(5)); // Let the system load elements before performing actions
                if (isFilterEnabled && filterSection_filterVisible() != null)
                {
                   
                    var giftTypeElement = driver.FindElement(By.Id(Locator.giftTypeDrop));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    
                    if (giftTypeElement != null)
                    {
                        IWebElement appliedFilterHeader = driver.FindElement(By.XPath(Locator.appliedFilterHeaderPath));
                        int initialcount = 0;
                        int updatedcount = 0;

                        try
                        {
                            string text = appliedFilterHeader.Text;
                            initialcount = int.Parse(text.Substring(0, text.IndexOf(' ')));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }

                        // child element is input type and not clickable. So I reach parent element and click on it to expand dropdown.
                        IWebElement parent = (IWebElement)(js.ExecuteScript(
                                       "return arguments[0].parentNode;", giftTypeElement));
                        
                        parent.ClickAction(driver);
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.giftTypeDropSubOptions)));
                        IList<IWebElement> list = driver.FindElements(By.XPath(Locator.giftTypeDropSubOptions));
                        if (list.Count == 3) // this means we have both eGift and Gift Card available
                        {
                            string filterText = "";
                            int count = list.Count;
                            
                            for (int i = 1; i < count; i++)
                            {
                                // reset logger
                                result.log = "";
                                result.logType = LogType.SUCCESS;

                                if (i==2) //in case the child elements removed from display! We need to bring them back to display
                                {
                                    giftTypeElement = driver.FindElement(By.Id(Locator.giftTypeDrop)); 
                                    parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", giftTypeElement));
                                    parent.ClickAction(driver);
                                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.giftTypeDropSubOptions)));
                                    list = driver.FindElements(By.XPath(Locator.giftTypeDropSubOptions));                                 
                                }
                                filterText = list[i].Text;
                                list[i].ClickAction(driver);
                                Thread.Sleep(TimeSpan.FromSeconds(3));
                                IWebElement appliedFilterHeaderUpdated = driver.FindElement(By.XPath(Locator.appliedFilterHeaderPath));
                                IWebElement appliedFilterName = driver.FindElement(By.XPath(Locator.appliedFilterNamePath));
                                if(filterText != appliedFilterName.Text)
                                {
                                    result.logType = LogType.FAIL;
                                    result.log += System.Environment.NewLine + "Applied filter and the selected dropdown value do not match." + " - " +
                                                 "Dropdown value: " + filterText + " -> " + "Applied Filter Value: " + appliedFilterName.Text;
                                }
                                else
                                {
                                    result.log += System.Environment.NewLine + "Applied filter being reflected on filter area text.";
                                }
                                try
                                {
                                    string text = appliedFilterHeaderUpdated.Text;
                                    updatedcount = int.Parse(text.Substring(0, text.IndexOf(' ')));
                                    if (updatedcount == initialcount)
                                    {
                                        result.logType = LogType.FAIL;
                                        result.log += System.Environment.NewLine + "Gift type filter applied, but products displayed haven't changed. Applied filter: " + list[i].Text  + " - " + 
                                            "Product count before filter applied: " + initialcount + " - " +
                                            "Product count after filter applied: " + updatedcount;
                                    }
                                    else
                                    {
                                        result.log += System.Environment.NewLine + "Gift type filter applied successfully. Applied filter: " + filterText + " - " +
                                            "Product count before filter applied: " + initialcount + " - " +
                                            "Product count after filter applied: " + updatedcount;
                                    }
                                    insertLog(test,driver,result,true);
                                }
                                catch (Exception exm)
                                {
                                    Console.WriteLine(exm.Message + Environment.NewLine + exm.StackTrace);
                                    result = new checkResult();
                                    result.logType = LogType.FAIL;
                                    result.log = exm.Message + Environment.NewLine + exm.StackTrace;
                                    insertLog(test,driver,result,true);
                                    Assert.IsFalse(true);
                                }
                            }
                            // Resetting gift type filter to "All"
                            giftTypeElement = driver.FindElement(By.Id(Locator.giftTypeDrop));
                            parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", giftTypeElement));
                            parent.ClickAction(driver);
                            wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.giftTypeDropSubOptions)));
                            list = driver.FindElements(By.XPath(Locator.giftTypeDropSubOptions));
                            list[0].ClickAction(driver); // first option is generally "select ALL". That's why we are selecting it.
                            Thread.Sleep(TimeSpan.FromSeconds(3));
                        }
                    }
                }
                else
                {
                    isFilterEnabled = false;
                    result.log = "Filter section is not enabled for the site. Skipping gift type filter selection tests.";
                    result.logType = LogType.INFO;
                    insertLog(test,driver,result,false);
                }
            }
            catch(Exception exm)
            {
                Console.WriteLine(exm.Message + Environment.NewLine + exm.StackTrace);
                checkResult result = new checkResult();
                result.logType = LogType.FAIL;
                result.log = exm.Message + Environment.NewLine + exm.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(false);
            }
        }
        public void filterSection_SortBySelection()
        {
            checkResult result = new checkResult();
            try
            {
                if (isFilterEnabled && filterSection_filterVisible() != null)
                {
                    //ePath path = new ePath { identifier = sortByDrop, pathType = HelperMethods.Type.ID };
                    //ePath optionsPath = new ePath { identifier = sortByDropSubOptions, pathType = HelperMethods.Type.XPath };

                    var sortByElement = driver.FindElement(By.Id(Locator.sortByDrop));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    if (sortByElement != null)
                    {
                        // child element is input type and not clickable. So I reach parent element and click on it to expand dropdown.
                        IWebElement parent = (IWebElement)(js.ExecuteScript(
                                       "return arguments[0].parentNode;", sortByElement));
                        parent.ClickandHoldAction(driver);
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                        wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.sortByDropSubOptions)));
                        IList<IWebElement> list = driver.FindElements(By.XPath(Locator.sortByDropSubOptions));
                        string filterText = "";
                        int count = list.Count;

                        for (int i = 0; i < count; i++)
                        {
                            //resetting logger
                            result.log = "";
                            result.logType = LogType.SUCCESS;
                            if (i > 0)
                            {
                                sortByElement = driver.FindElement(By.Id(Locator.sortByDrop));
                                parent = (IWebElement)(js.ExecuteScript(
                                       "return arguments[0].parentNode;", sortByElement));
                                parent.ClickandHoldAction(driver);
                            }
                            list = driver.FindElements(By.XPath(Locator.sortByDropSubOptions));
                            filterText = list[i].Text;
                            list[i].ClickAction(driver);
                            Thread.Sleep(TimeSpan.FromSeconds(3));

                            //Check whether the selected option is reflected
                            IWebElement optionCore = driver.FindElement(By.XPath("//*[@id='react-select-2-input']"));
                            IWebElement selectedOption = (IWebElement)(js.ExecuteScript(
                                       "return arguments[0].parentNode;", sortByElement));
                            if (filterText == selectedOption.Text) result.log += "Sort By option selection successful.";

                            // Check the sort order of items
                            if (i == 1 || i == 2) // A-Z Sorting
                            {
                                IList<IWebElement> productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                                List<string> x = new List<string>();
                                var alphabetical = true;
                                string previous = null;
                                int counter = 0;
                                foreach (var item in productList)
                                {
                                    string btnText = item.Text.Replace(Environment.NewLine, "");
                                    var current = item.Text.Replace(Environment.NewLine, "");
                                    x.Add(current);
                                    if (previous != null && StringComparer.Ordinal.Compare(previous, current) > 0)
                                    {
                                        alphabetical = false;
                                    }
                                    previous = current;
                                    counter++;
                                    if (counter == 10) break;
                                }
                                if (i == 1)
                                {
                                    if (alphabetical == false)
                                    {
                                        result.log += " A-Z sorting applied but products are not sorted from A to Z.";
                                        result.logType = LogType.FAIL;
                                    }
                                    else
                                    {
                                        result.log += " A-Z sorting successfully applied and working.";
                                    }
                                }
                                else if (i == 2)
                                {
                                    if (alphabetical == false)
                                    {
                                        result.log += " Z-A sorting successfully applied and working.";
                                    }
                                    else
                                    {
                                        result.log += " Z-A sorting applied but products are not sorted from Z to A.";
                                        result.logType = LogType.FAIL;
                                    }
                                }
                            }
                            insertLog(test,driver,result,true);
                        }
                        //resetting sort by
                        sortByElement = driver.FindElement(By.Id(Locator.sortByDrop));
                        parent = (IWebElement)(js.ExecuteScript(
                               "return arguments[0].parentNode;", sortByElement));
                        parent.ClickandHoldAction(driver);
                        list = driver.FindElements(By.XPath(Locator.sortByDropSubOptions));
                        filterText = list[0].Text;
                        list[0].ClickAction(driver);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                    else
                    {
                        result.log = " Sort By filter is not available within filters section. Skipping testing it.";
                        insertLog(test,driver,result,false);
                    }
                }
                else 
                { 
                   result.log = "Filter section is not enabled for the site. Skipping Sort By Filter tests.";
                   result.logType = LogType.INFO;
                   insertLog(test,driver,result,false);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log = ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(true);
            }
        }
        public void filterSection_Search()
        {
            checkResult result = new checkResult();
            try
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                if (isFilterEnabled && filterSection_filterVisible() != null)
                {
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
                    IWebElement searchElement = driver.FindElement(By.CssSelector(Locator.searchPath));
                    IWebElement searchButton;
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    if(searchElement != null)
                    {
                        // search with product name - finding a product name
                        string searchtext = "";
                        string productText = "";
                        int counter = 0;
                        IList<IWebElement> productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                        if (productList != null && productList.Count > 0)
                        {
                            searchtext = productList[0].Text.Replace(Environment.NewLine, ""); // pick the first 
                        }
                        searchElement.SendKeys(searchtext);
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        List<string> x = new List<string>();                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
                        productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                        if (productList != null && productList.Count > 0)
                        {
                            result.logType = LogType.FAIL;
                            foreach(var item in productList)
                            {
                                productText = item.Text.Replace(Environment.NewLine, "");
                                if(productText.Contains(searchtext))
                                {
                                    result.logType = LogType.SUCCESS;
                                    result.log += "Search filter applied successfully. Search text : " + searchtext + ".";
                                    break;
                                }
                                counter++;
                                if (counter == 10) break;
                            }
                            if (result.logType == LogType.FAIL) result.log += "Product search filter not working as expected.";
                            insertLog(test,driver,result,true);
                        }
                        // clear search field first
                       while(!String.IsNullOrEmpty(searchElement.GetAttribute("value")))
                        {
                            searchButton = driver.FindElement(By.CssSelector(Locator.searchButtonPath));
                            searchButton.ClickAction(driver);
                            searchElement = driver.FindElement(By.CssSelector(Locator.searchPath));
                            string text = searchElement.GetAttribute("value").ToString();
                            for (int i = 0 ; i < text.Length; i++)
                            {
                                searchElement.SendKeys(Keys.Backspace);
                            }
                        }

                        result.log = "";
                        result.logType = LogType.SUCCESS;
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass)); // pulling products
                        searchtext = productList[1].Text.Replace(Environment.NewLine, ""); // pick the second 
                        searchElement.SendKeys(searchtext); // search by the second product
                        //since this action will filter out products, we are checking the displayed list again
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                        x = new List<string>();
                        productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                        if (productList != null && productList.Count > 0)
                        {
                            foreach (var item in productList)
                            {
                                productText = item.Text.Replace(Environment.NewLine, "");
                                if (!productText.Contains(searchtext))
                                {
                                    result.logType = LogType.FAIL;
                                    result.log += " Product with a different name than the searched one returned.";
                                }
                                counter++;
                                if (counter == 10) break;
                            }
                        }
                    }
                    if (result.logType == LogType.SUCCESS) result.log += " Search field performs as expected.";
                    insertLog(test,driver,result,true);

                    //clear filtering
                    while (!String.IsNullOrEmpty(searchElement.GetAttribute("value")))
                    {
                        searchButton = driver.FindElement(By.CssSelector(Locator.searchButtonPath));
                        searchButton.ClickAction(driver);
                        searchElement = driver.FindElement(By.CssSelector(Locator.searchPath));
                        string text = searchElement.GetAttribute("value").ToString();
                        for (int i = 0; i < text.Length; i++)
                        {
                            searchElement.SendKeys(Keys.Backspace);
                        }
                    }
                }
                else
                {
                    result.log = "Filter section is not enabled for the site. Skipping Sthe Search By Filter tests.";
                    result.logType = LogType.INFO;
                    insertLog(test,driver,result,false);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log = ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(true);
            }
        }
        public void ViewAddCodesTest()
        {
            checkResult result = new checkResult();
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                IList<IWebElement> viewAddCode = driver.FindElements(By.XPath(Locator.viewAddCodes));

                if (viewAddCode.Count != 0)
                {
                    var viewAddCodesElement = driver.FindElement(By.XPath(Locator.viewAddCodes));
                    if (viewAddCodesElement.Enabled)
                        viewAddCodesElement.ClickAction(driver);
                    //Check if View/Add Codes panel opened. 
                    IList<IWebElement> viewCodesPanel = driver.FindElements(By.CssSelector(Locator.viewAddCodesPanel));
                    if (viewCodesPanel.Count > 0 && viewCodesPanel[0].Displayed)
                    {
                        result.log = "View/Add Codes panel exists. Opened and taken a screenshot";
                        result.logType = LogType.INFO;
                        insertLog(test, driver, result, false);
                        if (driver.FindElements(By.CssSelector(Locator.AddCodesFieldPath)).Count() <= 0)
                        {
                            result.log = "Add Codes section within View/Add Codes panel is not enabled. Skipping 'add code' tests under View/Add Codes Panel. Check screenshot!";
                            result.logType = LogType.WARNING;
                            insertLog(test, driver, result, true);
                        }
                        else
                        {
                            var AddCodeInputField = driver.FindElement(By.CssSelector(Locator.AddCodesFieldPath));
                            if (AddCodeInputField != null)
                            {
                                AddCodeInputField.SendKeys("DEMO1111");
                                var addButton = driver.FindElement(By.CssSelector("button.addCodeButton"));
                                if (addButton != null) addButton.Click();
                                //Check whether info popup opens successfully
                                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.NotificationBoxPath)));
                                var notificationBox = driver.FindElement(By.XPath(Locator.NotificationBoxPath));
                                var notificationTitle = driver.FindElement(By.XPath(Locator.NotificationBoxTitle_Path)).Text;
                                var notificationMessage = driver.FindElement(By.XPath(Locator.NotificationBoxMessage_Path)).Text;

                                if (notificationMessage != null)
                                {
                                    result.log = " Add code works as expected. Message given via toast message : " +
                                                  notificationTitle + " - " + notificationMessage;
                                }
                                insertLog(test, driver, result, true);
                                Console.WriteLine(result.log);
                                // removal of latest added code
                                IList<IWebElement> list = driver.FindElements(By.CssSelector("div.codeLineItem"));
                                int count = list.Count + 1;
                                IList<IWebElement> list2 = driver.FindElements(By.CssSelector("#appTopBarPortal > div > div:nth-child(2) > div > div.viewCodes > div > div.viewCodesPanel.open > div.viewCodesBottom > div.codesArea > div:nth-child(" + count + ") > div.first-row > button"));
                                IWebElement element = driver.FindElement(By.CssSelector("#appTopBarPortal > div > div:nth-child(2) > div > div.viewCodes > div > div.viewCodesPanel.open > div.viewCodesBottom > div.codesArea > div:nth-child(" + count + ") > div.first-row > button"));
                                element.ClickAction(driver);
                                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.NotificationBoxPath)));
                                notificationBox = driver.FindElement(By.XPath(Locator.NotificationBoxPath));
                                notificationTitle = driver.FindElement(By.XPath(Locator.NotificationBoxTitle_Path)).Text;
                                notificationMessage = driver.FindElement(By.XPath(Locator.NotificationBoxMessage_Path)).Text;
                                if (notificationMessage != null)
                                {
                                    result.log = " Remove code works as expected. Message given via toast message : " +
                                                  notificationTitle + " - " + notificationMessage;
                                }
                                insertLog(test, driver, result, true);
                                // Closing View Add Codes
                                driver.FindElement(By.CssSelector(Locator.ViewAddCodes_CloseButtonPath)).ClickAction(driver);
                            }
                        }
                    }
                    else
                    {
                        result.log = "View/Add Codes Button is found and clicked but panel didn't open.";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                }
                else
                {
                    result.log = "View/Add Codes Panel is not enabled for the site. Skipping tests for displaying/adding codes.";
                    result.logType = LogType.WARNING;
                    insertLog(test, driver, result, true);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log =  ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(true);
            }
        }
        public void AddProducttoBasket(ref double orderValue)
        {
            checkResult result = new checkResult();
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                bool filterState;
                // determining site design
                IList<IWebElement> slickContainer = driver.FindElements(By.CssSelector(Locator.OldDesignIndicatorPath));
                if (slickContainer.Count <= 0)
                {
                    //---- New Design -------------
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector("#tilesContainer > div")));
                    // Filtering and adding an eGift product if found
                    filterState = FilterByGiftType(js, GiftType.eGift);
                    AddProduct(result, js, ref orderValue);
                    // Filtering and adding an Gift Card product if found
                    filterState = FilterByGiftType(js, GiftType.GiftCard);
                    if (filterState) AddProduct(result, js,ref orderValue); //Add only if filtering is enabled. Otherwise, adding one product is ok.                 
                                                             // Check if we can checkout with the current balance or need to finish the whole balance?
                }
                else
                {
                    // Check remaining value and find products with denominations below this value
                    double remainingValue = 0;
                    double maximumOrderLimit = 0;
                    orderValue = 0;

                    //Check whether there is maximumOrderLimit
                    IList<IWebElement> orderLimit = driver.FindElements(By.CssSelector(Locator.MaximumOrderLimitPath));
                    if(orderLimit.Count>0) maximumOrderLimit = double.Parse(Extensions.extractMaximum(orderLimit[0].Text)); // there is a max order limit which we need to check

                    IList<IWebElement> slickFind = driver.FindElements(By.XPath(Locator.SlickVisibleProductsPath)); // pick all visible products
                    if (slickFind.Count > 0)
                    {
                        bool minTested = false,midTested = false,maxTested = false,customTested = false;

                        for (int i = 0; i < slickFind.Count; i++) //check available product(s) within the remaining balance scope
                        {
                            try
                            {
                                slickFind[i].ClickAction(driver);
                            }
                            catch(StaleElementReferenceException el)
                            {
                                slickFind = driver.FindElements(By.XPath(Locator.SlickVisibleProductsPath));
                                if(slickFind.Count > 0)
                                {
                                    slickFind[i].ClickAction(driver);
                                }    
                            }
                            string SlickDenominationSection = "div.slick-container > div > div > div > div:nth-child(" + (i + 1) + ") > div > div > div > div.tab.open > div > div > div.select-value-buttons.three-options";
                            string SlickAddProductButton = "#catalogRoot > div > div > div.slick-container > div > div > div > div:nth-child(" + (i + 1) + ") > div > div > div > div.tab.open > div > button.addToBasket";
                            string SlickProductCloseButton = "#catalogRoot > div > div > div.slick-container > div > div > div > div:nth-child(" + (i + 1) + ") > div > div > div > div.tab.open > button";
                            string CustomValueField = "#catalogRoot > div > div > div.slick-container > div > div > div > div:nth-child(" + (i + 1) + ") > div > div > div > div.tab.open > div > div > div.custom-value-area > input";
                            string CustomValueWarningMessage = "#catalogRoot > div > div > div.slick-container > div > div > div > div:nth-child(" + (i + 1) + ") > div > div > div > div.tab.open > div > div > div.custom-message-area>p";
                            // Check if denomination boxes visible/available
                            if (driver.FindElements(By.CssSelector(SlickDenominationSection)).Count() > 0)
                            {
                                var section = driver.FindElement(By.CssSelector(SlickDenominationSection));
                                //pick the minimum value
                                var elementMin = section.FindElement(By.CssSelector(Locator.SlickDenomsMinimumValuePath));
                                var elementMid = section.FindElement(By.CssSelector(Locator.SlickDenomsMediumValuePath));
                                var elementMax = section.FindElement(By.CssSelector(Locator.SlickDenomsMaximumValuePath));
                                double minDenomValue = double.Parse(elementMin.GetAttribute("value"));
                                double midDenomValue = double.Parse(elementMid.GetAttribute("value"));
                                double maxDenomValue = double.Parse(elementMax.GetAttribute("value"));
                                IWebElement remaining;
                                if (driver.FindElements(By.CssSelector(Locator.RemainingValuePath)).Count() > 0)
                                {
                                    remaining = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                                    remainingValue = double.Parse(remaining.Text.Remove(0, 1));
                                    if (remainingValue > 0 && (orderValue == 0 || (orderValue > 0 && maximumOrderLimit > 0 && orderValue.CompareTo(maximumOrderLimit) < 0)))
                                    {
                                        testDenom(ref remainingValue, ref orderValue, maximumOrderLimit,ref minTested, SlickAddProductButton, elementMin, minDenomValue, result); // testing min. denom
                                        testDenom(ref remainingValue, ref orderValue, maximumOrderLimit,ref midTested, SlickAddProductButton, elementMid, midDenomValue, result); // testing mid. denom
                                        testDenom(ref remainingValue, ref orderValue, maximumOrderLimit, ref maxTested, SlickAddProductButton, elementMax, maxDenomValue, result); // testing max. denom
                                        testCustomField(ref remainingValue, ref orderValue, maximumOrderLimit, customTested, SlickAddProductButton, CustomValueField, CustomValueWarningMessage, result); // testing custom value
                                        //Check if the basket automatically opens in the end
                                        if ((maximumOrderLimit > 0 && orderValue.CompareTo(maximumOrderLimit) == 0) || remainingValue == 0)
                                        {
                                            try
                                            {
                                                IWebElement basketPanel = driver.FindElement(By.CssSelector(Locator.DisplayedBasketPath));
                                                if (basketPanel.Displayed)
                                                {
                                                    result.log = " Basket displayed automatically after products added";
                                                    result.logType = LogType.INFO;
                                                    insertLog(test,driver,result,true);
                                                    break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                result.log = " An element couldn't be detected. Error details" + ex.Message + " - " + ex.StackTrace;
                                                result.logType = LogType.FATAL;
                                                insertLog(test,driver,result,false);
                                            }
                                        }
                                    }
                                    // Close Product Panel
                                    IList<IWebElement> buttonSearch = driver.FindElements(By.CssSelector(SlickProductCloseButton));
                                    if (buttonSearch.Count() > 0)
                                    {
                                        buttonSearch[0].ClickAction(driver);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log = ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(true);
            }
        }

        public void testCustomField(ref double remainingValue, ref double orderVal, double maxOrderLimit, bool customTested, string SlickAddProductButton, string CustomValueField, string CustomValueWarningMessage, checkResult result)
        {
            double customValue = 0;
            double allowedMaximum = 0;
            if (!customTested)
            {
                IList<IWebElement> customFind = driver.FindElements(By.CssSelector(CustomValueField));
                if (customFind.Count > 0)
                {
                    while (remainingValue > 0 && (orderVal == 0 || ((maxOrderLimit > 0 && orderVal.CompareTo(maxOrderLimit) != 0) || maxOrderLimit == 0)))
                    {
                        customValue = remainingValue;
                        customFind[0].SendKeys(remainingValue.ToString());
                        if (maxOrderLimit > 0 && (customValue + orderVal).CompareTo(maxOrderLimit) > 0)
                        {
                            result.log = "Custom Value is set above the maximum order limit. A warning message to be displayed";
                            result.logType = LogType.WARNING;
                            insertLog(test, driver, result, true);
                        }
                        IWebElement message;
                        //is there a warning regarding the input value
                        while (driver.FindElements(By.CssSelector(CustomValueWarningMessage)).Count() > 0)
                        {
                            message = driver.FindElement(By.CssSelector(CustomValueWarningMessage));
                            string messageText = message.Text;
                            allowedMaximum = double.Parse(Extensions.extractMaximum(messageText));
                            if ((allowedMaximum + orderVal).CompareTo(maxOrderLimit) == 0)
                                customValue = allowedMaximum;
                            else
                                continue;
                            for (int j = 0; j <= customFind[0].GetAttribute("value").Length; j++)
                            {
                                customFind[0].SendKeys(Keys.Backspace);
                                if (customFind[0].GetAttribute("value").Length == 0) break;
                            }
                            customFind[0].SendKeys(customValue.ToString()); // adjusted custom value placed.
                        }
                        IWebElement addButton = null;
                        try
                        {
                          addButton= driver.FindElement(By.CssSelector(Locator.AddProductButton));
                        }
                        catch(Exception ex)
                        {
                            string alternativeAddButtonPath = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > button";
                            addButton = driver.FindElement(By.CssSelector(alternativeAddButtonPath));
                        }
                        addButton.ClickAction(driver);
                        result.log = " Product added to the basket via custom value field. Product Value : " + customValue;
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                        orderVal += customValue;
                        IWebElement remaining = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                        remainingValue = double.Parse(remaining.Text.Remove(0, 1));
                    }
                }
            }
        }

        private void testDenom(ref double remainingValue, ref double orderVal, double maxOrderLimit, ref bool denomTested, string addButtonPath, IWebElement elementDenom, double denomValue, checkResult result)
        {
            string closeButtonPath = "body > div.ReactModalPortal > div > div > div.modal-close";
            string modalIdentifier = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap";
            IList<IWebElement> modalCheck;
            if (orderVal > 0 && (maxOrderLimit > 0 && orderVal.CompareTo(maxOrderLimit) >= 0))
            {
                result.log = "Maximum order value is reached.";
                if ((denomValue + orderVal).CompareTo(maxOrderLimit) > 0 && elementDenom.Enabled)
                {
                    result.log += denomValue + " denomination shouldn't be available because of the order value limit";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                    modalCheck = driver.FindElements(By.CssSelector(modalIdentifier));
                    if (modalCheck.Count > 0) driver.FindElement(By.CssSelector(closeButtonPath)).ClickAction(driver); //close modal
                    denomTested = true;
                }
            }
            else if (!denomTested && denomValue.CompareTo(remainingValue) <= 0 && (maxOrderLimit == 0 || (denomValue.CompareTo(maxOrderLimit) <= 0 && (denomValue + orderVal).CompareTo(maxOrderLimit) <= 0)))
            {
                if (!elementDenom.Enabled)//check if denom element is enabled:
                {
                    result.log = denomValue + " denomination should normally be enabled.";
                    result.logType = LogType.FAIL;
                    insertLog(test, driver, result, true);
                    modalCheck = driver.FindElements(By.CssSelector(modalIdentifier));
                    if (modalCheck.Count > 0) driver.FindElement(By.CssSelector(closeButtonPath)).ClickAction(driver);
                    denomTested = true;
                }
                else
                    elementDenom.ClickAction(driver);
                //check if Add Product Button available
                IWebElement addButton;
                try
                {
                   addButton = driver.FindElement(By.CssSelector(addButtonPath));
                }
                catch(Exception el)
                {
                    //Alternative button path for modal display
                    addButton = driver.FindElement(By.CssSelector("body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > button"));
                }
                addButton.ClickAction(driver);
                result.log = "Product added to the basket via denom box. Product Value : " + denomValue;
                result.logType = LogType.SUCCESS;
                insertLog(test, driver, result, false);
                orderVal += denomValue;
                IWebElement remaining = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                remainingValue = double.Parse(remaining.Text.Remove(0, 1));
                denomTested = true;
                Thread.Sleep(TimeSpan.FromSeconds(3));
                modalCheck = driver.FindElements(By.CssSelector(modalIdentifier));                
                if (modalCheck.Count > 0) driver.FindElement(By.CssSelector(closeButtonPath)).ClickAction(driver);
            }
            else
            {
                if (denomValue.CompareTo(remainingValue) > 0)
                {

                    if (elementDenom.Enabled)
                    {
                        result.log = "Denomination greater than the remaining value is available.";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                        denomTested = true;
                        return;
                    }


                    // check the tooltip text for the denomination
                    Actions builder = new Actions(driver);
                    builder.MoveToElement(elementDenom);
                    IAction mouseHover = builder.Build();
                    mouseHover.Perform();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    IWebElement parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", elementDenom));
                    IList<IWebElement> denomTooltips = parent.FindElements(By.CssSelector("span.tooltip-text"));
                    if (denomTooltips.Count <= 0)
                    {
                        result.log = "Disabled denom tooltip is not displayed";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, false);
                    }
                    else
                    {
                        string tooltipText = denomTooltips[0].GetAttribute("innerHTML");
                        result.log = " Tooltip is displayed for disabled denom : " + denomValue + ". Text: " + tooltipText;
                        result.logType = LogType.SUCCESS;
                        insertLog(test, driver, result, false);
                    }
                    denomTested = true;
                }
                if (maxOrderLimit > 0 && denomValue.CompareTo(maxOrderLimit) > 0)
                {
                    if (elementDenom.Enabled)
                    {
                        result.log = "Denomination greater than the maximum order limit value is available.";
                        result.logType = LogType.FAIL;
                        insertLog(test, driver, result, true);
                    }
                    denomTested = true;
                }
            }
        }
        public void CheckoutProcess(double orderTotal)
         {
            checkResult result = new checkResult();
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                if (filterSection_filterVisible() != null)
                {
                    //check Gift Type selection to be set to "All" -- NOTE: can't remember why doing this. Delete if it's not necessary
                    IWebElement giftTypeElement = driver.FindElement(By.Id(Locator.giftTypeDrop));
                    IWebElement parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", giftTypeElement));
                    parent.ClickAction(driver);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.giftTypeDropSubOptions)));
                    IList<IWebElement> list = driver.FindElements(By.XPath(Locator.giftTypeDropSubOptions));
                    list[0].ClickAction(driver); // first option is generally "select ALL". That's why we are selecting it.
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
                IWebElement basket;

                if (driver.FindElements(By.CssSelector(Locator.DisplayedBasketPath)).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(3));
                if (driver.FindElements(By.CssSelector(Locator.DisplayedBasketPath)).Count() == 0) // is basket displayed now?
                {
                    IWebElement myBasket = driver.FindElement(By.CssSelector(Locator.OpenBasketButtonPath));
                    myBasket.ClickAction(driver);
                }
                basket = driver.FindElement(By.CssSelector(Locator.DisplayedBasketPath));
                if (basket != null)
                {
                    //check whether the basket total reflected is correct
                    IWebElement totalArea = driver.FindElement(By.CssSelector(Locator.BasketTotalPath));
                    double totalVal = double.Parse(totalArea.Text.Remove(0, 1));
                    if(totalVal.CompareTo(orderTotal) == 0)
                    {
                        result.log = "Order total is reflected successfully on basket view";
                        result.logType = LogType.SUCCESS;
                        insertLog(test,driver,result,false);
                    }
                    IWebElement checkOutButton = driver.FindElement(By.CssSelector(Locator.BasketCheckoutButtonPath));
                    if (checkOutButton.Enabled)
                    {
                        checkOutButton.ClickAction(driver);
                        result.log = " Basket > Checkout is performed successfully";
                        result.logType = LogType.SUCCESS;
                        insertLog(test,driver,result,false);
                    }
                    else
                    {
                        // checkout is not enabled for a reason.
                        // check the remaining balance and add a product with that value for checkout
                        driver.FindElement(By.CssSelector(Locator.CloseBasketButtonPath)).ClickAction(driver);
                        int remainingValue = int.Parse(driver.FindElement(By.CssSelector("#chart-label")).Text.Remove(0, 1));
                        AddProduct(result, js, remainingValue);
                        if (driver.FindElements(By.CssSelector(Locator.DisplayedBasketPath)).Count() == 0)
                        {
                            IWebElement myBasket = driver.FindElement(By.CssSelector(Locator.OpenBasketButtonPath));
                            myBasket.ClickAction(driver);
                        }
                        basket = driver.FindElement(By.CssSelector(Locator.DisplayedBasketPath));
                        if (basket != null)
                        {
                            checkOutButton = driver.FindElement(By.CssSelector(Locator.BasketCheckoutButtonPath));
                            if (checkOutButton.Enabled)
                            {
                                checkOutButton.ClickAction(driver);
                                if(driver.FindElements(By.CssSelector(Locator.ConfirmYourOrderPagePath)).Count()>0)
                                    result.log = "Checkout process completed successfully.";
                                else
                                {
                                    result.log = "Checkout process failed.";
                                    result.logType = LogType.FAIL;
                                }
                            }
                            insertLog(test,driver,result,true);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log += ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsTrue(false);
            }
        }
        private bool FilterByGiftType(IJavaScriptExecutor js, GiftType giftType)
        {
            checkResult result = new checkResult();
            try
            {
                if (filterSection_filterVisible() != null)
                {
                    var giftTypeElement = driver.FindElement(By.Id(Locator.giftTypeDrop));
                    IWebElement parentGT = (IWebElement)(js.ExecuteScript(
                                   "return arguments[0].parentNode;", giftTypeElement));
                    parentGT.ClickandHoldAction(driver);
                    IList<IWebElement> listGT = driver.FindElements(By.XPath(Locator.giftTypeDropSubOptions));
                    if (listGT != null)
                    {
                        if (giftType == GiftType.eGift)
                            listGT[1].ClickAction(driver); // apply eGift Type
                        else if (giftType == GiftType.GiftCard)
                            listGT[2].ClickAction(driver);
                        Thread.Sleep(TimeSpan.FromSeconds(3));
                    }
                    return true;
                }
                return false;
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
        private void AddProduct(checkResult result, IJavaScriptExecutor js,ref double orderValue)
        {
            try
            {
                IWebElement parent;
                if (driver.FindElements(By.XPath(Locator.A_Z_SortingClass)).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(3));
                else
                {
                    IList<IWebElement> productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                    if (productList.Count > 0)
                    {
                        bool priceRangeAvailable = true;
                        IWebElement remainingVal = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                        double remainingValue = double.Parse(remainingVal.Text.Remove(0, 1));
                     

                        bool minTested = false, midTested = false, maxTested = false, customTested = false;
                        double maximumOrderLimit = 0;
                        // check for an available product
                        IList<IWebElement> orderLimit = driver.FindElements(By.CssSelector(Locator.MaximumOrderLimitPath));
                        if (orderLimit.Count > 0) maximumOrderLimit = double.Parse(Extensions.extractMaximum(orderLimit[0].Text));
                        foreach (IWebElement element in productList)
                        {
                            parent = (IWebElement)(js.ExecuteScript("return arguments[0].parentNode;", element));
                            if (priceRangeAvailable && parent.FindElements(By.CssSelector(".price-range")).Count > 0)
                            {
                                priceRangeAvailable = true;
                                IWebElement priceRange = parent.FindElement(By.CssSelector(".price-range"));
                                string priceText = priceRange.Text;
                                string[] prices = priceText.Split('-');
                                double minValue = double.Parse(prices[0].Remove(0, 1));
                                // check available balance
                                if (remainingValue.CompareTo(minValue)<0) //switch to the next product
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (priceRangeAvailable)
                                {
                                    result.log = " Price ranges are not displayed along with the products. Products will be added randomly according to availability";
                                    result.logType = LogType.WARNING;
                                    insertLog(test, driver, result, false);
                                    priceRangeAvailable = false;
                                }
                            }
                            parent.ClickAction(driver); //open product panel
                            result.log = "Product Popup Menu Opened successfully.";
                            result.logType = LogType.SUCCESS;
                            insertLog(test,driver,result,false);

                            if (driver.FindElements(By.CssSelector(Locator.DenominationSection)).Count() > 0)
                            {
                                var section = driver.FindElement(By.CssSelector(Locator.DenominationSection));
                                //pick the minimum value
                                var elementMin = section.FindElement(By.CssSelector(Locator.DenomsMinimumValuePath));
                                var elementMid = section.FindElement(By.CssSelector(Locator.DenomsMediumValuePath));
                                var elementMax = section.FindElement(By.CssSelector(Locator.DenomsMaximumValuePath));
                                double minDenomValue = double.Parse(elementMin.GetAttribute("value"));
                                double midDenomValue = double.Parse(elementMid.GetAttribute("value"));
                                double maxDenomValue = double.Parse(elementMax.GetAttribute("value"));
                                IWebElement remaining;
                                if (driver.FindElements(By.CssSelector(Locator.RemainingValuePath)).Count() > 0)
                                {
                                    remaining = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                                    remainingValue = double.Parse(remaining.Text.Remove(0, 1));
                                    if (remainingValue > 0 && (orderValue == 0 || ((maximumOrderLimit > 0 && orderValue.CompareTo(maximumOrderLimit) < 0) || maximumOrderLimit == 0)))
                                    {
                                        testDenom(ref remainingValue, ref orderValue, maximumOrderLimit, ref minTested, Locator.AddProductButton, elementMin, minDenomValue, result); // testing min. denom
                                        CheckELementAvailability(parent, ref elementMid, Locator.DenomsMediumValuePath);
                                        testDenom(ref remainingValue, ref orderValue, maximumOrderLimit, ref midTested, Locator.AddProductButton, elementMid, midDenomValue, result); // testing mid. denom
                                        CheckELementAvailability(parent, ref elementMax, Locator.DenomsMaximumValuePath);
                                        testDenom(ref remainingValue, ref orderValue, maximumOrderLimit, ref maxTested, Locator.AddProductButton, elementMax, maxDenomValue, result); // testing max. denom
                                        IList<IWebElement> customFind = driver.FindElements(By.CssSelector(Locator.CustomValueField));
                                        if (customFind.Count <= 0) parent.ClickAction(driver);
                                        testCustomField(ref remainingValue, ref orderValue, maximumOrderLimit, customTested, Locator.AddProductButton, Locator.CustomValueField, Locator.CustomValueWarningMessage, result); // testing custom value
                                        //Check if the basket automatically opens in the end
                                        if ((maximumOrderLimit > 0 && orderValue.CompareTo(maximumOrderLimit) == 0) || remainingValue == 0)
                                        {
                                            try
                                            {
                                                IWebElement basketPanel = driver.FindElement(By.CssSelector(Locator.DisplayedBasketPath));
                                                if (basketPanel.Displayed)
                                                {
                                                    result.log = " Basket displayed automatically after products added";
                                                    result.logType = LogType.INFO;
                                                    insertLog(test, driver, result, true);
                                                    break;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                result.log = " An element couldn't be detected. Error details" + ex.Message + " - " + ex.StackTrace;
                                                result.logType = LogType.FATAL;
                                                insertLog(test, driver, result, false);
                                            }
                                        }
                                    }
                                    // Close Product Panel
                                    IList<IWebElement> buttonSearch = driver.FindElements(By.CssSelector(Locator.ProductCloseButton));
                                    if (buttonSearch.Count() > 0)
                                    {
                                        buttonSearch[0].ClickAction(driver);
                                    }
                                }
                            }
                            else
                            {
                                if (driver.FindElements(By.CssSelector(Locator.ProductPopupValueButtonsMin)).Count() <= 0) Thread.Sleep(TimeSpan.FromSeconds(3));
                                if (driver.FindElements(By.CssSelector(Locator.ProductPopupValueButtonsMin)).Count() <= 0)
                                {
                                    if (driver.FindElements(By.CssSelector(Locator.ProductPopupAddToBasket)).Count() <= 0)
                                    {
                                        result.log = " Product appears to be not available. Will try another product";
                                        result.logType = LogType.WARNING;
                                        insertLog(test, driver, result, false);
                                        driver.FindElement(By.CssSelector("body > div.ReactModalPortal > div > div > div.modal-close")).ClickAction(driver); // close modal
                                        continue;
                                    }

                                }
                                else
                                {
                                    IWebElement DenomBox = driver.FindElement(By.CssSelector(Locator.ProductPopupValueButtonsMin));
                                    double denomValue = double.Parse(DenomBox.GetAttribute("value"));
                                    remainingVal = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                                    remainingValue = double.Parse(remainingVal.Text.Remove(0, 1));
                                    if (remainingValue.CompareTo(denomValue) < 0)
                                    {
                                        if (DenomBox.Enabled)
                                        {
                                            result.log = " Denomination box is enabled although remaining balance not enough for denom. value";
                                            result.logType = LogType.FAIL;
                                            insertLog(test, driver, result, true);
                                        }
                                        else
                                            continue;
                                    }
                                    DenomBox.ClickAction(driver);
                                    driver.FindElement(By.CssSelector(Locator.ProductPopupAddToBasket)).ClickAction(driver); // Add to basket
                                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.NotificationBoxPath)));
                                    var notificationBox = driver.FindElement(By.XPath(Locator.NotificationBoxPath));
                                    var notificationTitle = driver.FindElement(By.XPath(Locator.NotificationBoxTitle_Path)).Text;
                                    var notificationMessage = driver.FindElement(By.XPath(Locator.NotificationBoxMessage_Path)).Text;

                                    if (notificationMessage != null)
                                    {
                                        result.log = " Add product to basket works as expected. Message given via toast message : " +
                                                      notificationTitle + " - " + notificationMessage;
                                        result.logType = LogType.SUCCESS;
                                    }
                                    insertLog(test, driver, result, true);
                                    orderValue += denomValue;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log += ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsTrue(false);
            }

            void CheckELementAvailability(IWebElement parent, ref IWebElement elementDenom, string Path)
            {
                try
                {
                    if (!elementDenom.Displayed) parent.ClickAction(driver);
                }
                catch (Exception el)
                {
                    parent.ClickAction(driver);
                    elementDenom = driver.FindElement(By.CssSelector(Path));
                }
            }
        }
        private void AddProduct(checkResult result, IJavaScriptExecutor js, int productValue)
        {
            try
            {
                IWebElement parent;
                IList<IWebElement> productList = driver.FindElements(By.XPath(Locator.A_Z_SortingClass));
                // check for an available product
                foreach (IWebElement element in productList)
                {
                    parent = (IWebElement)(js.ExecuteScript(
                                      "return arguments[0].parentNode;", element));
                    IWebElement priceRange = parent.FindElement(By.CssSelector(".price-range"));
                    string priceText = priceRange.Text;
                    string[] prices = priceText.Split('-');
                    int minValue = int.Parse(prices[0].Remove(0, 1));
                    // check available balance
                    IWebElement remainingVal = driver.FindElement(By.CssSelector(Locator.RemainingValuePath));
                    int remainingValue = int.Parse(remainingVal.Text.Remove(0, 1));
                    if (remainingValue < minValue) //switch to the next product
                    {
                        continue;
                    }
                    parent.ClickAction(driver); //open product panel
                    IWebElement customInput = driver.FindElement(By.CssSelector(Locator.ProductCustomInputPath));
                    customInput.SendKeys(remainingValue.ToString());
                    IWebElement addButton = driver.FindElement(By.CssSelector(Locator.ProductPopupAddToBasket));
                    if (driver.FindElements(By.CssSelector(Locator.CustomInputErrorMessagePath)).Count() > 0 && !addButton.Enabled)
                    {
                        continue;
                    }
                    else
                    {
                        addButton.ClickAction(driver);
                    }
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
                    wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath(Locator.NotificationBoxPath)));
                    var notificationBox = driver.FindElement(By.XPath(Locator.NotificationBoxPath));
                    var notificationTitle = driver.FindElement(By.XPath(Locator.NotificationBoxTitle_Path)).Text;
                    var notificationMessage = driver.FindElement(By.XPath(Locator.NotificationBoxMessage_Path)).Text;

                    if (notificationMessage != null)
                    {
                        result.log = " Add product to basket via custom input field works as expected. Message given via toast message : " + 
                                      notificationTitle + " - " + notificationMessage;
                    }
                    insertLog(test,driver,result,true);
                    break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                result.logType = LogType.FAIL;
                result.log += ex.Message + " " + ex.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsTrue(false);
            }
        }
        public IWebElement filterSection_filterVisible()
        {
            try
            {              
                IList<IWebElement> filter = null;
                if (driver.FindElements(By.XPath(Locator.filterSection)).Count() <= 0) Thread.Sleep(new TimeSpan(0, 0, 5));
                if (driver.FindElements(By.XPath(Locator.filterSection)).Count() > 0)
                {
                    filter = driver.FindElements(By.XPath(Locator.filterSection));
                    if (filter.Count == 0)
                    {
                        return null;
                    }
                    return filter[0];
                }
                else // still not found.
                    return null;
                
            }
            catch(Exception exm)
            {
                Console.WriteLine(exm.Message + Environment.NewLine + exm.StackTrace);
                checkResult result = new checkResult();
                result.logType = LogType.FAIL;
                result.log = exm.Message + Environment.NewLine + exm.StackTrace;
                insertLog(test,driver,result,true);
                Assert.IsFalse(true);
                return null;
            }
        }       
    }
}
