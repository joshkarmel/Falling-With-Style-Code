using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * ANIMATION STYFF
*/

public class IKHandler : MonoBehaviour {

    
    private Animator anim;
    private Vector3 lookPos;
    private Vector3 IK_lookPos;
    private Vector3 targetPos;
    private SideScrollController pCtrl;

    public float lerpRate = 15;
    public float updatelookPosThreshold = 2;
    public float lookWeight = 1;
    public float bodyWeight = .9f;
    public float headWeight = 1;
    public float clampWeight = 1;

    public float rightHandWeight = 1;
    public float leftHandWeight = 1;

    public Transform rightHandTarget;
    public Transform rightElbowTarget;
    public Transform leftHandTarget;
    public Transform leftElbowTarget;

    void Start()
    {
        anim = GetComponent<Animator>();
        pCtrl = GetComponent<SideScrollController>();
    }

    //NEEDS REVISION
    void OnAnimatorIK()
    {

        //sets IK weights for rotations and position of hands and elbows
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandWeight);

        anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);

        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandWeight);

        anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);

        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, rightHandWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, leftHandWeight);

        anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTarget.position);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);

        //sets look position for IK
        lookPos = pCtrl.lookPos;
        lookPos.z = transform.position.z;

        //distance of the look pos from the player
        float distanceFromPlayer = Vector3.Distance(lookPos, transform.position);

   
        //sets minimum aim distance
        if(distanceFromPlayer>updatelookPosThreshold)
        {
            targetPos = lookPos;
        }

        //sets IK look position from the target position
        IK_lookPos = Vector3.Lerp(IK_lookPos, targetPos, Time.deltaTime * lerpRate);

        //applies IK to head and body
        anim.SetLookAtWeight(lookWeight, bodyWeight, headWeight, headWeight, clampWeight);
        anim.SetLookAtPosition(IK_lookPos);

    }


}
