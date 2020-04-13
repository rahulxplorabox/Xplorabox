using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class WebServiceController : MonoBehaviour
{
    #region Variayables and Contants
    #region Static Members
    [Header("Static Members")]
    public static WebServiceController webServiceController_Instance;
   
    public static string BaseUrl= "https://sandbox.app.xplorabox.com/";
    public static string Grant_Type = "password";
    public static string Client_Id = "2";
    public static string Client_Secret = "tAtrZjRWprr8T6c7SE24mjGtAa8AYJiylOryF0Pj";
    public static string UserName = "xplorabox@gmail.com";
    public static string Password = "qquKJMvvrAQwV8dt";
    public int GradesLength;
    #endregion

    #region PUBLIC_MEMBER
    [Header("Access Token")]
    public string AccessToken;
    public string LocalToken;

    [Header("Device ID")]
    public string DeviceId;

    [Header("User ID")]
    public int UserId;

    [Header("OTP Verification")]
    public string ServerResponseOTP;
    public string UserGivenOTP;
    public string OTPVerificationMessage;

    [Header("Worksheet Json Request")]
    public string WorksheetJsonReq;

    [Header("Child Grade Details List")]
    public int ChangeGradeIndex;
    public string ChangeGradeIndexValue;

    [Header("Language Details List")]
    public List<string> LanguageDetalList; //List of All Language 

    [Header("DeletePlayerData")]
    public bool DeletePlayerData;
    #endregion

    #region Private mebers
    [Header("Private members")]
    private string AccessTokenUrl = BaseUrl + "oauth/token";
    private string TokenCheckUrl = BaseUrl + "/api/user";
    private string SendOtpUrl = BaseUrl + "/api/send_otp";
    private string SubmitAfterOTPVerified = BaseUrl + "/api/submit_details_after_otp";
    private string GradeDetailsUrl = BaseUrl + "/api/age_groups";
    private string SubmitAfterChildAgeDetails = BaseUrl + "/api/submit_child";
    private string GetWorksheetJsonURl = BaseUrl + "/api/getworksheetjson";
    private string OrderValidationURl = BaseUrl + "/api/order_validation";
    private string LanguageDetailsUrl = BaseUrl + "/api/age_grade";

    [Header("Cache folder")]
    string cachefolder_path, XploraboxFolder;
    #endregion
    #endregion

  
    #region[Monobehaviour Methods]
    private void Awake()
    {
        #region[Deleting PlayerData]
        if (DeletePlayerData)
        {
            PlayerPrefs.DeleteAll();
            print("Played Data Deleted");
        }
        #endregion

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        foldercheck();
        if (webServiceController_Instance == null)
        {
            webServiceController_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
         GetAccessToken();
    }
    #region[Check Folder]
    public void foldercheck()
    {
        cachefolder_path = Application.persistentDataPath + "/_cachefolder/";
        XploraboxFolder = Application.persistentDataPath + "/_MyStuff/Xplorabox/";
        if (!Directory.Exists(cachefolder_path) || !Directory.Exists(XploraboxFolder))
        {
            Directory.CreateDirectory(cachefolder_path);
            Directory.CreateDirectory(XploraboxFolder);
            print("Folders doesn't exist so created new");
        }
    }
    #endregion
   

    #endregion

    #region[Custom Methods]  
    #region [GET ACCESSTOKEN]
    public void GetAccessToken()
    {
        LocalToken = PlayerPrefs.HasKey("_LocalAccessToken") ? PlayerPrefs.GetString("_LocalAccessToken") : null;
        if (!string.IsNullOrEmpty(LocalToken)) { print("Local Access Token is not null"); StartCoroutine(CheckAccessTokenValidity(LocalToken)); }
        else { print("GetAccessTokenFromServerCallBack"); StartCoroutine(GetAccessTokenFromServerCallBack(Grant_Type, Client_Id, Client_Secret, UserName, Password)); }
           
    }
    #endregion

    #region[Access Token Validity Check]
    IEnumerator CheckAccessTokenValidity(string LocalToken)
    {
        UnityWebRequest tokencheck = UnityWebRequest.Get(TokenCheckUrl);
        tokencheck.SetRequestHeader("Accept", "application/json");
        tokencheck.SetRequestHeader("Authorization", "Bearer " + LocalToken);
        yield return tokencheck.SendWebRequest();
        if (tokencheck.isNetworkError || tokencheck.isHttpError)
        {
            print("Token is Expired.. Getting New Token!!");
            StartCoroutine(GetAccessTokenFromServerCallBack(Grant_Type, Client_Id, Client_Secret, UserName, Password));
        }
        else
        {
            print("Token is Valid!!!");
            AccessToken = LocalToken;
            GetChildGradeDetails(LocalToken);
            print("Final Child Details**************" + PlayerPrefs.GetString("_finalChildNameDetails"));
            GetOrderValidity();
        }
    }
    #endregion

    #region[Getting Access Token From Server]
    IEnumerator GetAccessTokenFromServerCallBack(string _grantType, string _clientId, string _clientSecret, string _userName, string _password)
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", _grantType);
        form.AddField("client_id", _clientId);
        form.AddField("client_secret", _clientSecret);
        form.AddField("username", _userName);
        form.AddField("password", _password);

        UnityWebRequest www = UnityWebRequest.Post(AccessTokenUrl, form);
        www.SetRequestHeader("Accept", "application/json");
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            if (www.isDone) 
            {
                JsonData jsonData = JsonMapper.ToObject(www.downloadHandler.text.ToString());
                if (!string.IsNullOrEmpty(jsonData.ToString()) && !string.IsNullOrEmpty(jsonData["access_token"].ToString()))
                {
                    AccessToken = jsonData["access_token"].ToString();
                    PlayerPrefs.SetString("_LocalAccessToken", AccessToken);
                    GetChildGradeDetails(PlayerPrefs.GetString("_LocalAccessToken", AccessToken));
                    print("Local Access Token has been set");
                }
                else { Debug.Log("Network error in AccessToken API..."); }
            }
            
        }
    }
    #endregion

    #region[Send OTP]
    public IEnumerator SendOTPUrl(string PhoneNumber, string Name, string Email, string DeviceId)
    {
        if (ApploaderController.ApploaderController_Instance != null)
        {
            ApploaderController.ApploaderController_Instance.Activate_Apploader();
        }
        WWWForm sendotp_params = new WWWForm();
        sendotp_params.AddField("phone", PhoneNumber);
        sendotp_params.AddField("email", Email);
        sendotp_params.AddField("name", Name);
        sendotp_params.AddField("device_id", DeviceId);

        UnityWebRequest sendotp_request = UnityWebRequest.Post(SendOtpUrl, sendotp_params);
        sendotp_request.SetRequestHeader("Accept", "application/json");
        sendotp_request.SetRequestHeader("Authorization", "Bearer " + AccessToken);

        yield return sendotp_request.SendWebRequest();

        if (sendotp_request.isNetworkError || sendotp_request.isHttpError)
        {
            InternetConnection.InternetConnection_Instance.NetworkErrorPanel.SetActive(true);
            Debug.Log(sendotp_request.error);
        }
        else
        {
            JsonData jsonData = JsonMapper.ToObject(sendotp_request.downloadHandler.text.ToString());
            print(sendotp_request.downloadHandler.text.ToString());
            if (jsonData["message"].ToString().Equals("SUCCESS"))
            {
                Debug.Log("OTP in Server Response = " + jsonData["otp"].ToString());
                if (!string.IsNullOrEmpty(jsonData["otp"].ToString()))
                    ServerResponseOTP = jsonData["otp"].ToString();

                OTPVerificationMessage = jsonData["message"].ToString();

                if (ApploaderController.ApploaderController_Instance != null)
                {
                    ApploaderController.ApploaderController_Instance.Deactivate_Apploader();
                }
                if (LoginScenceController.LoginScenceController_Instance != null)
                    LoginScenceController.LoginScenceController_Instance.OTPDialog.SetActive(true);
            }
        }
    }
    #endregion

    #region [Submit Details After OTP]
    public void SubmitDetailsAfterOTP(string Email, string PhoneNumber, string Name, string DeviceId)
    {
        if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(PhoneNumber) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(DeviceId))
        {

            StartCoroutine(SubmitDetailsAfterOTPCallBack(Email, PhoneNumber, Name, DeviceId));
        }
    }

    IEnumerator SubmitDetailsAfterOTPCallBack(string Email, string PhoneNumber, string Name, string DeviceId)
    {
        WWWForm submitdetails_params = new WWWForm();
        submitdetails_params.AddField("phone", PhoneNumber);
        submitdetails_params.AddField("email", Email);
        submitdetails_params.AddField("name", Name);
        submitdetails_params.AddField("device_id", DeviceId);

        UnityWebRequest submitdetails = UnityWebRequest.Post(SubmitAfterOTPVerified, submitdetails_params);
        submitdetails.SetRequestHeader("Accept", "application/json");
        submitdetails.SetRequestHeader("Authorization", "Bearer " + AccessToken);

        yield return submitdetails.SendWebRequest();

        if (submitdetails.isNetworkError || submitdetails.isHttpError)
        {
            InternetConnection.InternetConnection_Instance.NetworkErrorPanel.SetActive(true);
            Debug.Log(submitdetails.error);
        }
        else
        {
            JsonData submitdetailsjsonData = JsonMapper.ToObject(submitdetails.downloadHandler.text.ToString());
            if (submitdetailsjsonData["message"].ToString().Equals("SUCCESS") || submitdetailsjsonData["message"].ToString().Equals("EXISTS"))
            {
                print("OTP Verified");
                PlayerPrefs.SetString("_SubmitedDetailsUserId", submitdetailsjsonData["app_user_id"].ToString());// Save User id in PlayerPrefs
                GetOrderValidity();
                SceneLoader.sceneLoader_Instance.SceneLoaderCallback("ChildDetalis");
            }
            else
            {
                print("Sorry!!!! Not submited after OTP varification");
            }
        }
    }
    #endregion

    #region [Get Child Grade Details]
    public void GetChildGradeDetails(string _LocalToken)
    {
        StartCoroutine(GetChildGradeDetailsCallBack(_LocalToken));
    }

    IEnumerator GetChildGradeDetailsCallBack(string LocalToken)
    {
        UnityWebRequest agedetails = UnityWebRequest.Get(GradeDetailsUrl);
        agedetails.SetRequestHeader("Accept", "application/json");
        agedetails.SetRequestHeader("Authorization", "Bearer " + LocalToken);
        yield return agedetails.SendWebRequest();
        if (agedetails.isNetworkError || agedetails.isHttpError)
        {
            print("Not getting age list!! Network error");
        }
        else
        {
            if (agedetails.isDone)
            {
                JsonData ageDetailListsarray = JsonMapper.ToObject(agedetails.downloadHandler.text.ToString());
                if (!string.IsNullOrEmpty(ageDetailListsarray.ToString()) && !string.IsNullOrEmpty(ageDetailListsarray.ToString()))
                {
                    PlayerPrefs.SetInt("_gradeslength", ageDetailListsarray.Count);
                    print("Grades length "+ PlayerPrefs.GetInt("_gradeslength"));
                    for (int i = 0; i<PlayerPrefs.GetInt("_gradeslength"); i++)
                    {
                        PlayerPrefs.SetString("_gradedetails" + i, ageDetailListsarray[i].ToString());
                        print(PlayerPrefs.GetString("_gradedetails" + i));
                    }
                }
                else
                {
                    Debug.Log("Network error in getting age list API...");
                }
            }
        }
    }
    #endregion

    #region [Submit Details After Given Child Details]
    public void Submit_ChildDetails_To_Server(string ChildName, string Grade, string UserId)
    {
        if (!string.IsNullOrEmpty(Grade) && !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(ChildName))
        {
            StartCoroutine(Submit_ChildDetails_To_ServerRequest(ChildName, Grade, UserId));
        }
    }

    IEnumerator Submit_ChildDetails_To_ServerRequest(string ChildName, string Grade, string UserId)
    {
        WWWForm submitChildDetails_params = new WWWForm();
        submitChildDetails_params.AddField("name", ChildName);
        submitChildDetails_params.AddField("age_group", Grade);
        submitChildDetails_params.AddField("app_user_id", UserId);

        UnityWebRequest submitdetails = UnityWebRequest.Post(SubmitAfterChildAgeDetails, submitChildDetails_params);
        submitdetails.SetRequestHeader("Accept", "application/json");
        submitdetails.SetRequestHeader("Authorization", "Bearer " + AccessToken);

        yield return submitdetails.SendWebRequest();

        if (submitdetails.isNetworkError || submitdetails.isHttpError)
        {
            InternetConnection.InternetConnection_Instance.NetworkErrorPanel.SetActive(true);
            Debug.Log(submitdetails.error);
            Debug.Log("Response ----" + submitdetails.downloadHandler.text);
        }
        else
        {
            JsonData submitChildDetailsjsonData = JsonMapper.ToObject(submitdetails.downloadHandler.text.ToString());
            if (submitChildDetailsjsonData["message"].ToString().Equals("SUCCESS") || submitChildDetailsjsonData["message"].ToString().Equals("EXISTS"))
            {
                print("Successfully submited after Child Details" + submitChildDetailsjsonData["child_id"].ToString() + "" + submitChildDetailsjsonData["age_group"].ToString());
                PlayerPrefs.SetString("_SubmitedDetailsUserId_ChildId", submitChildDetailsjsonData["child_id"].ToString());// Save Child Id in PlayerPrefs
                PlayerPrefs.SetString("_SubmitedDetailsUserId_AgeGroup", submitChildDetailsjsonData["age_group"].ToString());// Save Child Age Group in PlayerPrefs
            }
            else
            {
                
                print("Sorry!!!! Not WorksheetFound");
            }
        }
    }
    #endregion

    #region[Send worksheet JSON Request]
    public void Download_Worksheet_Method(int _gradeid)
    {
        StartCoroutine(Download_Worksheet(_gradeid));
    }
    public IEnumerator Download_Worksheet(int age_group_id)
    {
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        print(" Fetching fresh Json for current selected Grade ==============");
        WWWForm WorksheetJson_params = new WWWForm();
        WorksheetJson_params.AddField("age_group_id", age_group_id);
        UnityWebRequest worksheetJson_request = UnityWebRequest.Post(GetWorksheetJsonURl, WorksheetJson_params);
        worksheetJson_request.SetRequestHeader("Accept", "application/json");
        worksheetJson_request.SetRequestHeader("Authorization", "Bearer " + AccessToken);
        yield return worksheetJson_request.SendWebRequest();
        if (worksheetJson_request.isNetworkError || worksheetJson_request.isHttpError)
        {
            InternetConnection.InternetConnection_Instance.NetworkErrorPanel.SetActive(true);
            Debug.Log(worksheetJson_request.error);
        }
        else
        {
            JsonData jsonData = JsonMapper.ToObject(worksheetJson_request.downloadHandler.text.ToString());
            print(worksheetJson_request.downloadHandler.text.ToString());

            if (!string.IsNullOrEmpty(jsonData["json"].ToString()))
            {
                //var Json_url = jsonData["json"].ToString();
                var uwr = new UnityWebRequest(jsonData["json"].ToString(), UnityWebRequest.kHttpVerbGET);
                uwr.downloadHandler = new DownloadHandlerFile(Application.persistentDataPath + "/_MyStuff/Xplorabox/" + age_group_id + ".xplorabox");
                yield return uwr.SendWebRequest();
                while (!uwr.isDone)
                    yield return null;

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.LogError(uwr.error);
                    SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
                    if (WorksheetHomeController.WorksheetHomeController_Instance != null)
                    {
                        WorksheetHomeController.WorksheetHomeController_Instance.NoWorksheetFoundPanel.SetActive(true);
                    }
                }
                else
                {
                    if (uwr.isDone)
                    {
                        GetOrderValidity();
                        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
                        print("Json Downloaded for current filename" + age_group_id + ".xplorabox");
                    }
                }
            }
            else
            {
                SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
                if (WorksheetHomeController.WorksheetHomeController_Instance != null)
                { WorksheetHomeController.WorksheetHomeController_Instance.NoWorksheetFoundPanel.SetActive(true); }
            }
        }
    }
    #endregion

    #region [Get Language Details]
    public void GetLanguageDetails()
    {
        string LocalToken = PlayerPrefs.HasKey("_LocalAccessToken") ? PlayerPrefs.GetString("_LocalAccessToken") : null;
        if (LocalToken == null)
            StartCoroutine(GetAccessTokenFromServerCallBack(Grant_Type, Client_Id, Client_Secret, UserName, Password));
        else
            StartCoroutine(GetLanguageDetailsCallBack(LocalToken));
    }

    IEnumerator GetLanguageDetailsCallBack(string LocalToken)
    {
        UnityWebRequest languagedetails = UnityWebRequest.Get(LanguageDetailsUrl);
        languagedetails.SetRequestHeader("Accept", "application/json");
        languagedetails.SetRequestHeader("Authorization", "Bearer " + LocalToken);
        yield return languagedetails.SendWebRequest();
        if (languagedetails.isNetworkError || languagedetails.isHttpError)
        {
            print("Not getting age list!! Network error");
        }
        else
        {
            if (languagedetails.isDone)
            {
                JsonData LanguageDetailsListsarray = JsonMapper.ToObject(languagedetails.downloadHandler.text.ToString());
                if (!string.IsNullOrEmpty(LanguageDetailsListsarray.ToString()) && !string.IsNullOrEmpty(LanguageDetailsListsarray.ToString()))
                {
                    LanguageDetalList.Clear();
                    for (int i = 0; i < LanguageDetailsListsarray.Count; i++)
                    {
                        Debug.Log(LanguageDetailsListsarray[i].ToString());
                        LanguageDetalList.Add(LanguageDetailsListsarray[i].ToString());
                    }
                }
                else
                {
                    Debug.Log("Network error in getting age list API...");
                }
            }
        }
    }
    #endregion

    #region [Send Order Validation]
    public void GetOrderValidity()
    {
       
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString("_SubmitedDetailsUserId")))
        {
            StartCoroutine(SendOrderValidation(PlayerPrefs.GetString("_SubmitedDetailsUserId")));
        }
        else
        {
            PlayerPrefs.SetString("_OrderValidity", "EXPIRED");
            print("value is null and orer status is : " + PlayerPrefs.GetString("_OrderValidity"));
        }
     
    }
    #endregion

    #region[Send Order validation]
    public IEnumerator SendOrderValidation(string app_user_id)
    {
        print("App User Id is  "+ app_user_id);
        WWWForm orderValidation_params = new WWWForm();
        orderValidation_params.AddField("app_user_id", app_user_id);

        UnityWebRequest orderValidation_request = UnityWebRequest.Post(OrderValidationURl, orderValidation_params);
        orderValidation_request.SetRequestHeader("Accept", "application/json");
        orderValidation_request.SetRequestHeader("Authorization", "Bearer " + AccessToken);

        yield return orderValidation_request.SendWebRequest();

        if (orderValidation_request.isNetworkError || orderValidation_request.isHttpError)
        {
            InternetConnection.InternetConnection_Instance.NetworkErrorPanel.SetActive(true);
            print(orderValidation_request.downloadHandler.text);
            Debug.Log(orderValidation_request.error);
        }
        else
        {
            JsonData jsonData = JsonMapper.ToObject(orderValidation_request.downloadHandler.text.ToString());
            print("order has been " +orderValidation_request.downloadHandler.text.ToString());
            if (jsonData["plan"].ToString().Equals("NO_ORDER") || jsonData["plan"].ToString().Equals("EXPIRED"))
            {
                PlayerPrefs.SetString("_OrderValidity", "EXPIRED");
                print("Order has been Expired");
            }
            else
            {
                PlayerPrefs.SetString("_OrderValidity", "VALID");
                print("Order is still Valid");
            }
        }
    }
    #endregion

    #region[Delete Player Prefs]
    public void DeletePlayerPrefChildGrades()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("_gradeslength"); i++)
        {
            PlayerPrefs.DeleteKey("_gradedetails" + i);
        }
    }
    #endregion
    #endregion

}

