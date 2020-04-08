using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SetupWorksheets : MonoBehaviour
{
    #region [SetupWorksheets Vars]

    public static SetupWorksheets SetupWorksheetsAnswers_Instance;
    GameObject Imageicons;
    public GameObject myimageParent;
    [Header("odd_oneout_prefab")]
    public GameObject SelectionRightAnsPrefab;
    public GameObject SelectionWrongeAnsPrefab;

    [Header("MatchingAnsPrefab_prefab")]
    public GameObject MatchingAnsPrefab;
    public GameObject DemoVideoPanel;
    bool helpbutton_clicked;
    
    public string url;
    public Image worksheetImage;
    public List<Vector3> WorksheetAnswerPointPosition = new List<Vector3>();

    string CategoryTitleLang;
    public string Title;

    public Image CategoryTitleLangImage;
    public string CategoryTitleLangImage_URl;
    public Image TitleImage;
    public string TitleImage_URL;
    public string WorksheetTpye;



    int count = 0;
    int count1 = 0;

    #endregion


    #region[Test Vars]
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public string _current_gradeAge;
    public int _current_mainCategoryIndex;
    #endregion

    #region[Monobehaviour Methods]
    private void Awake()
    {
        _current_gradeAge = PlayerPrefs.GetString("_finalSelectedGradeString");
        _current_mainCategoryIndex = PlayerPrefs.GetInt("_MainCategoryIndexId");
        print("Current selected grade age is "+ _current_gradeAge);

        print("Current  SelectedgradeMainCategoryIndex  is " + _current_mainCategoryIndex);
        if (SetupWorksheetsAnswers_Instance == null)
        {
            SetupWorksheetsAnswers_Instance = this;
        }
      

        CategoryTitleLang = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CategoryQuestionText_Lang.ToString();
        url = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.ImageURL;
        CategoryTitleLangImage_URl = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CategoryTitleLang_Image_URL;
        TitleImage_URL = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Title_Text_Image_URL;
        WorksheetTpye = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType;


        DemoVideoPanel.SetActive(false);
        helpbutton_clicked = false;
        print(PlayerPrefs.GetInt("_MainCategoryIndexId"));
        print(PlayerPrefs.GetString("_finalSelectedGradeString") + " (Selected)");

    }

    void Start()
    {
        if (WorksheetTpye != "drawing")
        {
            WorksheetEvaluator.WorksheetEvaluator_instance.LineDrawerContainer.SetActive(false);
            WorksheetEvaluator.WorksheetEvaluator_instance.LineDraw.SetActive(false);
        }
        if (PlayerPrefs.GetString("Videoforcurrent_Image").Equals(_current_gradeAge + _current_mainCategoryIndex))
            InitiateWorksheet_view();
        else
            Show_DemoVideoPanel();
    }

    void InitiateWorksheet_view()
    {
        print("Shown video already so setting up worksheets for you ");
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        if (WorksheetTpye != "drawing")
        {
            WorksheetEvaluator.WorksheetEvaluator_instance.LineDrawerContainer.SetActive(false);
            WorksheetEvaluator.WorksheetEvaluator_instance.LineDraw.SetActive(false);
        }
        StartCoroutine(LoadImagesfromURL());
    }
    #endregion
    #region[Customs methods]

    #region[Load Images from URL]
    IEnumerator LoadImagesfromURL()
    {
        yield return new WaitForEndOfFrame();
        Title = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Title_Text.ToString();
        StartCoroutine(setImageFromURl(CategoryTitleLangImage, CategoryTitleLangImage_URl));
        StartCoroutine(setImageFromURl(TitleImage, TitleImage_URL));
        if (WorksheetTpye != "drawing")
        {
            StartCoroutine(setImageFromURl(worksheetImage, url));
        }
        else
        {
            ColoringController.ColoringController_Instance.InitWithRandomColoringPage();
        }
    }
    #endregion

    #region[Set Image From URL]
    IEnumerator setImageFromURl(Image imageName, string imageurl)
    {
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        yield return new WaitForEndOfFrame();
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageurl);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            print(www.error);
            if (WorksheetTpye != "drawing")
            {
                WorksheetEvaluator.WorksheetEvaluator_instance.LineDrawerContainer.SetActive(false);
                WorksheetEvaluator.WorksheetEvaluator_instance.LineDraw.SetActive(false);
            }
            ApploaderController.ApploaderController_Instance.Deactivate_Apploader();
            ApploaderController.ApploaderController_Instance._404ErrorPanel.SetActive(true);
        }
        else
        {
            if (www.isDone)
            {
                ApploaderController.ApploaderController_Instance.Activate_Apploader();
                Texture2D tex = new Texture2D(1, 1);
                tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
                imageName.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
                if (imageName.sprite != null && imageName.name == "WorksheetImage")
                {
                    print("Set : " + imageName);
                    InitGoogletexttospeech.InitGoogletexttospeech_intance.SynthesizeButtonOnClickHandler(CategoryTitleLang.ToString(), (int)InitGoogletexttospeech.Speaker_gender.Girl);
                    instantiate();
                }
            }
        }

    }
    #endregion

    #region[Instantiate all Right / Wrong Answer Position]
    public void instantiate()
    {
        WorksheetEvaluator.WorksheetEvaluator_instance.LineDrawerContainer.SetActive(true);
        WorksheetEvaluator.WorksheetEvaluator_instance.LineDraw.SetActive(true);
        ApploaderController.ApploaderController_Instance.Deactivate_Apploader();

        #region [WorksheetType OddOneOut Selection]
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "oddoneout")
        {
            foreach (Vector3 i in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.rightpos)
            {
                Imageicons = Instantiate(SelectionRightAnsPrefab);
                Imageicons.transform.position = i;
                Imageicons.transform.SetParent(myimageParent.transform, false);
                Imageicons.transform.name = count++.ToString();
                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Add(SelectionRightAnsPrefab);
            }
            foreach (Vector3 i in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.wrongpos)
            {
                Imageicons = Instantiate(SelectionWrongeAnsPrefab);
                Imageicons.transform.position = i;
                Imageicons.transform.SetParent(myimageParent.transform, false);
                Imageicons.transform.name = count1++.ToString();
                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Add(SelectionWrongeAnsPrefab);
            }
        }
        #endregion

        #region [WorksheetType Matching]
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "matching")
        {
            foreach (Vector3 i in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.rightpos)
            {
                Imageicons = Instantiate(MatchingAnsPrefab);
                Imageicons.transform.position = i;
                Imageicons.transform.SetParent(myimageParent.transform, false);
                Imageicons.transform.name = "Left-" + count++.ToString();
            }
            foreach (Vector3 i in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.wrongpos)
            {
                Imageicons = Instantiate(MatchingAnsPrefab);
                Imageicons.transform.position = i;
                Imageicons.transform.SetParent(myimageParent.transform, false);
                Imageicons.transform.name = "Right-" + count1++.ToString();
                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Add(MatchingAnsPrefab);
            }
        }
        #endregion
    }
    #endregion

    #region[DemopanelController]
    #region[Show_VideoPanle]
    public void Show_DemoVideoPanel()
    {
        print("Showing video first time");
        DemoVideoPanel.transform.GetChild(1).GetComponentInChildren<Text>().text = "Skip";
        DemoVideoPanel.SetActive(true);
        StartCoroutine(PlayVideo());
        Invoke("Change_skipbuttontext",10f);
    }

        IEnumerator PlayVideo()
        {
            videoPlayer.Prepare();
            WaitForSeconds waitForSeconds = new WaitForSeconds(1);
            while (!videoPlayer.isPrepared)
            {
                yield return waitForSeconds;
                break;
            }
            rawImage.enabled = true;
            rawImage.texture = videoPlayer.texture;
            videoPlayer.Play();
        }

    #endregion

    public void Demopanelcontroller_SkipBtn()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        CancelInvoke("Change_skipbuttontext");
        DemoVideoPanel.transform.GetChild(0).transform.GetComponent<RawImage>().enabled = false;
        DemoVideoPanel.SetActive(false);
        if (!helpbutton_clicked)
        {
            PlayerPrefs.SetString("Videoforcurrent_Image", _current_gradeAge + _current_mainCategoryIndex);
            InitiateWorksheet_view();
        }
        if (WorksheetTpye != "drawing")
        {
            swipetrial.swipetrial_instance.startLineDrawing = true;
        }
    }

    public void Demopanelcontroller_HelpBtn()
    {
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        helpbutton_clicked = true;
        if (WorksheetTpye != "drawing")
        {
            WorksheetEvaluator.WorksheetEvaluator_instance.ResetButtonControl();
            swipetrial.swipetrial_instance.startLineDrawing = false;
        }
        else
        {
            DrawingWorksheetEvaluator.DrawingWorksheetEvaluator_instance.ResetButtonControl();
        }
        Show_DemoVideoPanel();
    }

    public void Change_skipbuttontext()
    {
        DemoVideoPanel.transform.GetChild(1).GetComponentInChildren<Text>().text = "Lets start!";
    }
    #endregion

    #endregion
}