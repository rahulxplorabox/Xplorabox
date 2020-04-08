using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MatchingSelectedAfterDrawlineScripts : MonoBehaviour
{
    GraphicRaycaster raycaster;

    GameObject[] gameObjects;
    GameObject Line;
    bool isMatchEnable = false;


    string CurrentSelectedDragObjectName, CurrentSelectedDragObjectSide;
    string MouseButtonDownSelectedDragObjectName, MouseButtonDownSelectedDragObjectSide;

    public string CurrentDragObject;
    void Awake()
    {
        // Get both of the components we need to do this
        this.raycaster = GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMatchEnable = true;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log(result.gameObject.GetComponent<Image>().sprite.name);
                Debug.Log(result.gameObject.name);
             
                //if selected image is not in it then add select image in MySelectedGameObject
                if (isMatchEnable)
                {
                    string[] CategoryQuestionText_Lang = result.gameObject.name.ToString().Split('-');
                    MouseButtonDownSelectedDragObjectName = CategoryQuestionText_Lang[1];
                    MouseButtonDownSelectedDragObjectSide = CategoryQuestionText_Lang[0];

                    print(MouseButtonDownSelectedDragObjectSide+ "5445324565432145345243245645"+ MouseButtonDownSelectedDragObjectName);
                    if (MouseButtonDownSelectedDragObjectSide == "Left")
                    {
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.RightSideGameObject = result.gameObject;
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectName = MouseButtonDownSelectedDragObjectName;
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectSide = MouseButtonDownSelectedDragObjectSide;
                    }
                    if (MouseButtonDownSelectedDragObjectSide == "Right")
                    {
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.RightSideGameObject = result.gameObject;
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectName = MouseButtonDownSelectedDragObjectName;
                        CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectSide = MouseButtonDownSelectedDragObjectSide;
                    }
                }
            }
        }

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                Debug.Log(result.gameObject.GetComponent<Image>().sprite.name);
                Debug.Log(result.gameObject.name);

                string[] CategoryQuestionText_Lang = result.gameObject.name.ToString().Split('-');
                CurrentSelectedDragObjectSide = CategoryQuestionText_Lang[0];
                CurrentSelectedDragObjectName = CategoryQuestionText_Lang[1];

                if (isMatchEnable)
                {
                    print(CurrentDragObject + "***************" + result.gameObject.name);
                    print(CurrentSelectedDragObjectSide+"*******55555555555555*******"+ CurrentSelectedDragObjectName+"*****555555555************"+ CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectSide);
                    if (CurrentSelectedDragObjectSide != CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectSide)
                    {
                        if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectName == CurrentSelectedDragObjectName)
                        {
                            if (!CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Contains(result.gameObject))
                            {
                                //Add Right Answer 
                                print("Add Right Answer into list............");
                                if(CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectSide == "Right")
                                {
                                    GameObject SelectGameObject = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.RightSideGameObject;
                                    CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Add(SelectGameObject);
                                    SelectGameObject.GetComponent<MatchingSelectedAfterDrawlineScripts>().CurrentDragObject = "Right";
                                    AddIntoDictonery(result, SelectGameObject);
                                }
                                else
                                {
                                    CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Add(result.gameObject);
                                    result.gameObject.GetComponent<MatchingSelectedAfterDrawlineScripts>().CurrentDragObject = "Right";
                                    AddIntoDictonery(result, result.gameObject);
                                }
                            }
                        }
                        else if (!CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.AllAnswersGameObject.Contains(result.gameObject))
                        {
                            //Add Wronge Answer
                            if (CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.CurrentDragObjectSide == "Right")
                            {
                                GameObject SelectGameObject = CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.RightSideGameObject;
                                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Add(SelectGameObject);
                                SelectGameObject.GetComponent<MatchingSelectedAfterDrawlineScripts>().CurrentDragObject = "Wrong";
                                AddIntoDictonery(result, SelectGameObject);
                            }
                            else
                            {
                                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.Myselected_Answers.Add(result.gameObject);
                                result.gameObject.GetComponent<MatchingSelectedAfterDrawlineScripts>().CurrentDragObject = "Wrong";
                                AddIntoDictonery(result, result.gameObject);
                            }
                        }
                    }
                else
                    {
                        Destroy(Line);
                    }
                }
            else
            {
                Destroy(Line);
            }
            }
            isMatchEnable = false;
        }
    }

    public void AddIntoDictonery(RaycastResult result, GameObject SelectedGAmeobject)
    {
        gameObjects = GameObject.FindGameObjectsWithTag("Finish");
        if (gameObjects.Length != 0)
        {
            Line = gameObjects[gameObjects.Length - 1];
        }
        if (Line != null)
        {
            if (!CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.ContainsKey(Line))
            {
                print("Added into Distonery............");
                CurrentSelected_WorksheetController.CurrentSelected_WorksheetControllerInstance.MySelectedGameObject.Add(Line, SelectedGAmeobject);
            }
        }
    }
}
