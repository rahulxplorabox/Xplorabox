using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentselectedSubcategoryScrollController : MonoBehaviour
{
    public GameObject  PlayScene_ScrollviewContainer;
    public int MainCat, Subcat, Worksheetindex;

    void Start()
    {
        MainCat = CurrentSelected_WorksheetController.MainCategoryIndex;
        Subcat= CurrentSelected_WorksheetController.SubCategoryIndex;
        Worksheetindex = CurrentSelected_WorksheetController.WorksheetIndex;
        print("Workssheetindex is ==========================================="+ Worksheetindex);
        WorksheetHomeController.WorksheetHomeController_Instance.Initiate_WorksheetAccordingToCategory(MainCat, Subcat, PlayScene_ScrollviewContainer, false, Worksheetindex);
    }
}
