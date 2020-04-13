using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class WorksheetHomeController : MonoBehaviour
{
    public static WorksheetHomeController WorksheetHomeController_Instance;

    #region [Variables and Members]
    [Header("Json Contents")]
    string JsonData;
    string Jsonfolderpath;
    string Jsonfilename;
    public RootObject xplorasmartsheet;


    [Header("Json File Info")]
    string LastVisitDate;
    string LastVisitDateInPlayerPrefs;


    [Header("Worksheet Order and Language")]
    public ScrollRect MyVerticalScrollrect;
    public int languageindex;
    public string lang;
    public string userorder;
    public GameObject unlockDialog;
    public GameObject NoWorksheetFoundPanel;
    public string DefaultImageUrl;



    [Header("Worksheet Content")]
    GameObject[] WorksheetgameObjects;
    public string selectedlanguage;
    public Dropdown DropDownClassSelection;
    public int UserGrade;

    [Header("Scrollbars")]
    public GameObject TopHorizontalCategoryScrollView;
    public GameObject VerticalSubCategoryScrollView;

    [Header("Top MainCategory Prefab")]
    public GameObject TopMainCategoryPrefab;
    GameObject Initiated_TopMainCategoryPrefab;

    [Header("Veritcal SubCategory Scrollview Prefab")]
    public GameObject VeritcalSubCategoryScrollviewPrefab;
    GameObject Instantiated_VeritcalSubCategoryScrollviewPrefab;

    [Header("Worksheet Element Prefab")]
    public GameObject WorksheetElementPrefab;
    GameObject myWorksheetElement;

    [Header("Paid Worksheet Element Prefab")]
    public GameObject PaidWorksheetElementPrefab;
    GameObject myPaidWorksheetElement;
    public bool CallingFromSameScene = false;
    #endregion

    #region[MonoBehavior Methods]
    void Awake()
    {
        if (WorksheetHomeController_Instance == null)
        {
            WorksheetHomeController_Instance = this;
        }
       languageindex = 0;
        lang = "en";
        Jsonfolderpath = Application.persistentDataPath + "/_MyStuff/Xplorabox/";
        Jsonfilename = PlayerPrefs.GetInt("_finalSelectedGrade") + ".xplorabox";
    }

    private void Start()
    {
        DropDownClassSelection.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("_finalSelectedGradeString") + " (Selected)" });
        AddGradeDetails();
        Initialize();
    }

    #endregion

    #region[Initialize]
    public void Initialize()
    {
        #region[checkfilestatus()]
        if (checkfilestatus())
        {
            print("checkfilestatus()===========checking JSON");
            JsonData = File.ReadAllText(Jsonfolderpath + Jsonfilename);
            if (File.GetCreationTime(Jsonfolderpath + Jsonfilename).ToString("MM/dd") != System.DateTime.Now.ToString("MM/dd"))
            {
                print("xxxxxxxxxxxxxxxxxxxxxxxxxxx============JSON FILE OUTDATED...");
                #region[Deleting oldjson]
                if (File.Exists(Application.persistentDataPath + "/_MyStuff/Xplorabox/" + PlayerPrefs.GetInt("_finalSelectedGrade") + ".xplorabox"))
                {
                    print("xxxxxxxxxxxxxxxxxxxxxxxxxxx============OLD FILE FOUND...");
                    try
                    {
                        File.Delete(Application.persistentDataPath + "/_MyStuff/Xplorabox/" + PlayerPrefs.GetInt("_finalSelectedGrade") + ".xplorabox");
                        print("xxxxxxxxxxxxxxxxxxxxxxxxxxx============File  Deleted Successfully...");
                    }
                    catch (UnityException exception)
                    {
                        Debug.Log("Exception found while deleting " + exception.ToString());
                    }
                }
                #endregion

                print("Json file is outdated so requesting new Json");
                WebServiceController.webServiceController_Instance.Download_Worksheet_Method(PlayerPrefs.GetInt("_finalSelectedGrade"));
            }
            else
            {
                print("JSON FILE IS UP TP DATE...");
                if (!string.IsNullOrEmpty(JsonData))
                {
                    xplorasmartsheet = JsonUtility.FromJson<RootObject>(JsonData);
                    #region [intantiating Main Category prefab accrodingly]
                    print("Length of Main Category " + xplorasmartsheet.root.Length.ToString());
                    for (int m = 0; m < xplorasmartsheet.root.Length; m++)
                    {
                        Initiated_TopMainCategoryPrefab = Instantiate(TopMainCategoryPrefab);
                        Initiated_TopMainCategoryPrefab.name = "myMainCategoryCollection_" + m;
                        Initiated_TopMainCategoryPrefab.transform.SetParent(TopHorizontalCategoryScrollView.transform, false);
                        for (int l = 0; l < xplorasmartsheet.root[m].category.Length; l++)
                        {
                            string[] language_URL = xplorasmartsheet.root[m].category[l].ToString().Split('-');
                           
                            #region[settingDefaultImageURL]     
                            if (m == 0 &&PlayerPrefs.GetInt("_MainCategoryIndexId").Equals(0))                           
                            {
                                print("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxMain Category index has been set to zero");
                                DefaultImageUrl = language_URL[1];
                                PlayerPrefs.SetString("_MainCategoryIndexIdImageURL", DefaultImageUrl);
                                PlayerPrefs.SetInt("_MainCategoryIndexId", 0);
                            }
                            #endregion

                            Initiated_TopMainCategoryPrefab.GetComponent<MainCategoryController>().MainCategoryPrefab_ImageURl = language_URL[1];
                            Initiated_TopMainCategoryPrefab.GetComponent<MainCategoryController>().MainCategoryPrefab_id = m;
                        }
                    }
                    #endregion
                    #region[Initiating SubCategory Prefab Worksheets]
                    subCategoryInstantiat(PlayerPrefs.GetInt("_MainCategoryIndexId"));
                    #endregion
                }
            }
        }
        else
        {
            NoWorksheetFoundPanel.SetActive(true);
        }
        #endregion
    }
    #endregion

    #region[Check file status]
    bool checkfilestatus()
    {
        if (Directory.Exists(Jsonfolderpath) && File.Exists(Jsonfolderpath + Jsonfilename))
        {
            return true;
        }
        else
            Directory.CreateDirectory(Jsonfolderpath);
        return false;
    }
    #endregion

    #region[Add Child Grade Details In DropDown]
    public void AddGradeDetails()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("_gradeslength"); i++)
        {
            DropDownClassSelection.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("_gradedetails" + i) });
        }
    }
    #endregion

    #region [On Grade change Event]
    public void OnGradeChange()
    {
        WebServiceController.webServiceController_Instance.ChangeGradeIndex = DropDownClassSelection.value;
        WebServiceController.webServiceController_Instance.ChangeGradeIndexValue = DropDownClassSelection.options[DropDownClassSelection.value].text.ToString();
        PlayerPrefs.SetString("_finalSelectedGradeString", DropDownClassSelection.options[DropDownClassSelection.value].text.ToString());
        PlayerPrefs.SetInt("_finalSelectedGrade", DropDownClassSelection.value - 1);
        Jsonfilename = PlayerPrefs.GetInt("_finalSelectedGrade") + ".xplorabox";
        print("You tried to access file name" + Jsonfilename);
        PlayerPrefs.SetInt("_MainCategoryIndexId", 0);
        if (!File.Exists(Jsonfolderpath + Jsonfilename))
        {
            print(Jsonfilename + " file is not  here");
            print("Downloading the file of current selected grade " + PlayerPrefs.GetString("_finalSelectedGradeString") + "with index " + PlayerPrefs.GetInt("_finalSelectedGrade"));
            WebServiceController.webServiceController_Instance.Download_Worksheet_Method(PlayerPrefs.GetInt("_finalSelectedGrade"));//print("on grade change home page download");

        }
        else
        {
            print(Jsonfilename + " file is here");
            ApploaderController.ApploaderController_Instance.Activate_Apploader();
            SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
        }
    }
    #endregion

    #region [Event Onclick on worksheet]
    public void OnWorksheetClick()
    {
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetPlayScene");
    }
    #endregion

    #region [VerticalSubCategoryScrollView  Instantiated]
    public void subCategoryInstantiat(int mainCategoryIndex)
    {
        #region [Destroy_Current_VerticalSubCategoryScrollViewItems]
        foreach (Transform child in VerticalSubCategoryScrollView.transform)
        {
            Destroy(child.gameObject);
        }
        #endregion
        CallingFromSameScene = true;
        for (int i = 0; i < xplorasmartsheet.root[mainCategoryIndex].category_data.Length; i++)
        {
            #region[ Instantiated_VeritcalSubCategoryScrollviewPrefab with image]
            Instantiated_VeritcalSubCategoryScrollviewPrefab = Instantiate(VeritcalSubCategoryScrollviewPrefab);
            Instantiated_VeritcalSubCategoryScrollviewPrefab.transform.SetParent(VerticalSubCategoryScrollView.transform, false);
            Instantiated_VeritcalSubCategoryScrollviewPrefab.name = "Subcategory " + i;
            for (int l = 0; l < xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category.Length; l++)
            {
                string[] lan=xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category[l].ToString().Split('-');
                if (lan[0] == lang)
                {
                    Instantiated_VeritcalSubCategoryScrollviewPrefab.GetComponent<VeritcalSubCategory_TitleScripts>().CategoryImageURl = lan[1];
                }
            }
            #endregion
            
            #region[Instantiate Worksheet according to Sub Category]
            Initiate_WorksheetAccordingToCategory(mainCategoryIndex, i, Instantiated_VeritcalSubCategoryScrollviewPrefab, CallingFromSameScene,0);
            #endregion
        }

    }
    #endregion

    #region[Initiate_WorksheetAccordingToCategory]
    public void Initiate_WorksheetAccordingToCategory(int mainCategoryIndex,int subcategory ,GameObject ScrollView_Container,bool samescene_call,int Current_selected_worksheet)
    {
        for (int j = 0; j < xplorasmartsheet.root[mainCategoryIndex].category_data[subcategory].sub_category_data.Length; j++)
        {
            #region[Instantiate Worksheet according to Sub Category]
            if (xplorasmartsheet.root[mainCategoryIndex].category_data[subcategory].sub_category_data[j].type.ToString() == "free")
            {
                InstantiatFreeWorksheet(mainCategoryIndex, subcategory, j, false, ScrollView_Container, samescene_call, Current_selected_worksheet);
            }
            else
            {
                if (xplorasmartsheet.root[mainCategoryIndex].category_data[subcategory].sub_category_data[j].type.ToString() == "paid")
                {
                    //if (PlayerPrefs.GetString("_OrderValidity") == "EXPIRED")//free playerfabs
                    //{
                        print("userorder == EXPIRED");
                        myPaidWorksheetElement = Instantiate(PaidWorksheetElementPrefab);
                        if (samescene_call) { myPaidWorksheetElement.transform.SetParent(ScrollView_Container.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform, false); }
                        else
                        {
                            myPaidWorksheetElement.transform.localScale =new Vector3(.8f,.8f,.8f);
                            myPaidWorksheetElement.transform.SetParent(ScrollView_Container.transform, false);
                            if (j == Current_selected_worksheet)
                            {
                                myWorksheetElement.transform.GetChild(3).gameObject.SetActive(true);
                            }
                        }
                        myPaidWorksheetElement.name = "Worksheet " + j;
                        myPaidWorksheetElement.GetComponent<PaidWorksheetPrefab_Controller>().Paid_thumbnail = xplorasmartsheet.root[mainCategoryIndex].category_data[subcategory].sub_category_data[j].thumbnail;
                        myPaidWorksheetElement.GetComponent<PaidWorksheetPrefab_Controller>().Paid_worksheeturl = xplorasmartsheet.root[mainCategoryIndex].category_data[subcategory].sub_category_data[j].url;
                   // }
                    //else
                    //{
                    //    if (PlayerPrefs.GetString("_OrderValidity") == "VALID")//free playerfabs
                    //    {
                    //        print("userorder == VALID");
                    //        InstantiatFreeWorksheet(mainCategoryIndex, subcategory, j, true, ScrollView_Container, samescene_call, Current_selected_worksheet);
                    //    }
                    //}
                }
            }
            #endregion
        }
    }
    #endregion
    
    #region [Instantiate Worksheet Category Position and all other]
    public void InstantiatFreeWorksheet(int mainCategoryIndex, int i, int j, bool premium,GameObject _scrollbarContainer,bool same_scene_Call,int _Currentworksheet)
    {
        #region[InstantiatFreeWorksheet]
        myWorksheetElement = Instantiate(WorksheetElementPrefab);
        if (same_scene_Call) { myWorksheetElement.transform.SetParent(_scrollbarContainer.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform, false); }
        else
        {
            _scrollbarContainer.transform.localPosition = new Vector3(-(_Currentworksheet - 1) * 210, 0, 0);
            myWorksheetElement.transform.localScale = new Vector3(.8f, .8f, .8f);
            myWorksheetElement.transform.SetParent(_scrollbarContainer.transform, false);
            if (j == _Currentworksheet) {myWorksheetElement.transform.GetChild(3).gameObject.SetActive(true);}
        }
        myWorksheetElement.name = "Worksheet" + j;
        myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().WorksheetDetails_MainCategoryIndex = mainCategoryIndex;
        myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().WorksheetDetails_SubCategoryIndex = i;
        myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().WorksheetDetails_WorksheetIndex = j;
        myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().Thumbnail = xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].thumbnail;
        myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().Worksheeturl = xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].url;

        myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().WorksheetTpye = xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j]._script;////******

        if (premium) { myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().premiumorder = true; }
       
        #endregion

        #region[Fectching and setting other details]
        #region[Set Worksheet Title Language Text]
        if (xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].title_lang.Length > 0)
        {
            for (int l = 0; l < xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].title_lang.Length; l++)
            {
                string[] Title_lang = xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].title_lang[l].ToString().Split('-');
                if (Title_lang[0] == lang)
                {
                    myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().Title_Text = Title_lang[1];
                }
            }
        }
        #endregion

        #region[Set Worksheet Category QuestionText_Lang]
        if (xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].text_lang.Length > 0)
        {
            for (int l = 0; l < xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].text_lang.Length; l++)
            {
                string[] CategoryQuestionText_Lang = xplorasmartsheet.root[mainCategoryIndex].category_data[i].sub_category_data[j].text_lang[l].ToString().Split('-');
                if (CategoryQuestionText_Lang[0] == lang)
                {
                    myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().CategoryQestionText = CategoryQuestionText_Lang[1];
                }
            }
        }
        #endregion
        #region[Fectching and setting Worksheet details]
        fetching_worksheetDetails(mainCategoryIndex, i, j);
        #endregion
        #endregion
    }
    public void fetching_worksheetDetails(int _mainCategoryIndex, int _i, int _j)
    {
        
        #region[Set Worksheet TitleTextURL]
        if (xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].title_lang_url.Length > 0)
        {
            for (int l = 0; l < xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].title_lang_url.Length; l++)
            {
                string[] TitleTextURL = xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].title_lang_url[l].ToString().Split('-');
                if (TitleTextURL[0] == lang)
                {
                    myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().Title_Text_URL = TitleTextURL[1];
                }
            }
        }
        #endregion

        #region[set CategoryQuestionText_URL]
        if (xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].text_lang_url.Length > 0)
        {
            for (int l = 0; l < xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].text_lang_url.Length; l++)
            {
                string[] CategoryQuestionText_URL = xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].text_lang_url[l].ToString().Split('-');
                if (CategoryQuestionText_URL[0] == lang)
                {
                    myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().CategoryQuestionText_URL = CategoryQuestionText_URL[1];
                }
            }
        }
        #endregion

        #region[Set Position of Answer] 
        try
        {
            if (xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].positions.right.Length > 0) //Set Position of Right Answer
            {
                for (int k = 0; k < xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].positions.right.Length; k++)
                {
                    string[] pos = xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].positions.right[k].ToString().Split(',');
                    myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().rightpos.Add(new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }
            if (xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].positions.wrong.Length > 0) //Set Position of Wrong Answer
            {
                for (int l = 0; l < xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].positions.wrong.Length; l++)
                {
                    string[] pos = xplorasmartsheet.root[_mainCategoryIndex].category_data[_i].sub_category_data[_j].positions.wrong[l].ToString().Split(',');
                    myWorksheetElement.GetComponent<FreeWorksheetElementPrefab_Controller>().wrongpos.Add(new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2])));
                }
            }
        }
        catch (Exception e)
        {
            print("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!There is something wrong in Backend Positioning" +e.ToString());
        }
        #endregion
    }
    #endregion

    #region [Update new version]

    public void UpdateNewVersionJson()
    {
        #region[Deleting oldjson]
        if (File.Exists(Application.persistentDataPath + "/_MyStuff/Xplorabox/" + PlayerPrefs.GetInt("_finalSelectedGrade") + ".xplorabox"))
        {
            print("xxxxxxxxxxxxxxxxxxxxxxxxxxx============OLD FILE FOUND...");
            try
            {
                File.Delete(Application.persistentDataPath + "/_MyStuff/Xplorabox/" + PlayerPrefs.GetInt("_finalSelectedGrade") + ".xplorabox");
                print("xxxxxxxxxxxxxxxxxxxxxxxxxxx============File  Deleted Successfully...");
                WebServiceController.webServiceController_Instance.Download_Worksheet_Method(PlayerPrefs.GetInt("_finalSelectedGrade"));
            }
            catch (UnityException exception)
            {
                Debug.Log("Exception found while deleting " + exception.ToString());
            }
        }
        #endregion
    }

    #endregion

}

