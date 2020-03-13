using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
/*
 * USED
 * BEN SPURR
 * 
 * looks at a target and offsets from that target based on distance, position, and angle
*/

public class CameraController : MonoBehaviour
{
    //PUBLICS
    
    [Header ("Target")]
    public Transform target;

    [Header ("Modifiers")]
    public Vector3 positionOffset = new Vector3(0f, 0f, -1f);   //position of camera relative to player (should be normalized
    public Vector3 lookOffset;                                  //the direction the camera is looking relative to the target
    public float movementDamp = 7f;                             //speed of movement
    public float zoomDamp = .5f;                                //speed of dolly
    public float distance = 2.8f;                               //z distance from the player to the camera
    public float zMax;                                          //max z distance
    public float zMin;                                          //min z distance
    public bool isZoomedOut;                                    //for cinematic effects
    public float zoomOutDist;
    public bool startZoomed;                                    //start camera on player
    public float orthoSize;
    public float initOrthosize;

    //PRIVATES
    private float zTarget;                  //target z position for dynamic dolly
    private Vector2 curZMinMax;             //vector of zMax and zMins
    private SideScrollController pCtrl;     //gets reference to player controller
    private PostProcessingProfile postProfile;
    private float targetOtho;

    public GM gm;

    //initializes values
    void Start()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
        gm = FindObjectOfType<GM>();

        //(good for staging starting shots)
        if (startZoomed)
        {
            transform.position = target.position + positionOffset * (zTarget * distance);
        }
        initOrthosize = Camera.main.orthographicSize;
        postProfile = Camera.main.GetComponent<PostProcessingBehaviour>().profile;
    }

    void HandlePostDOF()
    {
        if (postProfile != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            // Get reference to the DoF settings
            var dof = postProfile.depthOfField.settings;

            // Set variables
            dof.focusDistance = dist;
            //dof.aperture = aperture;

            // Apply settings
            postProfile.depthOfField.settings = dof;
        }
    }

    // Late Uptate called after all for no render artifacts/stuttering
    void LateUpdate()
    {
        //HandlePostDOF();
        //dist multiplier based on player speed
        if (pCtrl.isDead)
        {
            zTarget = Mathf.Lerp(zTarget, zMin * .8f, Time.deltaTime * zoomDamp * 2f);
        }

        else
        {
            float targetMultiplier = Mathf.Clamp((pCtrl.currentVelocity / pCtrl.maxSpeed) * zMax, zMin, zMax);

            zTarget = Mathf.Lerp(zTarget, targetMultiplier, Time.deltaTime * zoomDamp);
        }
        //move and look
        transform.position = Vector3.Lerp(transform.position, target.position + positionOffset, Time.deltaTime * movementDamp);

        targetOtho = Mathf.Lerp(Camera.main.orthographicSize, zTarget, Time.deltaTime * zoomDamp);

        if (targetOtho <= 1) {
            Camera.main.orthographicSize = targetOtho;
        }
        else
        {
            Camera.main.orthographicSize = 20;
        }

        transform.LookAt((target.position + lookOffset));
    }
}
