using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController_PurchaseScene : MonoBehaviour
{
    public enum TabType  {worksheetSubscription,Webview }
    public  TabType tabType;
    public void TabButtonClickListener()
    {
        switch (tabType)
        {
            case TabType.worksheetSubscription:
            print("You have clicked WorksheetSubscription");
            break;

            case TabType.Webview:
            print("You have clicked Webview");
            OpenWebView();
            break;
        }
    }

    public void OpenWebView()
    {
        string Pagetopen = "https://www.xplorabox.com/mega-special-offer";
        print("Opening web view with link "+Pagetopen);
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.backButtonText = "Close";
        options.displayURLAsPageTitle = false;
        options.pageTitle = "Xplorabox";
        InAppBrowser.OpenURL(Pagetopen, options);
    }

    public void backButtonController()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        print("Back button press");
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
    }
   

}
