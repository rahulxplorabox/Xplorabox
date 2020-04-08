using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Camera mycam;
    void Update()
    {
        Vector3 Temp = Input.mousePosition;
        Temp.y = 0f;
        this.transform.position = mycam.ScreenToWorldPoint(Temp);
    }
    
}
