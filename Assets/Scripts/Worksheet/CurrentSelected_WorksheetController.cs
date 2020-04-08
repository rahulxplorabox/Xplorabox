using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentSelected_WorksheetController : MonoBehaviour
{
    public static CurrentSelected_WorksheetController CurrentSelected_WorksheetControllerInstance;
  
    public List<Vector3> rightpos;
    public List<Vector3> wrongpos;

    public List<GameObject> AllAnswersGameObject;
    public Dictionary<GameObject,GameObject> MySelectedGameObject = new Dictionary<GameObject, GameObject>();
    public List<GameObject> Myselected_Answers;

    public GameObject RightSideGameObject;////******
    public string CurrentDragObjectName;////******
    public string CurrentDragObjectSide;////******

    public string ImageURL;
    public string CategoryTitleLang_Image_URL;
    public string Title_Text_Image_URL;
    public string CategoryQuestionText_Lang;
    public string Title_Text;
    public string WorksheetType;////******
    public static int MainCategoryIndex, SubCategoryIndex, WorksheetIndex;
    private void Awake()
    {
        if (CurrentSelected_WorksheetControllerInstance == null)
        {
            CurrentSelected_WorksheetControllerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    public void OnBackButtonClick()
    {
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
        AllAnswersGameObject.Clear();
        MySelectedGameObject.Clear();
    }
}

