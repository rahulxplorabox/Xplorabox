using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkseetPlay_AnimationController : MonoBehaviour
{
    [Header("Animation Gameobjects")]
    public GameObject WorksheetPanel;
    public GameObject BottomScrollBar;
    public bool TapOnWorksheet = false;
    Animator WorksheetPanel_animator, BottomScrollBar_animator;
    public static WorkseetPlay_AnimationController workseetPlay_AnimationControllerInstance;

    private void Awake()
    {
        if (workseetPlay_AnimationControllerInstance == null)
        {
            workseetPlay_AnimationControllerInstance = this;
        }
    }
    void Start()
    {
        WorksheetPanel_animator = WorksheetPanel.GetComponent<Animator>();
        BottomScrollBar_animator = BottomScrollBar.GetComponent<Animator>();
    }
    #region[WorksheetPanel Animation]

    #region[Activate_Anim_BottomScrollBar_and_WorksheetPanel]
    public void Activate_Animation()
    {
        Invoke("Activate_Anim_BottomScrollBar_and_WorksheetPanel", 1f);
    }
    public void Activate_Anim_BottomScrollBar_and_WorksheetPanel()
    {
        WorksheetPanel_animator.SetFloat("C_Direction", 1);
        WorksheetPanel_animator.Play("PlayScene_CanvasScallingAnimation", -1, 1f);
        BottomScrollBar_animator.SetFloat("B_Direction", 1);
        BottomScrollBar_animator.Play("BottomScrollbaranimation", -1, 0.3f);
        TapOnWorksheet = true;
    }
    #endregion
    public void Deactivate_BottomScrollBar_and_WorksheetPanel()
    {
        WorksheetPanel_animator.SetFloat("C_Direction", -1);
        WorksheetPanel_animator.Play("PlayScene_CanvasScallingAnimation", -1, 1f);
        BottomScrollBar_animator.SetFloat("B_Direction", -1);
        BottomScrollBar_animator.Play("BottomScrollbaranimation", -1, 1f);
        TapOnWorksheet = false;
    }
   
    #endregion

   
}
