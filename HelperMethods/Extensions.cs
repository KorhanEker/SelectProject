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
using OpenQA.Selenium.Interactions;
using System.Net;

namespace SelectTest.HelperMethods
{
    enum selectorType
    {
        Id = 0,
        XPath = 1,
        CSSSelector = 2,
        Name = 3,
        LinkText = 4,
        ClassName = 5
    }

    public enum TestSource
    {
        Login = 0,
        InvalidLogin = 1,
        ChoiceRedemption = 2,
        PromotionalSite = 3
    }
    public static class Extensions
    {
        public static void ClickAction(this IWebElement element,IWebDriver driver)
        {
            try
            {
                String value = element.GetAttribute("type");
                if (value != null && value.Equals("submit"))
                    element.Submit();
                else
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(3));
                    element.Click();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", element);
            }
        }
        public static void ClickandHoldAction(this IWebElement element, IWebDriver driver)
        {
            try
            {
                Actions action = new Actions(driver);
                action.ClickAndHold(element).Build().Perform();
                //you need to release the control from the test
                //actions.MoveToElement(element).Release();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
                executor.ExecuteScript("arguments[0].click();", element);
            }
        }
        public static bool URLCheckTest(string URL)
        {
            bool urlExist = true;
            Uri urlCheck = new Uri(URL);
            WebRequest request = WebRequest.Create(urlCheck);
            request.Timeout = 15000;
            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (Exception)
            {
                urlExist = false; //url does not exist
            }
            return urlExist;
        }
        static String maximumNum(string curr_num, string res)
        {
            try
            {
                int len1 = curr_num.Length;
                int len2 = res.Length;

                // If both having equal lengths  
                if (len1 == len2 && len1 > 0 && len2 > 0) 
                {
                    // Reach first unmatched character / value  
                    int i = 0;
                    while (curr_num[i] == res[i])
                        i++;

                    // Return string with maximum value  
                    if (curr_num[i] < res[i])
                        return res;
                    else
                        return curr_num;
                }

                // If different lengths  
                // return string with maximum length  
                return len1 < len2 ? res : curr_num;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + " " + ex.StackTrace);
                return string.Empty;
            }
        }
        // Method to extract the maximum value  
        public static string extractMaximum(string str)
        {
            int n = str.Length;
            string curr_num = "";
            string res = "";

            // Start traversing the string  
            for (int i = 0; i < n; i++)
            {
                // Ignore leading zeroes  
                while (i < n && str[i] == '0')
                    i++;

                // Store numeric value into a string  
                while (i < n)
                {
                    if (Char.IsDigit(str[i]))
                        curr_num += str[i];
                    else if (curr_num.Length > 0 && !Char.IsDigit(str[i]))
                        break;
                    i++;
                }

                if (i == n)
                    break;

                if (curr_num.Length > 0)
                    i--;

                // Update maximum string  
                res = maximumNum(curr_num, res);

                curr_num = "";
            }

            // To handle the case if there is only  
            // 0 numeric value  
            if (curr_num.Length == 0 && res.Length == 0)
                res = res + '0';

            // Return maximum string  
            return maximumNum(curr_num, res);
        }

    }
    public enum GiftType
    {
        All = 0,
        eGift = 1,
        GiftCard = 2
    }

    public enum CardType
    {
        VisaCredit = 0,
        VisaDebit = 1,
        VisaPurchasing = 2,
        Mastercard = 3,
        MastercardDebit = 4
    }
}
