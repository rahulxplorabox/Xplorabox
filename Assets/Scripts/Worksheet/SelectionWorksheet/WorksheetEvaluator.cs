using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorksheetEvaluator : MonoBehaviour
{
    #region[Member variables]
   
    [Header("GameObjects")]
    public GameObject SubmitButtonContainer;
    public GameObject UndoButton;
    public GameObject ScanningEffectpanel;
    public GameObject WorksheetPanel;
    public GameObject LineDrawerContainer;
    public GameObject LineDraw;
    public GameObject SavetoGalleryButton;
    GameObject LastLineGameObject;
    GameObject[] MyDrawnAnswers;
    public string IsDeletable;
    public bool ifsumbitbtnclicked;
 


    [Header("Copyright Text")]
    public Text CopytightText;
    Animation anim;
    public static WorksheetEvaluator WorksheetEvaluator_instance;
    int RightAnswerCount = 0;
    int WrongAnswerCount=0;
    #endregion

    #region[Monobehaviour Methods]
    private void Awake()
    {
        if (WorksheetEvaluator_instance == null)
        {
            WorksheetEvaluator_instance = this;
        }
        ScanningEffectpanel.SetActive(false);
    }
    private void Start()
    {
        string CurrentYear = DateTime.Now.ToString("yyyy"); // Copyright text year from current date
        CopytightText.text = "Copyright © " + CurrentYear + " XploraBox. All rights reserved."; // Copyright text in worksheet
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Clear();
        swipetrial.swipetrial_instance.startLineDrawing = true;
    }
    #endregion

    #region[Custom Methods]
    #region[Submit Button]
    public void CheckSelectedImageAnswer()
    {
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Count > 0)
        {
            ifsumbitbtnclicked = true;
            LineDrawerContainer.SetActive(false);
            //LineDraw.SetActive(false);
            UndoButton.SetActive(false);
            SavetoGalleryButton.SetActive(false);
            ScanningEffectpanel.SetActive(true);
            Invoke("SubmitButton_control", 2.5f);
        }
    }

    public void SubmitButton_control()
    {
        if (ScanningEffectpanel.activeSelf){ScanningEffectpanel.SetActive(false);}
        print("SubmitButton Clicked");
        Animator anim2 = SubmitButtonContainer.GetComponent<Animator>();
        anim2.SetFloat("Direction", 1);
        anim2.Play("ResetButtonAnimi", -1, 0.3f);
        swipetrial.swipetrial_instance.startLineDrawing = false;
        #region [For OddOneOut Selection]
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "oddoneout")
        {
            foreach (KeyValuePair<GameObject, GameObject> item in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject)
            {
                item.Value.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        #endregion
        #region [For Matching]
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "matching")
        {
            foreach (GameObject mygame in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers)
            {
                if (mygame.gameObject.GetComponent<MatchingSelectedAfterDrawlineScripts>().CurrentDragObject == "Right")
                {
                    print(mygame.transform.GetComponent<Image>().sprite.name);
                    mygame.transform.GetChild(0).gameObject.SetActive(true);
                }
                if (mygame.gameObject.GetComponent<MatchingSelectedAfterDrawlineScripts>().CurrentDragObject == "Wrong")
                {
                    print(mygame.transform.GetComponent<Image>().sprite.name);
                    mygame.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }
        #endregion
        EvaluateAnswersAndSoundclips();
        WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.Activate_Animation();

    }
    #endregion

    #region [Reset button] 
    public void ResetButtonControl()
    {
        swipetrial.swipetrial_instance.count = 0;
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        print("Back button press & RESET");
        DestroyAllLines();
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Count > 0)
        {

            LineDrawerContainer.SetActive(false);
            foreach (KeyValuePair<GameObject, GameObject> item in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject)
            {
                item.Value.transform.GetChild(0).gameObject.SetActive(false);
                if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "matching")
                {
                    item.Value.transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            if (ifsumbitbtnclicked)
            {
                Animator anim = SubmitButtonContainer.GetComponent<Animator>();
                anim.SetFloat("Direction", -1);
                anim.Play("ResetButtonAnimi", -1, 1f);
            }
            CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Clear();

            if (WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.TapOnWorksheet)
            {
                WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.Deactivate_BottomScrollBar_and_WorksheetPanel();
            }
            CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Clear();
            LineDrawerContainer.SetActive(true);
            LineDraw.SetActive(true);
            UndoButton.SetActive(true);
            swipetrial.swipetrial_instance.startLineDrawing = true;
            SavetoGalleryButton.SetActive(true);
            ifsumbitbtnclicked = false;
        }
    }
    #endregion

    #region [Destroy DrawnLiness]
    public void DestroyAllLines()
    {
        foreach (Transform child in LineDrawerContainer.transform) { Destroy(child.gameObject); }
    }
    #endregion

    #region[back Button Control]
    public void OnBackButtonClick()
    {
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        print("Back button press");
        ApploaderController.ApploaderController_Instance.Activate_Apploader();  
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.rightpos.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.wrongpos.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Clear();
        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Clear();
        DestroyAllLines();
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
    }
    #endregion
    
    #region[Evaluating answers]
    public void EvaluateAnswersAndSoundclips()
    {   GameObject[] myanswers;
        #region [For OddOneOut Selection]
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "oddoneout")
        {
            myanswers = GameObject.FindGameObjectsWithTag("RightAnswer");
            for (int i = 0; i < myanswers.Length; i++)
                RightAnswerCount++;

            myanswers = GameObject.FindGameObjectsWithTag("WrongAnswer");

            for (int j = 0; j < myanswers.Length; j++)
                WrongAnswerCount++;
        }
        #endregion
        #region [For Matching]
        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "matching")
        {
            myanswers = GameObject.FindGameObjectsWithTag("RightAnswer");
            for (int i = 0; i < myanswers.Length; i++)
            {
                if (myanswers[i].transform.GetChild(0).gameObject.activeSelf == true)
                {
                    RightAnswerCount++;
                }
                if (myanswers[i].transform.GetChild(1).gameObject.activeSelf == true)
                {
                    WrongAnswerCount++;
                }
            }
        }
        #endregion

        print("Total Right Answers are===============" +RightAnswerCount);
        print("Total Wrong Answers are===============" + WrongAnswerCount);

        if (!RightAnswerCount.Equals(0) && !WrongAnswerCount.Equals(0))
        {
            print("Play_Random_TryAgainSoundClip()");
            SoundController.soundsControllerInstance.Play_Random_TryAgainSoundClip();
        }
        else
        {
            if (!RightAnswerCount.Equals(0) || WrongAnswerCount.Equals(0))
            {
                print("Play_Random_AllCorrectSoundClip();");
                SoundController.soundsControllerInstance.Play_Random_AllCorrectSoundClip();
            }
            if (RightAnswerCount.Equals(0) || !WrongAnswerCount.Equals(0))
            {
                print("Play_Random_AllInCorrectSoundClip();");
                SoundController.soundsControllerInstance.Play_Random_AllInCorrectSoundClip();
            }
        }
        RightAnswerCount = WrongAnswerCount=0;
    }


    #endregion

    #region[Undo_Btn]
    public void undoanswer()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        print("Undo button is clicked");
        MyDrawnAnswers = GameObject.FindGameObjectsWithTag("Finish");
        if (MyDrawnAnswers.Length != 0)
        {
            LastLineGameObject = MyDrawnAnswers[MyDrawnAnswers.Length - 1];
            foreach (KeyValuePair<GameObject, GameObject> pair in CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject)
            {
                if (pair.Key == LastLineGameObject)
                {
                    #region [For OddOneOut Selection]
                    if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "oddoneout")
                    {
                        GameObject im = pair.Value.transform.GetChild(0).gameObject;
                        im.SetActive(false);
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Remove(pair.Value);
                    }
                    #endregion
                    #region [For Matching]
                    if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.WorksheetType == "matching")
                    {
                        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Contains(pair.Value))
                        {
                            pair.Value.transform.GetChild(0).gameObject.SetActive(false);
                            CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Remove(pair.Value);
                        }
                    }
                    #endregion
                }
            }
            if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.ContainsKey(LastLineGameObject))
            {
                print("Line is remove from Dictonery" + LastLineGameObject.name);
                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Remove(LastLineGameObject);
                Destroy(LastLineGameObject);
            }
            else
            {
                Destroy(LastLineGameObject);
            }
        }
    }
    #endregion

    #endregion
}
