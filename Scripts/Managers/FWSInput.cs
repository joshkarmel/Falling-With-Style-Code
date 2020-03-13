using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * USED
 * BEN SPURR
 * 
 * Handles input from Input Manager
 * 
 * ATTACHED TO InputManager
*/

public class FWSInput : MonoBehaviour {

    //EXTERNAL
    public GameObject pauseScreen;
    public Texture2D reticle;
    public CursorMode cursorMode;
    public GM gm;

    [HideInInspector]
    public bool paused;

    [Header ("Input Axis Values")]
    public float horizontal;
    public float vertical;
    public float horizontalAim;
    public float verticalAim;

    [Header ("Conditionals")]
    public bool snap = false;
    public bool reset;
    public bool isJumping;
    public bool isUsingController;
    public bool isShooting;
    public Vector3 lookPos;

    //INTERNAL
    private float lastRotate;       //store rotation for controller aiming
    public SideScrollController pCtrl;
    public GrappleController grappleCtrl;
    private Quaternion aimRotation;

	// Use this for initialization
	void Start ()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
        grappleCtrl=FindObjectOfType<GrappleController>();
        paused = gm.frozen;
        cursorMode = CursorMode.Auto;
        Cursor.SetCursor(reticle, new Vector2(reticle.width/2f,reticle.height/2f),CursorMode.Auto);
	}
	
	// Update is called once per frame
	void Update ()
    {
        paused = gm.frozen;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //ENABLE WITH CONTROLLER SUPPORT
        //verticalAim = Input.GetAxisRaw("Vertical Aim");
        //horizontalAim = Input.GetAxisRaw("Horizontal Aim");

        if (!paused)
        {
            if (!pCtrl.isDead)
            {
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Confined;
            }
            isShooting = Input.GetButtonDown("Fire1");
            isJumping = Input.GetButtonDown("Jump");
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if(Input.GetButtonDown("Reset"))
        {
            //pCtrl.DisableRagdoll();
            reset = true;
        }
        else
        {
            reset = false;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            HandlePause();
        }

        if (isUsingController)
        {
            HandleControllerAim();
        }
        else
        {
            HandleMouseAim();
        }
    }

    //handles the aim from the mouse
    void HandleMouseAim()  
    {
        //Shoots ray forward for hit point so transform can rotate towards
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Vector3 point = (ray.GetPoint(Vector3.Distance(ray.origin, grappleCtrl.barrel.transform.position))- grappleCtrl.barrel.transform.position).normalized*4f;
        Vector3 point = ray.GetPoint(Vector3.Distance(ray.origin, grappleCtrl.barrel.transform.position));

        if (pCtrl.isAnchored && grappleCtrl.curHook != null)
        {
            lookPos = grappleCtrl.curHook.transform.position;
        }
        else
        {
            lookPos = point;
        }
    }

    //NOT FULLY IMPLEMENTED
    //handles the aim from the controller
    void HandleControllerAim()
    {
        //get vector between camera and player 
        Vector3 difference = Camera.main.transform.position - pCtrl.transform.position;

        //why negative difference? idk
        float camRotate = Mathf.Atan2(-difference.x, -difference.z);

        float playerRotate = Mathf.Atan2(horizontalAim, verticalAim);

        //combining the two radians
        playerRotate = playerRotate + camRotate;

        float checkRotation = (Mathf.Abs(Mathf.Atan2(horizontalAim, verticalAim)));

        //store last rotation of player so it doesn't reset when there is no joystick input
        if (checkRotation > 0.2f)
        {
            lastRotate = playerRotate;
        }
        else if (checkRotation < 0.01f && verticalAim > 0)
        {
            lastRotate = 0f;
        }
        else
        {
            playerRotate = lastRotate;
        }

        //convert radian to degrees
        Quaternion eulerRotation = Quaternion.Euler(0f, playerRotate * Mathf.Rad2Deg, 0f);

        //plugin degree conversion into transform
        aimRotation = Quaternion.Slerp(aimRotation, eulerRotation, Time.deltaTime * 10);
    }

    //JOSH KARMEL
    //Handles pausing the game
    void HandlePause()
    {
        gm.handlePause();

        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ps.handleParamScreen();
        }
        */
    }
}
