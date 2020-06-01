using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelectTest.Config;
using System.Data;
using System.Threading;
using SelectTest.HelperMethods;
using SelectTest.PageMethods;
using System.Security.Permissions;
using System.Reflection;
using System.IO;

namespace SelectTest.TestCases
{
    [TestFixture]
    public class PromotionalTest : ReportsGenerationClass
    {
        PromotionalSite promo;
        PromotionalTemplate template;

        [Test]
        [Category("PromotionalSite")]

        public void test_PromotionalOrder()
        {
            try
            {
                PopulateResource(TestSource.PromotionalSite);
                string url, name, headerMessage, bottomMessage, imagePath, logoOrientation, headerFont, headerTextSize, headerBold, headerTextColour, backgroundColour, headerAlignment;
                string bottomFont, bottomTextColour, bottomAlignment, nameFont, nameTextColour, nameAlignment;
                string validFileName, faultyFileName;
                
                var fullTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["fullTest"]);
                var logoUploadTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["logoUploadTest"]);
                var logoRemoveTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["logoRemoveTest"]);
                var templateSelectionTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["templateSelectionTest"]);
                var headerFormatting = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["headerFormatting"]);
                var bottomFormatting = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["bottomFormatting"]);
                var nameFormatting = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["nameFormatting"]);
                var templateCheck = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["captureTemplate"]);
                var downloadFileTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["downloadFileTest"]);
                var importFileTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["importFileTest"]);
                var manuelRecipientInput = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["manuelRecipientInput"]);
                var amendRecipient = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["amendRecipient"]);
                var addressAutoLookup = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["addressAutoLookup"]);
                var addressManualEntry = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["addressManualEntry"]);
                var dateandtimeTest = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["dateandtimeTest"]);

                foreach (DataRow row in _dtResource.Rows)
                {
                    promo = new PromotionalSite(GetDriver(), GetTest(), GetWait());
                    template = new PromotionalTemplate(GetDriver(), GetTest(), GetWait());
                    url = row["URL"].ToString();
                    name = row["Name"].ToString();
                    headerMessage = row["Header Message"].ToString();
                    bottomMessage = row["Bottom Message"].ToString();
                    imagePath = row["ImagePath"].ToString();
                    logoOrientation = row["Logo Orientation"].ToString();
                    headerFont = row["Header Font"].ToString();
                    headerTextSize = row["Header Text Size"].ToString();
                    headerBold = row["Header Bold"].ToString();
                    headerTextColour = row["Header Text Colour"].ToString();
                    backgroundColour = row["Background Colour"].ToString();
                    headerAlignment = row["Header Alignment"].ToString();
                    bottomFont = row["Bottom Font"].ToString();
                    bottomTextColour = row["Bottom Text Colour"].ToString();
                    bottomAlignment = row["Bottom Alignment"].ToString();
                    nameFont = row["Name Font"].ToString();
                    nameTextColour = row["Name Text Colour"].ToString();
                    nameAlignment = row["Name Alignment"].ToString();
                    validFileName = row["RecipientList Valid File Path"].ToString();
                    faultyFileName = row["RecipientList Invalid File Path"].ToString();

                    bool valid = promo.openSite(url);
                    if (!valid) continue;
                    promo.clickCookieP();
                    promo.orderNowBtnTest();

                    if (logoUploadTest)
                    {
                        promo.insertLogoTest(imagePath, fullTest);
                        promo.logoOrientationTest(logoOrientation, fullTest);
                    }
                    if(logoRemoveTest)
                    { 
                        promo.removeLogo();
                        if (!String.IsNullOrEmpty(imagePath))
                            promo.insertLogo(imagePath);
                        else
                            promo.insertLogo(resourcePath_Images_Valid);
                    }

                    if(templateSelectionTest) promo.templateImageSelectionTest();

                    promo.textInputTest("Header Message", headerMessage, Locator.headerMessageSection, Locator.headerMessagePulsatingIcon, Locator.headerMessageTextArea, Locator.headerMessageCharWarning, 150);

                    if (headerFormatting)
                    {
                        promo.textFontFormattingTest("Header Message", headerFont, Locator.headerFormatFontBtn, Locator.headerFormatFonts, Locator.headerMessageTextArea, Locator.headerValueArea);
                        promo.headerMessageSizeFormattingTest(headerTextSize);
                        promo.headerMessageBoldFormatTest(headerBold);
                        promo.textChangeTextColourTest("Header Message", headerTextColour, Locator.headerFormatColourBtn, Locator.headerMessagePulsatingIcon, Locator.headerMessageTextArea, Locator.headerFormatColourList, Locator.headerValueArea);
                        promo.headerMessageChangeBackgroundColourTest(backgroundColour);
                        promo.textAlignmentTest("Header Message", headerAlignment, Locator.headerFormatAlignmentButtons, Locator.headerMessagePulsatingIcon, Locator.headerMessageTextArea, Locator.headerValueArea);
                    }

                    promo.textInputTest("Bottom Message", bottomMessage, Locator.bottomMessageSection, Locator.bottomMessagePulsatingIcon, Locator.bottomMessageText, Locator.bottomMessageCharWarning, 500, Locator.bottomMessageHeader);

                    if (bottomFormatting)
                    {
                        promo.textFontFormattingTest("Bottom Message", bottomFont, Locator.bottomFormatFontBtn, Locator.bottomFormatFonts, Locator.bottomMessageText, Locator.bottomMessageHeader);
                        promo.textChangeTextColourTest("Bottom Message", bottomTextColour, Locator.bottomFormatColourBtn, Locator.bottomMessagePulsatingIcon, Locator.bottomMessageText, Locator.bottomFormatColourList, Locator.bottomMessageSection);
                        promo.textAlignmentTest("Bottom Message", bottomAlignment, Locator.bottomFormatAlignmentButtons, Locator.bottomMessagePulsatingIcon, Locator.bottomMessageText, Locator.bottomMessageSection);
                    }
                    promo.bottomMessageToolTipCapture();
                    promo.nameAreaInputTest(name);
                    if (nameFormatting)
                    {
                        promo.textFontFormattingTest("Name area", nameFont, Locator.nameAreaFontBtn, Locator.nameAreaFontList, Locator.nameAreaTextInput, null);
                        promo.textChangeTextColourTest("Name area", nameTextColour, Locator.nameAreaColourBtn, Locator.nameAreaPulsatingIcon, Locator.nameAreaTextInput, Locator.nameAreaColourList, null);
                        promo.textAlignmentTest("Name area", nameAlignment, Locator.nameAreaAlignmentButtons, Locator.nameAreaPulsatingIcon, Locator.nameAreaTextInput, null);
                    }
                    if (templateCheck) template.captureTemplateState();
                    promo.continueRecipientsPage();
                    promo.backtoTemplateandCheck(templateCheck, template); // instead of file row, use actual template populated on webpage. TBD

                    if (downloadFileTest)
                    {
                        promo.downloadSampleTest("RecipientsSample.csv");
                        promo.downloadSampleInfoIconTest();
                    }

                    if (importFileTest)
                    {
                        var path = Assembly.GetCallingAssembly().CodeBase;
                        var actualPath = path.Substring(0, path.LastIndexOf("bin"));
                        var projectPath = new Uri(actualPath).LocalPath;
                        projectPath = Path.Combine(projectPath, "Resource");
                        string[] names = RemovePrecedingChars("\\", validFileName, faultyFileName);
                        validFileName = names[0]; faultyFileName = names[1];
                        promo.importSpreadsheetTest(projectPath, validFileName, faultyFileName);
                    }

                    if(manuelRecipientInput) promo.addNewRecipientTest(row);
                    if(amendRecipient) promo.amendUploadedRecipientTest(row);
                    promo.continueDeliveryPage();
                    if(addressAutoLookup) promo.addressAutoCompleteTest(row);
                    if(addressManualEntry) promo.addressManualEntryTest(row);
                    promo.fillContactDetails(row);
                    if (dateandtimeTest)
                    {
                        promo.deliveryDetailsDateTest(row);
                        promo.deliveryDetailsTimeTest(row);
                    }
                    promo.deliveryPageBackAndNextTest();
                    promo.confirmationPageChecksTest(row);
                    promo.secureTradingTest(row);
                }
            }
            catch (Exception ex)
            {
                checkResult result = new checkResult();
                result.log = "An exception occured. Message :" + ex.Message + " - " + ex.StackTrace;
                result.logType = LogType.FATAL;
                insertLog(GetTest(), GetDriver(), result, false);
                Assert.IsTrue(false);
            }
        }

        private static string[] RemovePrecedingChars(string removal, params string[] textarray)
        {
            for (int i = 0; i < textarray.Length; i++)
            {
                if (textarray[i].Contains(removal))
                {
                    int separatorIndex = textarray[i].IndexOf(removal);
                    textarray[i] = textarray[i].Substring(separatorIndex + 1, textarray[i].Length - separatorIndex - 1);

                }
            }
            return textarray;
        }
    }
}
