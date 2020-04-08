using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZoomEfffect : MonoBehaviour
{
    float initialFingersDistance;
    Vector3 MinZoomScale;
    Vector3 MaxZoomScale;
    GraphicRaycaster raycaster;
    Vector3 initialScale;
    private void Awake()
    {
        this.raycaster = GetComponent<GraphicRaycaster>();
    }
    // Start is called before the first frame update
    void Start()
    {
        MinZoomScale = new Vector3(1f, 1f, 1f);
        MaxZoomScale = new Vector3(2f, 2f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        ZoomEffect();
    }
    public void ZoomEffect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            List<RaycastResult> results = new List<RaycastResult>();

            pointerData.position = Input.mousePosition;
            this.raycaster.Raycast(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "WorksheetImage")
                {
                    if (DrawingWorksheetEvaluator.DrawingWorksheetEvaluator_instance.ColoringImage.transform.GetComponent<ColoringController>().enabled == false)
                    {
                        DrawingWorksheetEvaluator.DrawingWorksheetEvaluator_instance.ResetButtonControl();
                    }
                }
            }
        }
        //if (Input.touches.Length == 2)
        //{
        //    print("Zooming it ");
        //    Touch t1 = Input.touches[0];

        //    Touch t2 = Input.touches[1];
        //    if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
        //    {
        //        initialFingersDistance = Vector2.Distance(t1.position, t2.position);
        //        initialScale = transform.localScale;
        //        print("Ye le initial scale" + initialScale);
        //    }
        //    else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
        //    {
        //        var currentFingersDistance = Vector2.Distance(t1.position, t2.position);
        //        var scaleFactor = currentFingersDistance / initialFingersDistance;
        //        var finalscale = initialScale * scaleFactor;
        //        print("Scale Factor " + scaleFactor);
        //        transform.localScale = finalscale;
        //        if (finalscale.magnitude <= MinZoomScale.magnitude)
        //        {
        //            print(" MinZoomScale limit reached xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        //            transform.localScale = new Vector3(1f, 1f, 1f);

        //        }
        //        if (finalscale.magnitude >= MaxZoomScale.magnitude)
        //        {
        //            print(" MaxZoomScale limit reached xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        //            transform.localScale = new Vector3(2f, 2f, 2f);
        //        }
        //    }
        //}
    }
}
