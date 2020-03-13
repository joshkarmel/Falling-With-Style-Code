using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * USED
 * BEN SPURR
 * 
 * ******************
 * SCRIPT REFERENCES
 * 
 * fInupt.cs
 * ------------------
 * 
 * The heavyweight
 * Controls player movement and actions and animations
 * 
 * ATTACHED TO Player
 * 
*/

public class SideScrollController : MonoBehaviour
{
    //PUBLICS AND DEPENDENCIES
    [Header("Player Objects")]
    public Animator anim;
    public PhysicMaterial playerPhys;
    public GameObject characterGameObj;
    public GameObject gunObj;
    public Transform rightShoulder;
    public GrappleController grapple;
    public CapsuleCollider playerCollider;
    public GameObject trailRenderer;

    //PUBLIC INTERNALS
    [Header("RB Values")]
    public Rigidbody playerRb;
    public Vector3 localVelocity;

    //CLASS REFRENCES
    [Header("Class References")]
    public GM gm;
    public HealthDepletion pHealth;

    //SOUNDS
    [Header("Audio")]
    public AudioClip footstepClip;
    public AudioClip deathClip;
    public AudioClip impactClip;
    public AudioClip jumpClip;

    //Multipliers for movement values
    [Header("Movement Modifiers")]
    public float fallMultiplier = 1.5f;
    public float jumpFallMultiplier = 1f;
    public float groundAccelerationPower;
    public float airAccelerationPower;
    public float swingAccelerationPower;
    public float jumpPower = 12;
    public float groundCheckDistance;   //ray distance for ground check
    public float maxSpeed = 7;
    public float maxAirSpeed = 9;
    public float maxSwingSpeed = 20;
    public float slowSpeed;
    public float stickForce;            //downward grounded force
    public float impactForce;
    public float maxImpact = 40f;        //max survivable impact force

    //General condition values
    [Header("Conditions and Info")]
    public float currentVelocity;
    public float yVelocity;
    public bool trailRenderOn;
    public bool isGrounded;
    public bool isAnchored;             //from GrappleController.cs
    public bool facingLeft;
    public bool isSlowing;              //player is decelerating horizontally
    public bool inContact;
    public bool isDead;
    public bool drawDebug = false;
    public LayerMask groundMask;        //Layers the ground rays can hit

    //Procedural animation values
    [Header("Leaning and Rotating")]
    public float turnSpeed = 15f;
    public float leanSpeed = 1f;
    public float maxLean;
    [HideInInspector]
    public float turnRot;
    public float targetLean;
    [HideInInspector]
    public float leanVal = 0f;
    public bool headCheck;

    //handles player animation for aiming

    public Vector3 lookPos;

    //PRIVATE INTERNALS
    private float look;
    //all pretty self explanatory
    public Vector3 initPlayerPos;
    private RaycastHit groundHitFront;
    private RaycastHit groundHitMid;
    private RaycastHit groundHitBack;
    private Vector2 groundCheckHeights;
    private Vector3 xForceDirection;
    private Vector3 groundNormal;
    private FWSInput inputCtrl;
    //player aiming
    //private GameObject rightShoulderPoint;
    private float lastRotate;

    //USER INPUT PARAMETERS
    public float leanAmt;
    public float horizontal;
    public float vertical;
    private float topForceDir;
    private int ragdollAddVelTimer;
    private bool isJumping;
    private Vector3 onDeathVel;
    private Vector3 xzForceDirection;
    private Rigidbody[] jointRbs;
    private Collider[] jointCols;

    //initializes objects and sets up animator
    void Start ()
    {
        gm = FindObjectOfType<GM>();
        pHealth = FindObjectOfType<HealthDepletion>();
        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        jointRbs = characterGameObj.GetComponentsInChildren<Rigidbody>();
        jointCols = characterGameObj.GetComponentsInChildren<Collider>();
        inputCtrl = FindObjectOfType<FWSInput>();
        initPlayerPos = transform.position;
        isDead = false;
        DisableRagdoll();
        
    }
    
