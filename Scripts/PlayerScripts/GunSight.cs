using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSight : MonoBehaviour
{

    LineRenderer line;
    GrappleController grapple;
    float range;
    FWSInput inputCtrl;

    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRenderer>();
        grapple = FindObjectOfType<GrappleController>();
        inputCtrl = FindObjectOfType<FWSInput>();

    }

    // Update is called once per frame
    void Update()
    {
        range = Vector3.Distance(inputCtrl.lookPos, grapple.barrel.transform.position);
        range = Mathf.Clamp(range, 1f, grapple.maxRopeRange);
        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, inputCtrl.lookPos);

        if (grapple.curHook == null)
        {
            line.enabled = true;
        }
        else
        {
            line.enabled = false;
        }
    }
}
