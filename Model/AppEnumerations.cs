namespace CSCache.Model
{
    public enum DevicesTypes
    {
        Desktop,
        Smartphone,
        Cellphone,
        Tablet
    }

    public enum DeviceImageSettingsResolution
    {
        Slider,
        Poster,
        Promo
    }

    public enum UITranslations
    {
        LOGIN_ID,
        PROFILE_ID,
        PRODUCT,
        COMBO,
        CABEZAL_BTN_FIND,
        CABEZAL_CBO_COMPLEX,
        COMPLEX_TITLE,
        MAIN_MOST_VIEWED,
        MENU_SHOWTIMES,
        MENU_COMPLEX_TITLE,
        MENU_PRE_SALE,
        MENU_COMMING_SOON,
        MENU_CONTACT,
        PROFILE_ID_TOOLTIP,
        WZD_TIMEOUT_MESSAGE,
        WZ1_CBO_COMPLEX,
        WZ1_CBO_MOVIE,
        WZ2_TIMEOUT_ERROR,
        WZ2_COMPLEX,
        WZ2_AUDITORIUM,
        WZ2_DATE,
        //GR 25/2/2018
        WZ2_LOG_IN,
        WZ4_COMPLEX,
        WZ4_PURCHASE_METHOD,
        WZ4_TITLE_INVOICE,
        WZ4_TITLE_CARD,
        WZ4_TITLE_TERMS,
        WZ4_TITLE_PICKUP_CODE,
        WZ4_TITLE_PURCHASE_DETAIL,
        // Agustin Martinez - 3/8/2017
        WZ4_TAX_ID,
        WZ4_TAX_NAME,
        WZ4_TAX_ERROR_MESSAGE,
        WZ4_BOTTOM_INFO,

        WZ5_TITLE_PURCHASE_SUMMARY,
        //GR 22/7/2017
        WZ2_PAYMENT_NUM_CBO_PRICE,
        WZ2_PAYMENT_NUM_SPAN_PRICE_REQUIRED,
        WZ2_PAYMENT_NUM_SPAN_PRICE_UNIQUE,
        WZ2_PAYMENT_NUM_TITLE,
        WZ2_PAYMENT_NUM_SPAN_INFO,
        WZ2_PAYMENT_NUM_TOTAL_SEATS,
        WZ2_PAYMENT_NUM_TOTAL_PRICE,
        WZ2_PAYMENT_NUM_SEATS,
        WZ2_PAYMENT_NUM_TYPE_AND_PRICE,
        WZ2_PAYMENT_NUM_ERROR_SELECT_PRICES,
        //GR 31/07/2017
        WZ2_BACK_STEP1_CONFIRMATION,
        WZ2_BACK_STEP1_CONFIRMATION_INFO,
        WZ2_BACK_STEP1_CONFIRMATION_QUESTION,
        WZ2_BACK_STEP1_CONFIRMATION_NO,
        WZ2_BACK_STEP1_CONFIRMATION_YES,
        CONTACT_PAGE_BOTTOM_INFO,

        //GR 7/9/2017
        WZ2_PROMOTION_AVAILABLE,
        WZ2_WITHOUT_PROMOTION,

        //GR 22/09/2017
        WZ3_COMBO_CONTROLSALE_MESSAGE,
        WZ3_COMBO_CONTROLSALE_TITLE,
        //GR 06/01/2018
        BUY_BUTTON,
        //GR 28/5/2018
        WZ2_UPDATE_USER_ERROR,
        WZ2_UPDATE_SUCCESS,
        WZ2_SELECT_DATE,
        WZ2_LOGIN_USER_ALL_FIELD_REQUIRED,
        WZ2_USER_REGISTER_SEND_MAIL,
        WZ2_COUNT_TICKETS_ERROR,
        WZ2_NO_TICKET_SELECTED,
        WZ2_CONFIRM_CONDITIONS,
        WZ2_NUMBER_CODE_DIGITS,
        WZ2_BUY_SUCCESS,
        WZ2_BUY_ERROR,
        WZ2_SELECT_MEANS_OF_PAYMENTS,
        WZ2_NO_TICKETS_SELECTED,
        WZ2_TICKETS_ANOTHER_COMPLEX,
        WZ2_MAX_SALE_ERROR,
        WZ2_BLOCKED_SEAT,
        WZ2_UPDATE_BROWSER,
        WZ2_NOT_SEAT_SELECTED,
        WZ2_BLOCKING_SEAT,
        WZ2_COUNT_TICKETS,
        //GR 04/08/2018
        BUY_TITLE,
        BUY_FUNCTION_DATE,
        BUY_BUY_DATE,
        BUY_COMPLEX,
        BUY_TICKETS,
        BUY_CODE,
        BUY_EMPTY,
        //GR 7/2/2018
        PROFILE_ERROR_TITLE,
        PROFILE_LOGIN,
        PROFILE_SIGN_UP,
        PROFILE_MAIL,
        PROFILE_PASSWORD,
        PROFILE_RESET_PASSWORD,
        PROFILE_FORGET_PASSWORD,
        PROFILE_CLOSE,
        PROFILE_NAME,
        PROFILE_ENTER_NAME,
        PROFILE_SURNAME,
        PROFILE_ENTER_SURNAME,
        PROFILE_SEX,
        PROFILE_MALE,
        PROFILE_FEMALE,
        PROFILE_PHONE,
        PROFILE_ENTER_PHONE,
        PROFILE_ADDRESS,
        PROFILE_ENTER_ADDRESS,
        PROFILE_ENTER_MAIL,
        PROFILE_CONFIRM_MAIL,
        PROFILE_BIRTH_DATE,
        PROFILE_ENTER_DATE,
        PROFILE_ENTER_PASSWORD,
        PROFILE_CONFIRM_PASSWORD,
        PROFILE_ENTER_CAPTCHA,
        PROFILE_FORGET_PASSWORD_ENTER_MAIL,
        PROFILE_RECOVERY_PASSWORD,
        PROFILE_BACK_SCREEN,
        PROFILE_SIGN_IN,
        PROFILE_UPDATE,
        PROFILE_CONFIRM_FIELDS,
        PROFILE_REQUIRED,
        PROFILE_INVALID_FIELD,
        PROFILE_UPDATE_SUCCESS,
        PROFILE_CONFIRM_REGISTER,
        PROFILE_UNEXPECTED_ERROR,
        PROFILE_EXISTING_ID,
        PROFILE_EXISTING_MAIL,
        PROFILE_ERROR_CAPTCHA,
        PROFILE_ERROR_USER_DATA,
        PROFILE_ERROR_MAIL_NOT_FOUND,
        PROFILE_ERROR_INVALID_MAIL,
        PROFILE_INVALID_LENGTH_PASSWORD,
        PROFILE_INVALID_STRENGTH_PASSWORD,
        //GR 11/5/2019
        REJECTED_SALE_MESSAGE,
        RESET_PASSWORD_MESSAGE,
        INVALID_LENGTH_FIELD,
        //GR 22/7/2019
        LOYALTY_TITLE,
        LOYALTY_DATE_FROM,
        LOYALTY_DATE_TO,
        LOYALTY_SEARCH,
        LOYALTY_COMPLEX,
        LOYALTY_BUY_TYPE,
        LOYALTY_DATE,
        LOYALTY_POINTS,
        LOYALTY_WITHOUT_RESULT,
        LOYALTY_TOTAL_POINTS,
        CONTACT_ERROR_CAPTCHA,
        CONTACT_MAIL_SENT,
        CONTACT_ENTER_CAPTCHA
    }

    public enum AppLanguages
    {
        ESP,
        ENG
    }

    public enum AppPages
    {
        TermsAndConds = 1,

    }

    //MF 12/2021
    //Para las estadisticas
    public enum AppEventId
    {
        NewVisitor,
        Login,
        Register,
        PassWordRecover,
        ShowTime,
        PreSale,
        CommingSoon,
        cartStep1,
        cartStep2,
        cartStep3,
        cartStep4,
        cartStep5,
        MovieTheater,
        Contact,
        Sections,
        Search
    }
}