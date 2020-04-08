using System;
using System.Collections.Generic;
using UnityEngine;
using pl.ayground;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DrawingWorksheetEvaluator : MonoBehaviour
{
    #region[Member variables]

    public List<Vector2> MyColoredArea;

    [Header("GameObjects")]
    public GameObject SubmitButtonContainer;
    public GameObject UndoButton;
    public GameObject ColoringImage;
    public GameObject WorksheetPanel;
    public GameObject SavetoGalleryButton;
    public GameObject ColorContainer;
    private DrawableTextureContainer imageContainer;
    public string IsDeletable;
    public bool ifsumbitbtnclicked;

    [Header("Copyright Text")]
    public Text CopytightText;
    Animation anim;
    public static DrawingWorksheetEvaluator DrawingWorksheetEvaluator_instance;
    int RightCount = 0;
    int count = 0;
    Vector2 undoitem;
    #endregion

    #region[Monobehaviour Methods]
    private void Awake()
    {
        if (DrawingWorksheetEvaluator_instance == null)
        {
            DrawingWorksheetEvaluator_instance = this;
        }
    }
    private void Start()
    {
        string CurrentYear = DateTime.Now.ToString("yyyy"); // Copyright text year from current date
        CopytightText.text = "Copyright © " + CurrentYear + " XploraBox. All rights reserved."; // Copyright text in worksheet
        MyColoredArea.Clear();
    }
    #endregion

    #region[Custom Methods]
    #region[Submit Button]
    public void CheckSelectedImageAnswer()
    {
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        if (MyColoredArea.Count > 0)
        {
            ifsumbitbtnclicked = true;
            ColoringImage.transform.GetComponent<ColoringController>().enabled = false;
            ColorContainer.SetActive(false);
            UndoButton.SetActive(false);
            SavetoGalleryButton.SetActive(false);
            Invoke("SubmitButton_control", 0.5f);
        }
    }

    public void SubmitButton_control()
    {
        print("SubmitButton Clicked");
        Animator anim2 = SubmitButtonContainer.GetComponent<Animator>();
        anim2.SetFloat("Direction", 1);
        anim2.Play("ResetButtonAnimi", -1, 0.3f);
        EvaluateAnswersAndSoundclips();
        WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.Activate_Animation();

    }
    #endregion

    #region [Reset button] 
    public void ResetButtonControl()
    {
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        ColoringImage.transform.GetComponent<ColoringController>().enabled = true;
        ColorContainer.SetActive(true);
        print("Back button press & RESET");
        if (MyColoredArea.Count > 0)
        {
            if (ifsumbitbtnclicked)
            {
                Animator anim = SubmitButtonContainer.GetComponent<Animator>();
                anim.SetFloat("Direction", -1);
                anim.Play("ResetButtonAnimi", -1, 1f);
            }
            foreach(Vector2 item in MyColoredArea)
            {
                ColoringController.ColoringController_Instance.setcolor((int)item.x, (int)item.y, Color.white);
            }
            if (WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.TapOnWorksheet)
            {
                WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.Deactivate_BottomScrollBar_and_WorksheetPanel();
            }
            MyColoredArea.Clear();
            UndoButton.SetActive(true);
            SavetoGalleryButton.SetActive(true);
            ifsumbitbtnclicked = false;
        }
    }
    #endregion

    #region[back Button Control]
    public void OnBackButtonClick()
    {
        InitGoogletexttospeech.InitGoogletexttospeech_intance.Stop_GoogleTextToSpeech_button();
        SoundController.soundsControllerInstance.PlayButtonsound();
        print("Back button press");
        DrawingWorksheetEvaluator_instance.MyColoredArea.Clear();
        ApploaderController.ApploaderController_Instance.Activate_Apploader();
        SceneLoader.sceneLoader_Instance.SceneLoaderCallback("WorksheetHomePage");
    }
    #endregion

    #region[Evaluating answers]
    public void EvaluateAnswersAndSoundclips()
    {
        //#endregion
        RightCount = MyColoredArea.Count;
        print("Total Right Answers are===============" + RightCount);

        if (!RightCount.Equals(0))
        {
            print("Play_Random_TryAgainSoundClip()");
            SoundController.soundsControllerInstance.Play_Random_TryAgainSoundClip();
        }
        else
        {
            if (!RightCount.Equals(0))
            {
                print("Play_Random_AllCorrectSoundClip();");
                SoundController.soundsControllerInstance.Play_Random_AllCorrectSoundClip();
            }
            if (RightCount.Equals(0))
            {
                print("Play_Random_AllInCorrectSoundClip();");
                SoundController.soundsControllerInstance.Play_Random_AllInCorrectSoundClip();
            }
        }
        RightCount = 0;
    }
    #endregion

    #region[Undo_Btn]
    public void undoanswer()
    {
        SoundController.soundsControllerInstance.PlayButtonsound();
        print("Undo button is clicked");
        if (MyColoredArea.Count > 0)
        {
            print("Undo button is clicked");
            foreach (Vector2 item in MyColoredArea)
            {
                count++;
                print("Undo button is clicked");
                if (MyColoredArea.Count == count)
                {
                    print("Undo button is clicked");
                    ColoringController.ColoringController_Instance.setcolor((int)item.x, (int)item.y, Color.white);
                    undoitem = item;
                }
            }
            removeitem(undoitem);
            count = 0;
        }
        }

    public void removeitem(Vector2 item)
    {
       MyColoredArea.Remove(item);
    }
    #endregion
    #endregion
}