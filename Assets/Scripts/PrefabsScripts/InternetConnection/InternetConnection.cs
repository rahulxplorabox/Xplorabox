using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InternetConnection : MonoBehaviour
{
    public GameObject NetworkErrorPanel;
    public GameObject SomethingWentWrong_Panel;
    public static InternetConnection InternetConnection_Instance;
   #region[Monobehaviour_Methods]

    private void Awake()
    {
        if(InternetConnection_Instance == null)
        {
            InternetConnection_Instance = this;
        }
        NetworkErrorPanel.SetActive(false);
    }

    private void Update()
    {

        if (!CheckInternet())
        {
            print("No internet found");
            NetworkErrorPanel.SetActive(true);
        }
    }
    #endregion

    #region[Custom Methods]
    public void OnRetryClick()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        if (CheckInternet())
        {
            DeactivateNetworkErrorPanel();
        }
       
    }

    public bool CheckInternet()
    {
        return (Application.internetReachability != NetworkReachability.NotReachable);
    }

    public void DeactivateNetworkErrorPanel()
    {
        NetworkErrorPanel.SetActive(false);
        if (SceneManager.GetActiveScene().name.Contains("WorksheetHomePage") || SceneManager.GetActiveScene().name.Contains("WorksheetPlayScene"))
        {
            SceneLoader.sceneLoader_Instance.SceneLoaderCallback(SceneLoader.sceneLoader_Instance.CurrentLoadedsceneName());
        }
    }

    public void Activate_SomethingWentWrong_Panel()
    {
        SomethingWentWrong_Panel.SetActive(true);
    }
    #endregion

    


}
