using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeponRecoil : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
    public SideScrollController pCtrl;
    public GM gm;

    //PUBLIC ATTRIBUTES
    public bool isShooting;
    public float recoilSpeed=10f;
    public float recoilLength=.3f;
    public Vector3 rotationalRecoil;
    public bool moveToInitPosition;
    public bool isShouldered;
    public float dist;

    public Vector3 targetPos;
    public Quaternion targetRotation;
    public Transform ShoulderPos;//parent
    
    public Transform aimingTrans;

    public float animBobbingSpeed = 20.0f;  // Speed of sine movement
    public float animBobbingMagnitude = 0.5f;   // Size of sine movement

    // Use this for initialization
    void Start ()
    {
        moveToInitPosition = true;
        ShoulderPos = transform.parent;
        pCtrl = FindObjectOfType<SideScrollController>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        HandleRecoil();
        HandleGunSway();

        if (pCtrl.yVelocity > 0f && !pCtrl.isGrounded && !pCtrl.isAnchored)
        {
            isShouldered = true;
        }
        else
        {
            isShouldered = false;
        }
    }

    void HandleRecoil()
    {
        dist = Vector3.Distance(transform.position, ShoulderPos.position);

        if (isShooting)
        {
            if (dist >= recoilLength - 0.01f)
            {
                moveToInitPosition = true;
                isShooting = false;
            }
            else
            {
                moveToInitPosition = false;
            }
        }

        float speed = recoilSpeed;

        if (moveToInitPosition && !isShouldered) 
        {
            targetRotation = Quaternion.Euler(Vector3.zero);
            targetPos = ShoulderPos.position;
        }
        else if(isShouldered)
        {
            targetRotation = Quaternion.Euler(Vector3.zero);
            targetPos = aimingTrans.position;
            speed = speed * .3f;
        }
        else
        {
            targetRotation = Quaternion.Euler(rotationalRecoil);
            targetPos = ShoulderPos.position - transform.forward * recoilLength;
            speed = speed * 2f;
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * speed);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
    }

    void HandleGunSway()
    {
        if (!pCtrl.isDead && !gm.frozen && !gm.paused)
        {
            if (pCtrl.horizontal != 0f && pCtrl.isGrounded)
            {
                float ratio = pCtrl.currentVelocity / pCtrl.maxSpeed;
                transform.position = transform.position + Vector3.up * Mathf.Sin(Time.time * animBobbingSpeed) * animBobbingMagnitude * ratio;
            }
            else if (pCtrl.horizontal == 0f && pCtrl.isGrounded)
            {
                transform.position = transform.position + Vector3.up * Mathf.Sin(Time.time * animBobbingSpeed * .2f) * animBobbingMagnitude * .3f;
            }
        }
    }
}
