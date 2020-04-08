using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AppUpdater : MonoBehaviour
{
    public GameObject AppUpdatePanel;
    private void Awake()
    {
        AppUpdatePanel.SetActive(true);
    }

    public void UpdateXplorabox_App()
    {
        print("Application.version===" + Application.version);
        Application.OpenURL("market://details?id=com.bts.xplorabox");
    }
}
