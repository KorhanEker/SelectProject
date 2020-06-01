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

namespace SelectTest.TestCases
{
    [TestFixture]
    public class ChooseTest : ReportsGenerationClass
    {
        ChoosePage choosePage;
        ConfirmYourOrder confirmOrder;
        LoginPage loginPage;

        [Test]
        [Category("Choose")]
        public void test_validChoose()
        {
            PopulateResource(TestSource.ChoiceRedemption);
            string url, code, pin, cvv, expiry, firstName, lastName, staffNumber, emailAddress;
            string updateFirstName, updateLastName, updateStaffNumber, updateEmailAddress;
            string addressLine1, addressLine2, cityTown, county, postCode;
            double orderTotal = 0;

            foreach (DataRow row in _dtResource.Rows)
            {
                choosePage = new ChoosePage(GetDriver(), GetTest(), GetWait(), loginPage);
                confirmOrder = new ConfirmYourOrder(GetDriver(), GetTest(), GetWait());
                url = row["URL"].ToString();
                code = row["Code"].ToString();
                pin = row["Pin"].ToString();
                cvv = row["CV2"].ToString();
                expiry = row["ExpiryDate"].ToString();
                firstName = row["FirstName"].ToString();
                lastName = row["LastName"].ToString();
                staffNumber = row["StaffNumber"].ToString();
                emailAddress = row["EmailAddress"].ToString();

                //for updating Delivery Details
                updateFirstName = row["UpdateFirstName"].ToString();
                updateLastName = row["UpdateLastName"].ToString();
                updateStaffNumber = row["UpdateStaffNumber"].ToString();
                updateEmailAddress = row["UpdateEmailAddress"].ToString();

                // Address Details
                addressLine1 = row["AddressLine1"].ToString();
                addressLine2 = row["AddressLine2"].ToString();
                cityTown = row["CityTown"].ToString();
                county = row["County"].ToString();
                postCode = row["PostCode"].ToString();
                // getting to the choose page
                bool valid = choosePage.gotoChoosePage(url, code, pin, cvv, expiry);
                if (!valid) continue;
                // check if the Gift Type filter available and working
                choosePage.filterSection_GiftTypeSelection();
                // check if the Sort By filter available and working
                choosePage.filterSection_SortBySelection();
                // check if search functionality working
                choosePage.filterSection_Search();
                // viewing and adding codes
                choosePage.ViewAddCodesTest();
                // adding products to the basket
                choosePage.AddProducttoBasket(ref orderTotal);
                // checkout process
                choosePage.CheckoutProcess(orderTotal);
                // confirm your order page population and confirming order
                confirmOrder.fillingDetailsandOrderComplete(firstName, lastName, emailAddress, staffNumber, postCode, updateFirstName, updateLastName, updateStaffNumber, updateEmailAddress, addressLine1, addressLine2, cityTown, county);
            }
        }
    }
}
