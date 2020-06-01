using Microsoft.AspNetCore.Http;
using OpenQA.Selenium.DevTools.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SelectTest.HelperMethods
{
    public static class Locator
    {
        //------------------------------------    Choose Login Page Locators -----------------------------------------------------------------------

        public const string codeInput = "CardCodeInput";
        public const string pinInput = "PinInput";
        public const string cv2Input = "Cv2Input";
        public const string expiryDateInput = "ExpiryDateInput";
        public const string btnContinue = "//button[@class='primary-button redeem-button']";
        public const string choosePage = "//html/body[@class='signedIn']";
        public const string cookiePolicy = "#cp-yes > button";
        public const string notificationBox = "notificationBox";

        //------------------------------------    Choose Page Locators -----------------------------------------------------------------------
        public static string CodeEntryNotificationPath = "#notificationBox";
        public static string CodeEntryNotificationPopupButton = "#notificationBox > div.noty_body > div > button";
        public static string filterSection = "//section[@class='filters-section']";
        public static string giftTypeDrop = "react-select-3-input";
        public static string giftTypeDropSubOptions = "//div[contains(@id,'react-select-3-option-')]";
        public static string appliedFilterHeaderPath = "//section[@class='applied-filters']/h2";
        public static string appliedFilterNamePath = "//span[@class='filter-name']";
        public static string sortByDrop = "react-select-2-input";
        public static string sortByDropSubOptions = "//div[contains(@id,'react-select-2-option-')]";
        public static string A_Z_SortingClass = "//div[@class='content-wrapper']/div[@class='productInfo']/div[@class='name']";
        public static string ProductPricingRange = "//div[@class='content-wrapper']/div[@class='productInfo']/div[@class='price-range']";
        public static string searchPath = "div.search>input[type=text]";
        public static string searchButtonPath = "button[aria-label=submit]";
        public static string viewAddCodes = "//div[@class='viewCodesSelect']/button";
        public static string viewAddCodesPanel = "div.viewCodes > div > div.viewCodesPanel.open";
        public static string AddCodesFieldPath = "input[name=CardCodeInput]";
        public static string NotificationBoxPath = "//div[@id='notificationBox']";
        public static string NotificationBoxTitle_Path = "//div[@id='notificationBox']//span[contains(@class,'title')]";
        public static string NotificationBoxMessage_Path = "//div[@id='notificationBox']//p[contains(@class,'message')]";
        public static string ViewAddCodes_CloseButtonPath = "#appTopBarPortal > div > div:nth-child(2) > div > div.viewCodes > div > div.viewCodesPanel.open > div.closePanel > div.closePanelArea > button";
        public static string RemainingValuePath = "div[class = 'remaining chartContainer'] > div > div > h3[class='chart-label']";
        public static string ProductPopupValueButtonsMin = "input[id^=min-value-input]";
        public static string ProductPopupAddToBasket = "div.modal-body > div.flexWrap > div.product-actions > button";
        public static string DisplayedBasketPath = "div.basketPanel.open > div.basketBottom";
        public static string BasketTotalPath = "div.basketPanel.open > div.basketBottom > div.total-area > span:nth-child(2)";
        public static string OpenBasketButtonPath = "div.basket > div > div.basketSelect > div.basketSelectInner > button";
        public static string BasketCheckoutButtonPath = "div.basketPanel.open > div.basketBottom > a > button";
        public static string CloseBasketButtonPath = "div.basketPanel.open > div.closePanel > div.closePanelArea > button";
        public static string ProductCustomInputPath = "div.flexWrap > div.product-actions > div.custom-value-area > input";
        public static string CustomInputErrorMessagePath = "div.flexWrap > div.product-actions > div.custom-message-area > p";
        public static string ConfirmYourOrderPagePath = "#catalogRoot > div > div > div.orderConfirmation";
        public static string OldDesignIndicatorPath = "div.slick-container";
        public static string SlickVisibleProductsPath = "(//button[contains(@class,'tabTop')])";
        public static string SlickDenomsMinimumValuePath = "div > div > div > div.tab.open > div > div > div.select-value-buttons.three-options > div:nth-child(1)>input";
        public static string SlickDenomsMediumValuePath = "div > div > div > div.tab.open > div > div > div.select-value-buttons.three-options > div:nth-child(2)>input";
        public static string SlickDenomsMaximumValuePath = "div > div > div > div.tab.open > div > div > div.select-value-buttons.three-options > div:nth-child(3)>input";
        public static string ChoosePageVerifierPath = "div#catalogRoot";
        public static string MaximumOrderLimitPath = "#appTopBarPortal > div > div.appStatistics > div > div.statsTitle > div.charts-warning";


        public static string DenominationSection = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > div.select-value-buttons.three-options";
        public static string AddProductButton = "div.tab.open > div > button.addToBasket";
        public static string ProductCloseButton = "div.tab.open > button";
        public static string CustomValueField = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > div.custom-value-area > input";
        public static string CustomValueWarningMessage = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > div.custom-message-area>p";
        public static string DenomsMinimumValuePath = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > div.select-value-buttons.three-options > div:nth-child(1)>input";
        public static string DenomsMediumValuePath = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > div.select-value-buttons.three-options > div:nth-child(2)>input";
        public static string DenomsMaximumValuePath = "body > div.ReactModalPortal > div > div > div.modal-body > div.flexWrap > div.product-actions > div.select-value-buttons.three-options > div:nth-child(3)>input";

        //------------------------------------    Confirm Your Order Page Locators -----------------------------------------------------------------------

        public static string YourDetailsSummaryPath = "div.details-group > div.user-details-preview";
        public static string YourDetailsEditButtonPath = "#confirmationForm > div.largeWrap > div.details > div:nth-child(2) > div:nth-child(1) > div > i";
        public static string FirstNamePath = "input[name=FirstNameInput]";
        public static string LastNamePath = "input[name=LastNameInput]";
        public static string StaffNumberPath = "input[name=StaffNumberInput]";
        public static string EmailAddressPath = "input[name=EmailAddressInput]";
        public static string ConfirmEmailAddressPath = "input[name=ConfirmEmailAddressInput]";
        public static string AddressInputFieldPath = "#react-select-4-input";
        public static string AddressResultPopup = "div[id^=react-select-4-option-]";
        public static string AddressLine1Path = "input[name=AddressLine1Input]";
        public static string AddressLine2Path = "input[name=AddressLine2Input]";
        public static string CityTownPath = "input[name=CityTownInput]";
        public static string CountyPath = "input[name=CountyInput]";
        public static string PostcodePath = "input[name=PostcodeInput]";
        public static string CountryPath = "input[name=CountryInput]";
        public static string TandCsPathNewDesign = "#confirmationForm > div.largeWrap > div.details > div:nth-child(3) > div > div > label > div > b:nth-child(1) > a";
        public static string TandCsPathOldDesign = "#confirmationForm > div.largeWrap > div.details > div:nth-child(3) > div:nth-child(1) > div > label > div > p > a:nth-child(1)";
        public static string PrivacyPolicyPathNewDesign = "#confirmationForm > div.largeWrap > div.details > div:nth-child(3) > div > div > label > div > b:nth-child(2) > a";
        public static string PrivacyPolicyPathOldDesign = "#confirmationForm > div.largeWrap > div.details > div:nth-child(3) > div:nth-child(1) > div > label > div > p > a:nth-child(2)";
        public static string TandCCheckBoxPath = "input[name = termsCheckbox][type=checkbox]";
        public static string PlaceOrderBtnPath = "//button[@class='icon-upArrowIconAfter primary-button placeOrder']";
        public static string ChangeOrderBtnPath = "//button[@class='icon-upArrowIcon secondary-button changeOrder']";
        public static string ChoocePageIdentifier = "#tilesContainer > div";
        public static string TotalValuePath = "div.basket > div > div.basketPanel.open > div.basketBottom > div.total-area > span:nth-child(2)";
        public static string CheckoutBtnPath = "div.basket > div > div.basketPanel.open > div.basketBottom > a > button";
        public static string ConfirmCheckoutPopupPath = "//div[@class='ReactModal__Content ReactModal__Content--after-open modal-Dialog']";
        public static string ConfirmPopupGoBackBtnPath = "body > div.ReactModalPortal > div > div > button.icon-upArrowIcon.modal-cancel-button.secondary-button";
        public static string ConfirmPopupConfirmBtnPath = "body > div.ReactModalPortal > div > div > button.icon-upArrowIconAfter.modal-confirm-button.primary-button";
        public static string PlaceOrderBtnAfterPath = "//button[@class='icon-upArrowIconAfter primary-button placeOrder']";
        public static string pathforProductListSlick = "div.slick-container > div > div.slick-list> div.slick-track>div";

        //------------------------------------    Promotional Site Locators -----------------------------------------------------------------------
        public static string orderNowBtn_1 = "body > main > section.homepageBanner > div.top > div > div.homeBannerContent > a.orderButton";
        public static string navigation_bar = "div#nav-bar.nav-bar.sticky";

        public static string logodrop = "div.container.templateWrap > div.row.imageUploadArea > div > div > ngx-img > div > input[type=file]";
        public static string logoErrorMessage = "div.row.imageUploadArea > div > div > ngx-img > div > div.ngx-img-message > div.ngx-img-error > ul > li";
        public static string logoTitlePath = "div.container.templateWrap > div.row.imageUploadArea > div > div > ngx-img > div > div > div > div > p.ngx-img-filename";
        public static string logoTitleAlternative = "div.container.templateWrap > div.row.imageUploadArea > div > div > ngx-img > div > input[type=file]";
        public static string logoOrientationButtons = "div.row.imageUploadArea > div > div > div.row.buttonWrap > div > div.btn-group > button";
        public static string logoOrientationLeft = "div.container.templateWrap > div.row.imageUploadArea > div.col-sm-4";
        public static string logoOrientationMiddle = "div.container.templateWrap > div.row.imageUploadArea > div.col-sm-4.col-sm-offset-4";
        public static string logoOrientationRight = "div.container.templateWrap > div.row.imageUploadArea > div.col-sm-4.col-sm-offset-8";
        public static string removeLogoBtn = "div.row.imageUploadArea > div > div > ngx-img > div > button.ngx-img-clear";
        public static string logoAreaforTextSearch = "div.container.templateWrap > div.row.imageUploadArea > div > div > ngx-img.imageUploadDropBox > div.ngx-img-wrapper";

        public static string templateImagePath = "div.container.templateWrap > div:nth-child(2) > div:nth-child(1) > div.imageSelection>img";
        public static string templateImageHover = "//div[contains(@class,'selectTemplateHover')]";
        public static string templateList = "app-template-customisation > app-template-selection > div > div > div > div > div > div.templateSelection.row";
        public static string selectedTemplate = "app-template-customisation > app-template-selection > div > div > div > div > div > div > div[class*='selected']";
        public static string templateSelectorModal = "app-template-customisation > app-template-selection > div > div > div.modal-content > div.modal-body";

        public static string headerMessageSection = "div[class*='headerMessageZone']";
        public static string headerMessageTextArea = "div.bodyTextArea > textarea";
        public static string headerMessageCharWarning = "div[class^='bodyTextWrap'] > div.bodyTextArea >small";
        public static string headerMessagePulsatingIcon = "div.bodyTextWrap > div.bodyTextArea > div.pulsating-circle";
        public static string headerFormatFontBtn = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div> button.btn.dropdown-toggle.fontButton";
        public static string headerFormatFonts = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar >  div:nth-child(1) > ul > li";
        public static string headerFormatSizeBtn = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div:nth-child(2) > button";
        public static string headerFormatSizes = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div:nth-child(2) > ul > li";
        public static string headerFormatBoldBtn = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > button.btn.activePinkButton.boldSelect";
        public static string headerFormatColourBtn = "div.container.templateWrap > div:nth-child(2) >  div:nth-child(2) > div.customiseBar > div:nth-child(4) > button";
        public static string headerFormatColourList = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div:nth-child(4)> ul > li";
        public static string headerFormatBackgroundClrBtn = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div:nth-child(5) > button";
        public static string headerFormatBgrColourList = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div:nth-child(5) > ul > li";
        public static string headerFormatAlignmentButtons = "div.container.templateWrap > div:nth-child(2) > div:nth-child(2) > div.customiseBar > div:nth-child(6) > button";
        public static string headerValueArea = "div.container.templateWrap > div:nth-child(2) > div.col-xs-12.headerMessageZone > div > div.valueAmount";

        public static string bottomMessageSection = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div[class^='bottomAreaWrap']";
        public static string bottomMessageHeader = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div[class^='bottomAreaWrap']>h3";
        public static string bottomMessageText = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div[class^='bottomAreaWrap']>div.bottomMessageTextArea>textarea";
        public static string bottomMessagePulsatingIcon = "div.container.templateWrap > div.row.bottomMessageZone > div.pulsating-circle";
        public static string bottomMessageCharWarning = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div[class^='bottomAreaWrap']> div > small";
        public static string bottomFormatFontBtn = "div.container.templateWrap > div.row.bottomMessageZone > div.customiseBar > div:nth-child(1) > button";
        public static string bottomFormatFonts = "div.container.templateWrap > div.row.bottomMessageZone > div.customiseBar > div:nth-child(1) > ul > li";
        public static string bottomFormatColourBtn = "div.container.templateWrap > div.row.bottomMessageZone > div.customiseBar > div:nth-child(2) > button";
        public static string bottomFormatColourList = "div.container.templateWrap > div.row.bottomMessageZone > div.customiseBar > div:nth-child(2) > ul > li";
        public static string bottomFormatAlignmentButtons = "div.container.templateWrap > div.row.bottomMessageZone > div.customiseBar > div:nth-child(3) > button";
        public static string bottomMessageTooltipIcon = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div[class ^= 'bottomAreaWrap'] > h3 > span > i";

        public static string nameAreaPulsatingIcon = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > div";
        public static string nameAreaFontBtn = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > div.customiseBar > div:nth-child(1) > button";
        public static string nameAreaFontList = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > div.customiseBar > div:nth-child(1) > ul > li";
        public static string nameAreaColourBtn = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > div.customiseBar > div:nth-child(2) > button";
        public static string nameAreaColourList = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > div.customiseBar > div:nth-child(2) > ul > li";
        public static string nameAreaAlignmentButtons = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > div.customiseBar > div:nth-child(3) > button";
        public static string nameAreaTextInput = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea > input";
        public static string nameSection = "div.container.templateWrap > div.row.bottomMessageZone > div.col-xs-12 > div > div.nameTextArea";

        public static string templateContinueBtn = "app-template-customisation > div > div.previewColumn > div.navButtons.text-center > button.btn.btn-info.goForward";
        public static string recipientsPageIndicator = "body > main > app-root > block-ui > section > div > div > app-recipients-list";
        public static string CustomiseTemplateBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.navButtons.row.form-group.text-center > button.btn.btn-default.goBack";
        public static string TemplatePageIndicator = "body > main > app-root > block-ui > section > div > div > app-template-customisation > div.customiseTemplateWrapper";
        public static string templateContainer = "div.container.templateWrap";

        public static string downloadSampleBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.row.actionButtons > div:nth-child(1) > div > button";
        public static string downloadSampleModal = "mat-dialog-container[id ^= 'mat-dialog-']";
        public static string downloadSampleModalCancelBtn = "mat-dialog-container[id ^= 'mat-dialog-'] > app-download-sample-confirmation-dialog > mat-dialog-actions > div > button[class *='cancelButton']";
        public static string downloadSampleModalDownloadBtn = "mat-dialog-container[id ^= 'mat-dialog-'] > app-download-sample-confirmation-dialog > mat-dialog-actions > div > button.btn.btn-primary.downloadButton";
        public static string downloadSampleInfoIcon = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.row.actionButtons > div:nth-child(1) > div > i";
        public static string downloadSampleTooltip = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.row.actionButtons > div:nth-child(1) > div > div > div.tooltip-inner";
        public static string importSpreadsheetBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.row.actionButtons > div:nth-child(1) > input";
        public static string importSuccessBanner = "div.overlay-container>div[id = 'toast-container'] > div[class ^= 'toast-success']";
        public static string importErrorBanner = "div.overlay-container>div[id = 'toast-container'] > div[class ^= 'toast-error']";
        public static string importBannerText = "#toast-container > div > div.toast-message.ng-star-inserted";
        public static string recipientsTable = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.recipientLists > div:nth-child(1) > div > mat-table";
        public static string recipientRecords = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.recipientLists > div:nth-child(1) > div > mat-table > mat-row";
        public static string editRecipientModal = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body";
        public static string editRecipientValueRow = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div.row.emailRow > div[class *= 'buttonRow'] > div[class *= 'btnRow']";
        public static string editRecipientMessage = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-xs-12.col-md-5 > div > textarea";
        public static string editRecipientLimitWarning = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div.maxValueWarning > span";
        public static string editRecipientAboveLimitError = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div.row.ng-star-inserted > div > div";
        public static string editRecipientSaveBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-footer > div > button:nth-child(2)";
        public static string editRecipientCancelBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-footer > div > button:nth-child(1)";
        public static string editRecipientMessageRemainingChars = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-xs-12.col-md-5 > div > small";
        public static string filldetailsOnlineBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.row.actionButtons > div:nth-child(2) > button.manualButton";
        public static string editRecipientFirstNameWarning = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div:nth-child(1) > div:nth-child(1) > div";
        public static string editRecipientLastNameWarning = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div:nth-child(1) > div:nth-child(2) > div";
        public static string editRecipientEmailWarning = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div.row.emailRow > div:nth-child(1) > div > div";
        public static string editRecipientValueMissingWarning = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div.col-md-7 > div.row.ng-star-inserted > div";
        public static string editRecipientDenoms = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-body > div:nth-child(3) > div > div > div:nth-child(1) > div.row.emailRow > div[class *= 'buttonRow']> div";
        public static string editRecipientModalDoneBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-footer > div > button[class ^= 'cancelBTN']";
        public static string editRecipientModalAddAnotherBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > app-add-recipients > form > div > div > div > div.modal-footer > div > button[class *= 'addAnother']";
        public static string recipientsPageContinueBtn = "body > main > app-root > block-ui > section > div > div > app-recipients-list > div > div.navButtons.row.form-group.text-center > button.btn.btn-info.goForward";
        
        public static string deliveryPageIndicator = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form[class ^= 'deliveryAddressForm']";
        public static string deliveryPageAddressSearchResults = "mat-option[id ^= 'mat-option']";
        public static string deliveryPageAutoCompleteTextArea = "#autoCompleteAddressArea > textarea"; 
        public static string deliveryPageManualAddressBtn = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div:nth-child(2) > div[class *= 'leftCol'] > div:nth-child(1) > button";
        public static string deliveryPageManualAddressTxtArea = "#autoCompleteAddressArea > textarea";
        public static string deliveryCompanyNameWarning = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div:nth-child(2) > div[class *= 'leftCol'] > div.row.bottomArea > div:nth-child(2) > div";
        public static string deliveryFirstNameWarning = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div:nth-child(2) > div[class *= 'leftCol'] > div.row.bottomArea > div:nth-child(3) > div";
        public static string deliveryLastNameWarning = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div:nth-child(2) > div[class *= 'leftCol'] > div.row.bottomArea > div:nth-child(4) > div";
        public static string deliveryEmailAddressWarning = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div:nth-child(2) > div[class *= 'leftCol'] > div.row.bottomArea > div:nth-child(5) > div > div";
        public static string deliveryPageDateBtn = "mat-datepicker-toggle > button";
        public static string deliveryPageCalendarModal = "mat-calendar[id ^= 'mat-datepicker']";
        public static string deliveryPageCalendarRows = "mat-calendar[id ^= 'mat-datepicker'] > div > mat-month-view > table > tbody > tr";
        public static string deliveryPageCalendarMonthYear = "mat-calendar[id ^= 'mat-datepicker'] > mat-calendar-header > div > div > button.mat-calendar-period-button.mat-button > span";
        public static string deliveryPageCalendarNextButton = "mat-calendar[id ^= 'mat-datepicker'] > mat-calendar-header > div > div > button.mat-calendar-next-button.mat-icon-button";
        public static string deliveryPageTimeBtn = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div:nth-child(2) > div[class *= 'rightCol'] > div > div.body > mat-form-field[class ^='timeSelect']";
        public static string deliveryPageNextBtn = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div.navButtons.row.form-group.text-center > button.btn.btn-info.goForward";
        public static string deliveryPageGoBackBtn = "body > main > app-root > block-ui > section > div > div > app-delivery-address > form > div > div.navButtons.row.form-group.text-center > button.btn.btn-default.goBack";

        public static string confirmationPageIndicator = "body > main > app-root > block-ui > section > div > div > app-confirmation > div[class *= 'confirmPage']";
        public static string confirmationPageCheckoutBtn = "body > main > app-root > block-ui > section > div > div > app-confirmation > div > div:nth-child(5) > button";
        public static string confirmationPageCheckoutModalPopup = "mat-dialog-container[class ^= 'mat-dialog-'] > app-proceed-payment-confirmation-dialog";
        public static string confirmationPageCheckOutModalCancelBtn = "mat-dialog-container[class ^= 'mat-dialog-'] > app-proceed-payment-confirmation-dialog > div > mat-dialog-actions > div > button[class *= 'cancelBTN']";
        public static string confirmationPageCheckOutModalContinueBtn = "mat-dialog-container[class ^= 'mat-dialog-'] > app-proceed-payment-confirmation-dialog > div > mat-dialog-actions > div > button:nth-child(2)";

        public static string secureTradingStandardDetails = "#st-block-standard-details";
        public static string PaymentConfirmationPageIndicator = "body > main > div.container.tab-pane.paymentConfirmPage";
        public static string secureTradingBillingGroup = "#st-block-billingdetailsdiv > div";
        public static string secureTradingSubmitErrorMessage = "#st-block-messagesdiv > div.message-error";
    }

}

