using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class swipetrial : MonoBehaviour
{
    public Camera mycam;
    public GameObject trailPrefab;
    public GameObject thisTrail;
    Vector3 startPos;
    Plane objPlane;
    public GameObject LineDrawerContainer;
    public bool linedrawenable;
    public bool startLineDrawing;
    public int count;

    public static swipetrial swipetrial_instance;

    private void Awake()
    {
        if(swipetrial_instance == null)
        {
            swipetrial_instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        objPlane = new Plane(mycam.transform.forward * -1, this.transform.position);
    }
    void Update()
    {
        if (startLineDrawing && linedrawenable)
        {
            if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
            {

                thisTrail = Instantiate(trailPrefab, this.transform.position, Quaternion.identity);
                thisTrail.transform.SetParent(LineDrawerContainer.transform, false);
                thisTrail.name = "line_" + count;
                count++;

                Ray mRay = mycam.ScreenPointToRay(Input.mousePosition);
                print("Mouse Down" + Input.mousePosition);

                float rayDistance;
                if (objPlane.Raycast(mRay, out rayDistance))
                {
                    startPos = mRay.GetPoint(rayDistance);
                }
            }
            else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0))
            {
                Ray mRay = mycam.ScreenPointToRay(Input.mousePosition);

                float rayDistance;
                if (objPlane.Raycast(mRay, out rayDistance))
                {
                    thisTrail.transform.position = mRay.GetPoint(rayDistance);
                }
                else
                {
                    Destroy(thisTrail);
                }
            }
            else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp(0))
            {
                print("Mouse up" + Input.mousePosition);
                if (Vector3.Distance(thisTrail.transform.position, startPos) < 0.1)
                {
                    Destroy(thisTrail);
                }
            }
        }
    }
}