    //for non-phsyics physics and calculations
    private void Update()
    {
        checkVel();
        HandleAnimValues();
        HandleJump();
        lookPos = inputCtrl.lookPos;
        if(inputCtrl.reset)
        {
            gm.ResetScene();
        }
        horizontal = inputCtrl.horizontal;
        vertical = inputCtrl.vertical;
        isJumping = inputCtrl.isJumping;
        groundCheckHeights = new Vector2(groundHitFront.point.y, groundHitBack.point.y); //checks slopes of surface relative to player forward
        checkTrailRender();
    }

    //resetting the player position to initial
    public void resetPosition()
    {
        transform.position = initPlayerPos;
        playerRb.velocity = Vector3.zero;
    }

    //physics functions and calclations
    public void FixedUpdate()
    {
        currentVelocity = playerRb.velocity.magnitude;
        localVelocity.x= transform.InverseTransformDirection(playerRb.velocity).x;
        localVelocity.z= transform.InverseTransformDirection(playerRb.velocity).z;
        HandleGroundCheck();
        HandleMovement();
        HandleRotation();
        HandleFriction();
        HeadCheck();
        //HandleRagdoll();
        //HandleShoulder();

        yVelocity = playerRb.velocity.y;

        if (!isDead)
            onDeathVel = playerRb.velocity;
    }

    //HANDLERS
    //handles movement forces for top-down and sidescrolling
    void HandleMovement()
    {
        //movement forces for while sidescrolling
        if (isGrounded)
        {
            if (currentVelocity < maxSpeed)
            {
                playerRb.AddForce(xForceDirection * groundAccelerationPower, ForceMode.Force);
            }
        }
        else if ((isAnchored && currentVelocity < maxSwingSpeed) || (!isAnchored && currentVelocity < maxAirSpeed))
        { 
            playerRb.AddForce(xForceDirection * airAccelerationPower, ForceMode.Force);
        }

        //slowing forces for horizontal momentum
        if (isGrounded && horizontal == 0f)
        {
            isSlowing = true;
            playerRb.AddForce(new Vector3(-playerRb.velocity.magnitude, 0f, 0f) * slowSpeed, ForceMode.Acceleration);
        }
        else
        {
            isSlowing = false;
        }
    }

