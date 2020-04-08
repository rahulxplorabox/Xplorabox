using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChildDetailsControler : MonoBehaviour
{
    #region[Member Variables]
    public static ChildDetailsControler ChildDetailsControler_Instance;
    [Header("Child Details Variable")]
    public Dropdown DropDownClassSelection;
    public InputField ChildNameInputField;
    string childName;
    int selectedgradeindex;
    string userId;
    string selectedgrade;

    public GameObject NoWorksheetFound;
    #endregion

    #region[MonoBehaviour Methods]
    private void Awake()
    {
        if(ChildDetailsControler_Instance == null)
        {
            ChildDetailsControler_Instance = this;
        }
        NoWorksheetFound.SetActive(false);
        userId = PlayerPrefs.GetString("_SubmitedDetailsUserId");
    }

    void Start()
    {
        AddGradeDetails();
    }
    #endregion

    #region[Custom Methods]
    
    #region[Add child grade detail in dropdown]
    public void AddGradeDetails()
    {
        for (int i = 0; i<PlayerPrefs.GetInt("_gradeslength"); i++)
        {
            DropDownClassSelection.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("_gradedetails" + i) });
        }
    }
    #endregion

    #region[Add Child Grade Details selected into polayerfabs]
    public void AddGradeDetailsIntoPlayerfabs()
    {
        print("_finalSelectedGrade"+DropDownClassSelection.options[DropDownClassSelection.value].text.ToString());
        PlayerPrefs.SetString("_finalSelectedGradeString", DropDownClassSelection.options[DropDownClassSelection.value].text.ToString());
        PlayerPrefs.SetInt("_finalSelectedGrade", DropDownClassSelection.value-1);
    }
    #endregion

    #region[SubmitChildDetailsBtnClicked] 

    public void SubmitChildDetailsBtnClicked()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        if (InternetConnection.InternetConnection_Instance != null && InternetConnection.InternetConnection_Instance.CheckInternet())
        {
            selectedgrade = DropDownClassSelection.value.ToString();
            selectedgradeindex = DropDownClassSelection.value;
            if (!string.IsNullOrEmpty(ChildNameInputField.text) && !string.IsNullOrEmpty(selectedgrade) && selectedgradeindex >= 1)
            {
                if (ChildNameInputField.text.Length >= 3)
                {
                    print("child name validated");
                    childName = ChildNameInputField.text.ToString();
                    PlayerPrefs.SetInt("_SelectedGradeIndex", selectedgradeindex);
                    PlayerPrefs.SetString("_finalChildNameDetails", childName);
                    print("Selected grade (Playerprefs) " + PlayerPrefs.GetString("_SelectedGradeIndex") + "_finalChildNameDetails " + PlayerPrefs.GetString("_finalChildNameDetails"));
                    WebServiceController.webServiceController_Instance.Submit_ChildDetails_To_Server(childName, selectedgrade, userId);

                    selectedgradeindex = DropDownClassSelection.value;
                    if (selectedgradeindex != 0)
                        WebServiceController.webServiceController_Instance.Download_Worksheet_Method(selectedgradeindex - 1);// Downloading Worksheet for the current grade for the first time 
                    else
                        print("select valid grade details");
                }
                else
                {
                    childnameerror();
                }
            }
            else
            {
                if (string.IsNullOrEmpty(childName))
                {
                    childnameerror();
                }
                if (selectedgradeindex < 1)
                {
                    Animator anim = DropDownClassSelection.GetComponent<Animator>();
                    anim.Play("WrongInputFieldShake");
                }
            }
        }
    }
    #region
    public void childnameerror()
    {
        ChildNameInputField.text = "";
        ChildNameInputField.GetComponent<InputField>().placeholder.GetComponent<Text>().text = "Please Enter a Valid Name";
        ChildNameInputField.placeholder.GetComponent<Text>().color = Color.red;
        Animator anim = ChildNameInputField.GetComponent<Animator>();
        anim.Play("WrongInputFieldShake");
    }
    #endregion
    #endregion
    public void CloseNoWorkSheetpopup()
    {
        NoWorksheetFound.SetActive(false);
    }
    #endregion
}
