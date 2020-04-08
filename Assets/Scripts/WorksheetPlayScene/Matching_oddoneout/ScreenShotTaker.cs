using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class ScreenShotTaker : MonoBehaviour
{
    #region 
    public GameObject upperImage;
    public GameObject bottomImage;
    public GameObject Toast;
    public RectTransform UIElement;
    public int width;
    public int height;
    public string PathToSave;
    string TimeStamp, fileName;
    string currentImageName;
    #endregion

    #region[Monobehaviour Methods]
    private void Awake()
    {
        PathToSave = Application.persistentDataPath + "/ScreenShots/";
        if (!Directory.Exists(PathToSave))
        {
            Directory.CreateDirectory(PathToSave);
            print("Directory created");
        }
        Toast.SetActive(false);
    }
    private void Start()
    {
        width = Convert.ToInt32(UIElement.rect.width);
        height = Convert.ToInt32(UIElement.rect.height);
    }
    #endregion

    #region[ScreenShotonbuttonclick]
    public void ScreenShotonbuttonclick()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) || Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            StartCoroutine(TakeScreenShot());
        }
        else
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            print("Allow the storage permission");
        }
    }
    #endregion

    #region[TakeScreenShot]
    public IEnumerator TakeScreenShot()
    {
        #region[Enabling Top and Border Cover Images Before Screeshot]
        print("Worksheet Name: " + PlayerPrefs.GetString("_finalSelectedGradeString"));
        currentImageName = PlayerPrefs.GetString("_finalSelectedGradeString") + "_" + CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Title_Text.ToLower();
        upperImage.SetActive(true);
        bottomImage.SetActive(true);
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType != "drawing")
        {
            WorksheetEvaluator.WorksheetEvaluator_instance.ResetButtonControl();
        }
        else
        {
            DrawingWorksheetEvaluator.DrawingWorksheetEvaluator_instance.ColorContainer.SetActive(false);
        }

        #endregion

        #region[Saving Image to Gallery]
        yield return new WaitForEndOfFrame(); // it must be a coroutine
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        screenTexture.Apply();
        byte[] dataToSave = screenTexture.EncodeToPNG();
        TimeStamp = DateTime.Now.ToString("dd-MM-yyyy");
        fileName = currentImageName + TimeStamp + ".png";
        File.WriteAllBytes(PathToSave + fileName, dataToSave);
        print("File " + fileName + " saved in " + PathToSave);
        NativeGallery.SaveImageToGallery(screenTexture, "Xplorabox", fileName, null);
        ActivateImageSavingtoGalleryToast();
        #endregion

        #region[Disabling Top and Border Cover Images Before Screeshot]
        print("screen shot done");
        upperImage.SetActive(false);
        bottomImage.SetActive(false);
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "drawing")
        {
            DrawingWorksheetEvaluator.DrawingWorksheetEvaluator_instance.ColorContainer.SetActive(true);
        }

        #endregion
    }
    #endregion

    #region[Toast Controlls]
    public void ActivateImageSavingtoGalleryToast()
    {
        Toast.SetActive(true);
        Invoke("Deactivate_Active", 2f);
    }

    void Deactivate_Active()
    {
        Toast.SetActive(false);
    }
    #endregion

}