    //handles jump forces
    void HandleJump()
    {
        //jump
        if (isGrounded && isJumping)
        {
            SoundManager.PlaySFX(jumpClip, true, 1f);
            
            playerRb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }


        if(playerRb.velocity.y>0f)
        {
            playerRb.velocity += Vector3.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else if (playerRb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            playerRb.velocity += Vector3.up * Physics.gravity.y * jumpFallMultiplier * Time.deltaTime;
        }
    }

    public void SpecialJump(float power, Vector3 dir, bool useNormalJump)
    {
        if(useNormalJump)
        {
            playerRb.AddForce(dir * jumpPower, ForceMode.Impulse);
        }
        else
        {
            playerRb.AddForce(dir * power, ForceMode.Impulse);
        }
    }

    //checks if the player is on the ground and handles surface slopes
    void HandleGroundCheck()
    {
        //finds the center of volume from the origin point
        Vector3 offset = new Vector3(0f, playerCollider.height / 2f, 0f);

        //ground normal check
        if (Physics.Raycast(transform.position+offset, Vector3.down, out groundHitMid, groundCheckDistance * 2f, groundMask))
        {
            groundNormal = groundHitMid.normal;

            HandleFriction();
        }
        else
        {
            groundNormal = Vector3.up;
        }

        //find tangent of ground normal for force direction
        Vector3 tangent = Vector3.Cross(groundNormal, transform.forward);

        //makes movement on slopes easier through directional forces
        if (tangent.magnitude == 0)
        {
            tangent = Vector3.Cross(groundNormal, Vector3.up);
        }

        xForceDirection = new Vector3(horizontal, tangent.y, 0f);
        
        //checks if the player is on the ground
        if (Physics.Raycast(transform.position+offset + (transform.forward * .3f), Vector3.down, out groundHitFront, groundCheckDistance, groundMask) ||
            Physics.Raycast(transform.position+offset + (-transform.forward * .3f), Vector3.down, out groundHitBack, groundCheckDistance, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //draws rays for debugging
        if(drawDebug)
        {
            Debug.DrawRay(transform.position + offset + (transform.forward * .3f), Vector3.down * groundCheckDistance, Color.blue);
            Debug.DrawRay(transform.position + offset + (-transform.forward * .3f), Vector3.down * groundCheckDistance, Color.blue);
            Debug.DrawRay(groundHitMid.point, groundNormal, Color.red);
        }
    }

    //controls the friction for the colliders
    void HandleFriction()
    {
        //high friction when input, no friction when no input
        if (horizontal != 0 || vertical != 0)
        {
            playerPhys.dynamicFriction = .1f;
            playerPhys.staticFriction = .1f;
            playerRb.AddForce(-groundNormal * stickForce, ForceMode.Force);
        }
        else
        {
            playerPhys.dynamicFriction = .8f;
            playerPhys.staticFriction = 1f;
            playerRb.AddForce(-groundNormal * stickForce * 5f, ForceMode.Force);
        }
    }

    //sends values to animator
    void HandleAnimValues()
    {
        //sends values to animator
        anim.SetBool("OnAir", !isGrounded);
        anim.SetBool("isSideScrolling", true);
        anim.SetFloat("MovementX", localVelocity.x/maxSpeed);
        anim.SetFloat("MovementZ", localVelocity.z/maxSpeed);
        anim.SetFloat("AirMovement", playerRb.velocity.y);
    }

    //Plays footstep sounds using AnimationEvents
    public void PlayFootstep()
    {
        //Debug.Log("Play sound?");
        SoundManager.PlaySFX(footstepClip, true, Mathf.Clamp(currentVelocity/maxSpeed,.1f,.2f));
    }

    //handles player rotation
    void HandleRotation()
    {
        float dir = Mathf.Sign(localVelocity.z);

        leanAmt = Mathf.Clamp01(Mathf.Abs(playerRb.velocity.x) / maxSpeed);

        if (isSlowing||isAnchored)
        {
            targetLean= -leanAmt*dir;
        }
        else
        {
            targetLean = leanAmt*dir;
        }

        float curLean=0f;
        curLean = Mathf.Lerp(leanVal, Mathf.Clamp(targetLean*maxLean, 0f, 45f), Time.deltaTime * leanSpeed);

        if (!isAnchored)
        {
            if (lookPos.x < transform.position.x)
            {
                look = -90f;
            }
            else
            {
                look = 90f;
            }
        }

        turnRot = Mathf.Lerp(turnRot, look, Time.deltaTime * turnSpeed);
        //the target rotation for the player rotation
        Quaternion targetRotation;
        targetRotation = Quaternion.Euler(curLean, turnRot, 0f);

        playerRb.transform.rotation = targetRotation;
    }

    //HEY BEN IF YOU COULD DOCUMENT THIS THATD BE GREAT THANKS
    public void DisableRagdoll()
    {
        anim.enabled = true;
        foreach (Rigidbody rb in jointRbs)
        {
            rb.interpolation = RigidbodyInterpolation.None;
            rb.isKinematic = true;
        }
        foreach (Collider col in jointCols)
        {
            col.gameObject.tag = "Player";
            col.enabled = false;
        }
        characterGameObj.transform.parent = transform;
        characterGameObj.transform.localPosition = Vector3.zero;
        characterGameObj.transform.localRotation = Quaternion.identity;
        playerCollider.enabled = true;
        playerRb.isKinematic = false;
        ragdollAddVelTimer = 0;
    }


    public void EnableRagdoll()
    {
        characterGameObj.transform.parent = null;
        playerCollider.enabled = false;
        playerRb.isKinematic = true;
        anim.enabled = false;
        foreach (Rigidbody rb in jointRbs)
        {
            ragdollAddVelTimer++;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.isKinematic = false;

            if(ragdollAddVelTimer<=jointRbs.Length)
            {
                rb.velocity = onDeathVel;
            }
        }
        foreach (Collider col in jointCols)
        {
            col.enabled = true;
        }
    }

    void HeadCheck()
    {
        if (Physics.Raycast(transform.TransformPoint(playerRb.centerOfMass), transform.up - ((Vector3.right).normalized * 0f), (playerCollider.height / 2f) + .8f, groundMask.value) &&
            Physics.Raycast(transform.TransformPoint(playerRb.centerOfMass), transform.up - ((Vector3.right).normalized * .6f), (playerCollider.height / 2f) + .8f, groundMask.value) &&
            Physics.Raycast(transform.TransformPoint(playerRb.centerOfMass), transform.up + ((Vector3.right).normalized * .6f), (playerCollider.height / 2f) + .8f, groundMask.value) &&
            isAnchored)
        {
            headCheck = true;
            Debug.Log("hit head");
        }
        else
        {
            headCheck = false;
        }

        Debug.DrawRay(transform.TransformPoint(playerRb.centerOfMass), ((transform.up - (Vector3.right * 0f)) * ((playerCollider.height / 2f) + .8f)), Color.red);
        Debug.DrawRay(transform.TransformPoint(playerRb.centerOfMass), ((transform.up - (Vector3.right * .6f)) * ((playerCollider.height / 2f) + .8f)), Color.blue);
        Debug.DrawRay(transform.TransformPoint(playerRb.centerOfMass), ((transform.up + (Vector3.right * .6f)) * ((playerCollider.height / 2f) + .8f)), Color.green);
    }

    //checks the velocity of the player to turn the trail renderer on
    public void checkVel()
    {
        if(currentVelocity > 25f || currentVelocity < -25f)
        {
            trailRenderOn = true;
        }
        else
        {
            trailRenderOn = false;
        }
    }
    
    //if trailRenderOn, then the time of the renderer is 0.5.
    //else reduce the time of the renderer until 0
    public void checkTrailRender()
    {
        if (trailRenderOn && isAnchored)
        {
            trailRenderer.GetComponent<TrailRenderer>().time = .5f;
        }
        else
        {
            if (trailRenderer.GetComponent<TrailRenderer>().time >= 0f)
            {
                trailRenderer.GetComponent<TrailRenderer>().time -= .025f;
            }
        }
    }

    //COLLISION CHECKS
    //checks collision entering
    private void OnCollisionEnter(Collision collision)
    {
        
        //checks the impact force
        impactForce = collision.impulse.magnitude;

        if (collision.gameObject.tag == "Hazard" || collision.gameObject.tag == "Deathball")
        {
            isDead = true;
            gm.touchHazard = true;
            SoundManager.PlaySFX(deathClip, true, 1f);

            //enable ragdoll physics
            EnableRagdoll();
        }

        if (impactForce > 12f && isGrounded)
        {
            SoundManager.PlaySFX(footstepClip, true, .4f);
        }


        if (isAnchored && !isGrounded)
        {
            if(!headCheck && impactForce > maxImpact * .6f && grapple.curRopeLength > grapple.maxRopeRange * .5f)
            {
                SoundManager.PlaySFX(impactClip, true, Mathf.Clamp01(impactForce / maxImpact));
                SoundManager.PlaySFX(deathClip, true, 1f);

                pHealth.handleHealth(-(int)impactForce / 3, false);//small dmg
            }
        }
        else
        {
            if (impactForce > maxImpact * .5f)//handle impact dmg/sfx
            {
                SoundManager.PlaySFX(impactClip, true, Mathf.Clamp01(impactForce / maxImpact));
                SoundManager.PlaySFX(deathClip, true, 1f);
                pHealth.handleHealth(-(int)impactForce / 3, false);//small dmg

            }
            else if (impactForce > maxImpact * .85f && !isAnchored)//if close to max impact -2 chunks
            {
                SoundManager.PlaySFX(deathClip, true, 1f);
                pHealth.handleHealth(-2, true);
            }

            if (impactForce > maxImpact)
            {
                SoundManager.PlaySFX(deathClip, true, 1f);
                isDead = true;
                EnableRagdoll();
            }
        }
    }

    //checks if exiting collider
    private void OnCollisionExit(Collision collision)
    {
        inContact = false;
    }
}
