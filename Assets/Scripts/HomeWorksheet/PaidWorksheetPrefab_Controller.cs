using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class PaidWorksheetPrefab_Controller : MonoBehaviour
{
    public RawImage PaidWorksheetImage;
    public string Paid_thumbnail;

    public string Paid_worksheeturl;
    string cachefolder_path;


    #region[Monobehavior Methods]

    #region[ImageSetter]
    public IEnumerator Start()
    {
        cachefolder_path = Application.persistentDataPath + "/_cachefolder/";
        yield return null;
        string Worksheet_Imagefilename = Path.GetFileName(Paid_thumbnail).ToString().Split('.')[0];
        if (Is_Cachefile(Worksheet_Imagefilename))
        {
            PaidWorksheetImage.texture = LoadImages_fromCache(Worksheet_Imagefilename);
        }
        else
        {
            #region[Load and Download Image ]
            print(" LoadingTextureFromURL" + Path.GetFileName(Paid_thumbnail));
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(Paid_thumbnail);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }
            else
            {
                try
                {
                    Texture2D myTexture = new Texture2D(32, 32, TextureFormat.Alpha8, false);
                    myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    PaidWorksheetImage.texture = myTexture;

                    byte[] bytes = myTexture.EncodeToJPG();
                    File.WriteAllBytes(cachefolder_path + Path.GetFileName(Paid_thumbnail).ToString().Split('.')[0], bytes);
                    print(Path.GetFileName(Paid_worksheeturl) + " Downloaded");
                }
                catch (Exception e)
                {
                    print("Exception while Downloaing Image " + e.ToString());
                }
                if (www.isDone)
                {
                    www.Dispose();
                    www = null;
                }
            }
            #endregion
        }
    }

    #region [Check if file exist in Cache_Memory]
    public bool Is_Cachefile(string Imagename_received)
    {
        string Images_Files_Location = cachefolder_path;
        if (File.Exists(Images_Files_Location + "/" + Imagename_received))
            return true;
        else
            return false;
    }
    #endregion
    #endregion

    #region [Load_cache_Image]
    public Texture LoadImages_fromCache(string imagename_received)
    {
        print("Loading image " + imagename_received + " from cache");
        string Images_Files_Location = cachefolder_path;
        byte[] myimage = File.ReadAllBytes(@Images_Files_Location + "/" + imagename_received);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(myimage);
        return texture;
    }
    #endregion

    #endregion
    public void onButtonClick()
    {
        //GameObject.Find("UnlockDialogControl").transform.GetChild(0).gameObject.SetActive(true);
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("PurchasingScene");
    }
}
