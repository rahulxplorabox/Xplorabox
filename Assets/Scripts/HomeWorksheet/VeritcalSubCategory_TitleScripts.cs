using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VeritcalSubCategory_TitleScripts : MonoBehaviour
{
    public RawImage CategoryImage;
    public string CategoryImageURl;
    string cachefolder_path;

   
    #region[ImageSetter]
    public IEnumerator Start()
    {
        cachefolder_path = Application.persistentDataPath + "/_cachefolder/";
        yield return null;

        string Worksheet_Imagefilename = Path.GetFileName(CategoryImageURl).ToString().Split('.')[0];
        if (Is_Cachefile(Worksheet_Imagefilename))
        {
            CategoryImage.texture = LoadImages_fromCache(Worksheet_Imagefilename);
        }
        else
        {
            #region[Load and Download Image]
            print(" LoadingTextureFromURL" + Path.GetFileName(CategoryImageURl));
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(CategoryImageURl);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }
            else
            {
                try
                {
                    Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                    CategoryImage.texture = myTexture;
                    byte[] bytes = ((Texture2D)myTexture).EncodeToPNG();
                    File.WriteAllBytes(cachefolder_path + Path.GetFileName(CategoryImageURl).ToString().Split('.')[0], bytes);
                    print(Path.GetFileName(CategoryImageURl) + " Downloaded");
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



}
