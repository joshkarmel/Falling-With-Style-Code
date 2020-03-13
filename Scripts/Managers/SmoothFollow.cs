using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * used for moving game obejcts between two points and for camera focal points
*/

public class SmoothFollow : MonoBehaviour
{
    [Header("Dependencies")]
    [Space(1)]
    public SideScrollController pCtrl;
    [Space(5)]

    [Header("Transforms")]
    [Space(1)]
    public Transform target1;
    public Transform target2;
    [Space(5)]

    [Header("Attributes")]
    [Space(1)]
    public float smoothFactor = 2;
    public float smoothSnapThresh = 0.1f;
    public float vectDist;
    [Space(5)]

    [Header("Conditions")]
    [Space(1)]
    public bool target1Snap;
    public bool target2Snap;
    public bool onTarget1 = true;

    private void Start()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
        if(target1==null)
        {
            target1 = FindObjectOfType<GunSight>().transform;
        }
    }

    //INCOMPLETE
    void Update()
    {
        target2Snap = false;

        //distance between the two points
        vectDist = Vector3.Distance(transform.position, target1.position);

        //checks if the distance is within the threshold and lerps towards target
        if (onTarget1)
        {
            if (vectDist > smoothSnapThresh && !target1Snap)
            {
                transform.position = Vector3.Lerp(transform.position, target1.position, Time.deltaTime * smoothFactor);
            }
            else
            {
                //stops lerping if it reaches the threshold
                target1Snap = true;
                transform.position = target1.position;
            }
        }
        else
        {
            if (vectDist > smoothSnapThresh && !target2Snap)
            {
                transform.position = Vector3.Lerp(transform.position, target2.position, Time.deltaTime * smoothFactor);
            }
            else
            {
                //stops lerping if it reaches the threshold
                target2Snap = true;
                transform.position = target2.position;
            }
        }
    }
}