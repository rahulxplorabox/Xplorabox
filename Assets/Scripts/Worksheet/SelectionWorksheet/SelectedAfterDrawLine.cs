using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedAfterDrawLine : MonoBehaviour
{
    GraphicRaycaster raycaster;
    GameObject[] gameObjects;
    GameObject Line;

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
                #region[Adding values to Dictionary]
                print("Line and Current Selected GameObject has been added to the dictionary");
                if (!CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Contains(result.gameObject))
                {
                    print("Added into result");
                    if (!CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Contains(result.gameObject))
                    {
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Add(result.gameObject);
                        gameObjects = GameObject.FindGameObjectsWithTag("Finish");
                        if (gameObjects.Length != 0)
                        {
                            Line = gameObjects[gameObjects.Length - 1];
                        }
                        if (Line != null)
                        {
                            if (!CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.ContainsKey(Line))
                            {
                                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Add(Line, result.gameObject);
                            }
                        }
                    }
                }
                #endregion
            }
        }
    }
}
