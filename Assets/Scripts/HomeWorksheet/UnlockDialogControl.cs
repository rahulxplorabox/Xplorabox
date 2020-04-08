using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDialogControl : MonoBehaviour
{
    public GameObject UnlockDialog_Panel;

    public string Pagetopen= "https://www.xplorabox.com/mega-special-offer";
    // Start is called before the first frame update
    private void Awake()
    {
        UnlockDialog_Panel.SetActive(false);
    }

    public void OpenWebView()
    {
        UnlockDialog_Panel.SetActive(false);
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
        options.backButtonText = "Close";
        options.displayURLAsPageTitle = false;
        options.pageTitle = "Xplorabox";
        InAppBrowser.OpenURL(Pagetopen, options);
    }
    public void CloseButtonClick()
    {
        UnlockDialog_Panel.SetActive(false);
        
    }
}
