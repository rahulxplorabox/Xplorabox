using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DrawLineAllow : MonoBehaviour
{
    GraphicRaycaster raycaster;

    void Awake()
    {
        // Get both of the components we need to do this
        this.raycaster = GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.TapOnWorksheet)
                {
                    WorkseetPlay_AnimationController.workseetPlay_AnimationControllerInstance.Deactivate_BottomScrollBar_and_WorksheetPanel();
                    Invoke("ShowAnswerMarks", .9f);
                }
                Debug.Log(result.gameObject.name);
                if (result.gameObject.name == "WorksheetImage")
                {
                    swipetrial.swipetrial_instance.linedrawenable = true;
                }
                else
                {
                    swipetrial.swipetrial_instance.linedrawenable = false;
                }
            }
        }
        else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButtonUp(0))
        {
            swipetrial.swipetrial_instance.linedrawenable = false;
        }
    }
    void ShowAnswerMarks()
    {
        WorksheetEvaluator.WorksheetEvaluator_instance.LineDrawerContainer.SetActive(true);
        WorksheetEvaluator.WorksheetEvaluator_instance.SavetoGalleryButton.SetActive(true);
    }
}
