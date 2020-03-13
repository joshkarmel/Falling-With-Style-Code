using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAim : MonoBehaviour
{
    private GameObject rightShoulderPoint;
    private SideScrollController pCtrl;
    public Vector2 minMaxAim;
    private Transform rightShoulder;
    public GM gm;

    // Use this for initialization
    void Start()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
        rightShoulder = pCtrl.rightShoulder;

        rightShoulderPoint = new GameObject();
        rightShoulderPoint.name = transform.root.name + "Right Shoulder IK Helper";
    }

    // Update is called once per frame
    void Update()
    {
        if (!gm.frozen && !gm.paused)
        {
            HandleShoulder();
        }
	}


    void HandleShoulder()
    {
        Vector3 rightShoulderPos = rightShoulder.TransformPoint(Vector3.zero);
        rightShoulderPoint.transform.position = rightShoulderPos;
        rightShoulderPoint.transform.parent = transform.parent;

        ///procedural animation for aiming
        transform.LookAt(pCtrl.lookPos);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,0f,0f);
        transform.position = rightShoulderPoint.transform.position;
    }
}
