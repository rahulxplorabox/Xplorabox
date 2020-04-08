using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class FreeWorksheetElementPrefab_Controller : MonoBehaviour
{
    #region[Fields]
    public GameObject PremiumYellowIcon;
    public GameObject FreeBlueIcon;
    public bool premiumorder;
    public RawImage worksheetRawImage;
    public List<Vector3> rightpos;
    public List<Vector3> wrongpos;
    public string Title_Text;
    public string CategoryQestionText;
    public string Title_Text_URL;
    public string CategoryQuestionText_URL;
    public string Thumbnail;
    public string Worksheeturl;

    public string WorksheetTpye;////******

    public int WorksheetDetails_MainCategoryIndex, WorksheetDetails_SubCategoryIndex, WorksheetDetails_WorksheetIndex;

    string cachefolder_path;
    #endregion

    #region[Monobehavior Methods]

        #region[ImageSetter]
        public IEnumerator Start()
        {
            cachefolder_path = Application.persistentDataPath + "/_cachefolder/";
            yield return null;
            if(premiumorder)
            {
                PremiumUser();
            }

            string Worksheet_Imagefilename = Path.GetFileName(Thumbnail).ToString().Split('.')[0];
            if (Is_Cachefile(Worksheet_Imagefilename))
            {
                worksheetRawImage.texture = LoadImages_fromCache(Worksheet_Imagefilename);
            }
            else
            {
                #region[Load and Download Images]
                print(" LoadingTextureFromURL" + Path.GetFileName(Thumbnail));
                UnityWebRequest www = UnityWebRequestTexture.GetTexture(Thumbnail);
                yield return www.SendWebRequest();
                if (www.isNetworkError || www.isHttpError) { Debug.Log(www.error); }
                else
                {
                    try
                    {
                        Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                        worksheetRawImage.texture = myTexture;
                        byte[] bytes = ((Texture2D)myTexture).EncodeToPNG();
                        File.WriteAllBytes(cachefolder_path + Path.GetFileName(Thumbnail).ToString().Split('.')[0], bytes);
                        print(Path.GetFileName(Thumbnail) + " Downloaded");
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
        #endregion

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
   
    #region [On Click on Subcategory Worksheet]
    public void onButtonClick()
    {

        CurrentSelected_WorksheetController.MainCategoryIndex = WorksheetDetails_MainCategoryIndex;
        CurrentSelected_WorksheetController.SubCategoryIndex = WorksheetDetails_SubCategoryIndex;
        CurrentSelected_WorksheetController.WorksheetIndex = WorksheetDetails_WorksheetIndex;
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Title_Text = Title_Text;
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CategoryQuestionText_Lang = CategoryQestionText;
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Title_Text_Image_URL = Title_Text_URL;
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CategoryTitleLang_Image_URL = CategoryQuestionText_URL;
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.ImageURL = Worksheeturl;

        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType = WorksheetTpye;////******

        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.rightpos.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.wrongpos.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Clear();
        for (int i = 0; i < rightpos.Count; i++)
        {
            CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.rightpos.Add(rightpos[i]);
        }
        for (int i = 0; i < wrongpos.Count; i++)
        {
            CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.wrongpos.Add(wrongpos[i]);
        }
            SceneManager.LoadScene("WorksheetPlayScene", LoadSceneMode.Single);

        if (WorksheetTpye == "drawing")
        {
            SceneManager.LoadScene("DrawingWorksheetScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("WorksheetPlayScene", LoadSceneMode.Single);
        }
    }
    #endregion

    #region[Premium Activate]
    public void PremiumUser()
    {
        PremiumYellowIcon.SetActive(true);
        FreeBlueIcon.SetActive(false);
    }
    #endregion
}