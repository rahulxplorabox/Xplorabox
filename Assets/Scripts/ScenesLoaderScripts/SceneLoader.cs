using LitJson;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region 
    [Header("STATIC_MEMBER_VARIABLE")]
    public static SceneLoader sceneLoader_Instance;
    public string LastVisitDate;
    public GameObject AppUpdatePanel;
    public float AppVersionCode;
    bool ShowAppUpdatePanel;
    static string Apiversion_URL = "https://app.xplorabox.com/api/get_app_version";
    #endregion

    #region MONOBEHAVIOUR_METHODS
    private void Awake()
    {
        if (sceneLoader_Instance == null)
        {
            sceneLoader_Instance = this;
        }
        AppUpdatePanel.SetActive(false);

    }

    private void Start()
    {
        StartCoroutine(SplashScreen_SceneValidations(2f));
    }

    #endregion

    #region[Custom_methods]

    #region[Loading Next Scenes]
    public void SceneLoaderCallback(string SceneName)
    {
        print("Scene Loaded " + SceneName);
        StartCoroutine(LoadYourAsyncScene(SceneName));
    }

    IEnumerator LoadYourAsyncScene(string _scenename)
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(_scenename);
    }
    public string CurrentLoadedsceneName()
    {
        return SceneManager.GetActiveScene().name.ToString();
    }
    #endregion

    #endregion

    #region[Splash Screen Validation]
    IEnumerator SplashScreen_SceneValidations(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        CheckApp_VersionMethod();

    }
    #endregion

    #region[AppUpdate_Controls]

    #region[Check app version]
    public void CheckApp_VersionMethod()
    {
        StartCoroutine(CheckApp_Version());
    }

    IEnumerator CheckApp_Version()
    {
        WWWForm form = new WWWForm();
        form.AddField("device_id", SystemInfo.deviceUniqueIdentifier.ToString());
        UnityWebRequest AppVersion = UnityWebRequest.Post(Apiversion_URL, form);
        AppVersion.SetRequestHeader("Accept", "application/json");
        yield return AppVersion.SendWebRequest();
        if (AppVersion.isHttpError || AppVersion.isNetworkError)
        {
            print("There is a problem in app version check" + AppVersion.error);
        }
        else
        {
            if (AppVersion.isDone)
            {
                print("responce==========" + AppVersion.downloadHandler.text.ToString());
                JsonData jsonData = JsonMapper.ToObject(AppVersion.downloadHandler.text.ToString());
                if (!string.IsNullOrEmpty(jsonData.ToString()) && !string.IsNullOrEmpty(jsonData["version"].ToString()))
                {
                    print("Current App version " + Application.version);
                    print("PlayConsole App Version " + jsonData["version"].ToString());
                    AppVersionCode = float.Parse(jsonData["version"].ToString());
                    if (float.Parse(Application.version) != AppVersionCode)
                    {
                        AppUpdatePanel.SetActive(true);

                    }
                    else
                    {
                        if ((bool)jsonData["maintenance_mode"])
                        {
                            print("it's true");
                            MaintenancePanel.MaintenancePanel_intance.ShowEmergencyPanel();
                        }
                        else
                        {
                            proceed_to_homescreen();
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region[UpdateXplorabox__Button control]
    public void UpdateXplorabox__app()
    {
        Application.OpenURL("market://details?id=com.bts.xplorabox");
    }
    #endregion
    #endregion

    #region[Proceed_to_HomeScreen]
    public void proceed_to_homescreen()
    {
        if (!string.IsNullOrEmpty(WebServiceController.webServiceController_Instance.AccessToken))
        {
            print("version Matched");
            if (PlayerPrefs.GetString("_finalChildNameDetails") != "")
            {
                WebServiceController.webServiceController_Instance.GetOrderValidity();
                print("Already Logged In");
                SceneLoaderCallback("WorksheetHomePage");
            }
            else
            {
                print("Version Mismatched, Please update the app");
                SceneLoaderCallback("LoginScene");
            }
        }
    }
    #endregion
}







