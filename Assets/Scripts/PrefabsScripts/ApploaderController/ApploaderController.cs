using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ApploaderController : MonoBehaviour
{
    #region[vars]
    [Header("App Loader Components")]
    public static ApploaderController ApploaderController_Instance;
    public GameObject AppLoader;
    public GameObject Retrypanel;
    public GameObject _404ErrorPanel;
    bool isRunning=false;
    float elapsedTime = 0.0f;
    #endregion

    #region[Monobehaviour Methods]
    private void Awake()
    {
        if (ApploaderController_Instance == null)
        {
            ApploaderController_Instance = this;
        }
        Deactivate_Apploader();
        _404ErrorPanel.SetActive(false);
    }

    public void Update()
    {
        #region[runs if Loader is Activated]
        if (isRunning)
        { 
            elapsedTime += Time.deltaTime;
            if (elapsedTime>20f)
            {
                isRunning = false;
                print("isRunning=false");
                AppLoader.SetActive(false);// setting loader off on time out
                if (SceneManager.GetActiveScene().name.ToString()=="WorksheetPlayScene")
                {
                    WorksheetEvaluator.WorksheetEvaluator_instance.WorksheetPanel.SetActive(false);
                }
                Retrypanel.SetActive(true);
                print("There is a network Error Please restart your app");
            }
        }
        #endregion
    }
    #endregion

    #region[Custom Methods]
    public void Deactivate_Apploader()
    {
        isRunning = false;
        Invoke("setAppLoaderactiveoff", 0.7f);
    }
    public void setAppLoaderactiveoff()
    {
       AppLoader.SetActive(false);
    }

    public void Activate_Apploader()
    {
        AppLoader.SetActive(true);
        isRunning = true;
    }
   

    #region[Retry Panel Button]
    public void RetryAppLoader_btn()
    {
        Retrypanel.SetActive(false);
        print("Current scene "+SceneManager.GetActiveScene().name.ToString());
        SceneManager.LoadScene(SceneManager.GetActiveScene().name.ToString());
    }
    #endregion

    #region[_404_Panel_Button]
    public void _404_Panel_Button()
    {
        Activate_Apploader();
        _404ErrorPanel.SetActive(false);
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
    }
    #endregion
    #endregion


}
