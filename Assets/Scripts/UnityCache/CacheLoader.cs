using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CacheLoader : MonoBehaviour
{
    [Header("Cache Loader member variables")]
    public static CacheLoader CacheLoader_Instance;
    string cachefolder_path, XploraboxFolder;


    #region[Monobehaviour Behaviour]
    void Awake()
    {
        if (CacheLoader_Instance == null)
        {
            CacheLoader_Instance = this;
        }
        foldercheck();
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


    public void ApplyTexture(string imageURL, RawImage _Image)
    {
        string Worksheet_Imagefilename = Path.GetFileName(imageURL).ToString().Split('.')[0];
        if (Is_Cachefile(Worksheet_Imagefilename))
        {
            _Image.texture = LoadImages_fromCache(Worksheet_Imagefilename);
        }
        else
        {
           StartCoroutine(LoadTextureAndDownloadFromURL(imageURL, _Image));
        }

    }
    #endregion

    #region[Custom Methods]

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

   
    #region[LoadImages from URL]

    IEnumerator LoadTextureAndDownloadFromURL(string url, RawImage rawImage)
    {
        print(" LoadingTextureFromURL"+ Path.GetFileName(url));
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }
        else
        {
            try
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                rawImage.texture = myTexture;
                byte[] bytes = ((Texture2D)myTexture).EncodeToPNG();
                File.WriteAllBytes(cachefolder_path + Path.GetFileName(url).ToString().Split('.')[0], bytes);
                print(Path.GetFileName(url) + " Downloaded");
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
    }
    #endregion

    #endregion

    #region [Load_cache_Image]
    public Texture LoadImages_fromCache(string imagename_received)
    {
        print("Loading image "+ imagename_received + " from cache");
        string Images_Files_Location = cachefolder_path;
        byte[] myimage = File.ReadAllBytes(@Images_Files_Location + "/" + imagename_received);
        Texture2D texture = new Texture2D(1, 1);
        return texture;
    }
    #endregion


}
