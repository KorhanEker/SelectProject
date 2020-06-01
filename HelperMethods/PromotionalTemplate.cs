using AventStack.ExtentReports;
using com.sun.org.apache.xerces.@internal.dom;
using com.sun.xml.@internal.messaging.saaj.packaging.mime;
using Microsoft.AspNetCore.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SelectTest.Config;
using SelectTest.PageMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LogType = SelectTest.Config.LogType;

namespace SelectTest.HelperMethods
{
    public class PromotionalTemplate : ReportsGenerationClass
    {
        private IWebDriver driver;
        private ExtentTest test;
        private WebDriverWait wait;

        public Orientation logoOrientation;
        public string logoTitle;
        public string selectedTemplateImageSource;
        public string headerText;
        public string headerFont;
        public int headerSize;
        public bool headerBoldState;
        public string headerFontColour;
        public string headerBackGroundColour;
        public Orientation headerOrientation;
        public string bottomText;
        public string bottomFont;
        public string bottomFontColour;
        public Orientation bottomTextOrientation;
        public string nameText;
        public string nameFont;
        public string nameFontColour;
        public Orientation nameOrientation;



        public PromotionalTemplate(IWebDriver driver, ExtentTest test, WebDriverWait wait)
        {
            this.driver = driver;
            this.test = test ?? throw new ArgumentNullException(nameof(test));
            this.wait = wait;
        }
        /// <summary>
        /// Method captures all detail regarding the template, including fonts, colours, images, texts, selected template cover etc.
        /// </summary>
        public void captureTemplateState()
        {
            checkResult result = new checkResult();
            try
            {
                //logo attrbiutes
                if (driver.FindElements(By.CssSelector(Locator.logoOrientationLeft)).Count() > 0) logoOrientation = Orientation.LEFT;
                else if (driver.FindElements(By.CssSelector(Locator.logoOrientationMiddle)).Count() > 0) logoOrientation = Orientation.CENTER;
                else if (driver.FindElements(By.CssSelector(Locator.logoOrientationRight)).Count() > 0) logoOrientation = Orientation.RIGHT;
                logoTitle = driver.FindElement(By.CssSelector(Locator.logodrop)).GetAttribute("title");
                // template
                selectedTemplateImageSource = driver.FindElement(By.CssSelector(Locator.templateImagePath)).GetAttribute("src");
                // header attributes
                string headerTextStyle = driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).GetAttribute("style");
                string bottomTextStyle = driver.FindElement(By.CssSelector(Locator.bottomMessageText)).GetAttribute("style");
                headerText = driver.FindElement(By.CssSelector(Locator.headerMessageTextArea)).GetAttribute("value");
                headerFont = PromotionalSite.findAttribute("font-family: ", headerTextStyle).Replace("\"", "");
                headerSize = PromotionalSite.extractValueFromText(PromotionalSite.findAttribute("font-size: ", headerTextStyle));
                headerBoldState = PromotionalSite.isBold(headerTextStyle);
                headerFontColour = PromotionalSite.findAttribute("color: ", headerTextStyle).Replace(" ", "");
                headerBackGroundColour = PromotionalSite.findAttribute("background-color: ", headerTextStyle).Replace(" ", "");
                headerOrientation = FindOrientation(headerTextStyle);
                // bottom message attributes
                bottomText = driver.FindElement(By.CssSelector(Locator.bottomMessageText)).GetAttribute("value");
                bottomFont = PromotionalSite.findAttribute("font-family: ", bottomTextStyle).Replace("\"", "");
                bottomFontColour = PromotionalSite.findAttribute("color: ", bottomTextStyle).Replace(" ", "");
                bottomTextOrientation = FindOrientation(bottomTextStyle);
                // name area attributes
                string nameTextStyle = driver.FindElement(By.CssSelector(Locator.nameAreaTextInput)).GetAttribute("style");
                nameText = driver.FindElement(By.CssSelector(Locator.nameAreaTextInput)).GetAttribute("value");
                nameFont = PromotionalSite.findAttribute("font-family: ", nameTextStyle).Replace("\"", "");
                nameFontColour = PromotionalSite.findAttribute("color: ", nameTextStyle).Replace(" ", "");
                nameOrientation = FindOrientation(nameTextStyle); 
    }
            catch (Exception ex)
            {
                result.log = "Exception occurred. Error message : " + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(test, driver, result, false);
            }
        }

        private Orientation FindOrientation(string headerTextStyle)
        {
            string horient = PromotionalSite.findAttribute("text-align: ", headerTextStyle).Replace(" ", "");
            if (horient == "left") return Orientation.LEFT;
            else if (horient == "center") return Orientation.CENTER;
            else if (horient == "right") return Orientation.RIGHT;
            else return Orientation.CENTER;
        }
    }
    
    public enum Orientation
    {
        LEFT = 0,
        CENTER = 1,
        RIGHT = 2
    }
}
