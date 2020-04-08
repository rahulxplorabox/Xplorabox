using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDialogController : MonoBehaviour
{

    #region [GameObjects]
    public GameObject ExitDialoguePanel;
    #endregion

    private void Awake()
    {
        ExitDialoguePanel.SetActive(false);
    }

    private void Update()
    {
       if (Input.GetKeyUp(KeyCode.Escape))
       {
            ExitDialoguePanel.SetActive(true);
       }
    }
    
    public void ExitDialog_Yes_Btn()
    {
        ExitDialoguePanel.SetActive(false);
        Application.Quit();
    }
    public void ExitDialog_NoBtn()
    {
        ExitDialoguePanel.SetActive(false);
    }
}
