using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelectTest.Config;
using SelectTest.PageMethods;
using System.Data;
using System.Threading;
using SelectTest.HelperMethods;

namespace SelectTest.TestCases
{
    [TestFixture]
    public class LoginTest : ReportsGenerationClass
    {
        LoginPage loginPage;

        [Test]
        [Category("Login")]
        public void test_validLogin()
        {
            checkResult result = new checkResult();
            PopulateResource(TestSource.Login);
            foreach (DataRow row in _dtResource.Rows)
            {
                loginPage = new LoginPage(GetDriver(),GetTest(),GetWait());
                loginPage.goToPage(row["URL"].ToString());
                loginPage.clickCookieP();
                Thread.Sleep(new TimeSpan(0,0,4));

                loginPage.enterFieldValue(row["Code"].ToString(), Locator.codeInput);
                loginPage.enterFieldValue(row["Pin"].ToString(), Locator.pinInput);
                loginPage.enterFieldValue(row["CV2"].ToString(), Locator.cv2Input);
                loginPage.enterFieldValue(row["ExpiryDate"].ToString(), Locator.expiryDateInput);
                loginPage.clickbtnContinue();
                Thread.Sleep(new TimeSpan(0, 0, 8));
                loginPage.checkErrorNotification();
                var checkResult = new checkResult();
                checkResult.log += " --> Login details: " +
                        row["URL"].ToString() + " - " +
                        row["Code"].ToString() + " - " +
                        row["Pin"].ToString() + " - " +
                        row["CV2"].ToString() + " - " +
                        row["ExpiryDate"].ToString();
                loginPage.insertLog(GetTest(), GetDriver(),checkResult, true);         
                Assert.IsTrue(!loginPage.verifyElement(Locator.notificationBox,selectorType.Id));
                Assert.IsTrue(loginPage.verifyElement(Locator.choosePage, selectorType.XPath));
            }
            loginPage.closeBrowser();
        }
        [Test]
        [Category("Login")]
        public void test_invalidLogin()
        {
            PopulateResource(TestSource.InvalidLogin);
            bool errorCaptured = false;
            foreach (DataRow row in _dtResource.Rows)
            {
                loginPage = new LoginPage(GetDriver(),GetTest(),GetWait());
                loginPage.goToPage(row["URL"].ToString());
                Thread.Sleep(new TimeSpan(0, 0, 4));
                loginPage.clickCookieP();
                loginPage.enterFieldValue(row["Code"].ToString(), Locator.codeInput);
                loginPage.enterFieldValue(row["Pin"].ToString(), Locator.pinInput);
                loginPage.enterFieldValue(row["CV2"].ToString(), Locator.cv2Input);
                loginPage.enterFieldValue(row["ExpiryDate"].ToString(), Locator.expiryDateInput);
                loginPage.clickbtnContinue();
                Thread.Sleep(new TimeSpan(0, 0, 4));
                loginPage.checkErrorNotification();
                var checkErrorResult = new checkResult();
                checkErrorResult.log += " --> Login details: " +
                        row["URL"].ToString() + " - " +
                        row["Code"].ToString() + " - " +
                        row["Pin"].ToString() + " - " +
                        row["CV2"].ToString() + " - " +
                        row["ExpiryDate"].ToString();
                loginPage.insertLog(GetTest(),GetDriver(),checkErrorResult,true);
                if (loginPage.verifyElement(Locator.notificationBox, selectorType.Id)) errorCaptured = true;
            }
            Assert.IsTrue(!errorCaptured);
            loginPage.closeBrowser();
        }
    }
}