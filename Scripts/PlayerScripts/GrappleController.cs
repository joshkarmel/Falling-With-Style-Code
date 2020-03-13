using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * shoots the hook projectile and controls the joints
 * 
 * NEEDS REVISION TO WORK WITH PHYSICS SO THAT THE JOINT IS ON THE PLAYER NOT THE BARREL TRANSFORM
*/
//to work with configurable joint, using angular drive for force
public class GrappleController : MonoBehaviour
{
    //DEPENDENCIES
    [Header("Dependencies")]
    public GameObject HookPrefab;
    public GameObject HookStaticModel;
    public GameObject barrel;
    public GameObject gunshotParticlePrefab;
    public GameObject dashParticlePrefab;
    public GM gm;
    private FWSInput pInput;
    public AudioClip shootSound;
    public AudioClip swingSound;
    public AudioClip retractSound;
    public GameObject swingParticles;

    [Header("Modifiers")]
    public float swingPower;
    public float fallPower;//while swinging
    public float effectiveSwingAngle = 5f;//hanging straight down = 0 degrees


    public float ropeJumpPower = 15f;
    public float jumpThreshold = 2f; //distance from anchor before player can jump up from rope;

    public float power;
    public float recoilForce = 0.3f;
    public float massInfluence = 1f;
    [Header("Rope Attributes")]

    public float climbSpeed = .2f;
    public float relativeRange = 20;
    public float maxRopeRange;
    public float minRopeRange = 1f;
    public float curRopeLength;

    public float ropeLengthCorrectionSpeed = .3f; //speed at which rope shortens if beyond max range

    private ConfigurableJoint joint;
    public SoftJointLimit ropeLengthLimit;
    private GameObject curHookMesh;
    bool canPlaySfx;
    public int ropeSmoothness = 7;
    public float ropeDroop = -1f;
    public LayerMask ropeCollisionMask;

    [Header("Conditionals")]
    public bool hookIsSetinRb;
    public bool SwingDebug;
    public bool collisionDebug;
    public bool addNewAnchor;
    public bool unWrapCurrentAnchor;

    //INTERNALS

    private GameObject staticHook;
    public SideScrollController pCtrl;
    private WeponRecoil recoil;
    private LineRenderer line;

    public List<Vector3> anchors = new List<Vector3>();

    [HideInInspector]
    public GameObject curHook;
    public Rigidbody anchoredRb;

    RaycastHit ropeCollision;

    private Rigidbody curAnchorRb;
    private Vector3 lineMid;
    public Vector3 lineEnd;
    private float lineCoef;
    public GameObject curParticle;
    public float tightenSpeed;
    public float targetDist;

    //JK~
    public float shots;

    // Use this for initialization
    void Start()
    {
        gm = FindObjectOfType<GM>();
        recoil = FindObjectOfType<WeponRecoil>();
        line = gameObject.GetComponent<LineRenderer>();
        pInput = FindObjectOfType<FWSInput>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        pCtrl = FindObjectOfType<SideScrollController>();
        staticHook = Instantiate(HookStaticModel, barrel.transform);
        ropeLengthLimit = joint.linearLimit;

        Retract();

    }

    private void Update()
    {
        if ((pInput.isShooting || pInput.reset && curHook != null) && (!pCtrl.isDead || !gm.frozen || !gm.paused))
        {
            Shoot(); //shoot if input, retracts if dead/hook exists
        }

        maxRopeRange = transform.localScale.x * relativeRange;

        HandleLine();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Mathf.Clamp(massInfluence, 0f, 3f);
        lineCoef = ropeDroop / 10f;

        if (pCtrl.isAnchored && (!pCtrl.isDead || !gm.frozen || !gm.paused))
        {
            RopeLength();

            if (!pCtrl.isGrounded)
            {
                HandleRopeCollision();
                HandleSwingingPhysics(pCtrl.horizontal);
            }

            if (pInput.isJumping)//retract if jump and swinging/climbing
            {
                Retract();
                if (curRopeLength - minRopeRange < jumpThreshold && !pCtrl.isGrounded)//jump higher if close enough to anchor
                {
                    pCtrl.SpecialJump(ropeJumpPower, Vector3.up, true);
                }
            }
        }

        
    }

    public void ResetRopeLength()
    {
        ropeLengthLimit.limit = Vector3.Distance(transform.position, curHook.transform.position);
        joint.linearLimit = ropeLengthLimit;
        curRopeLength = ropeLengthLimit.limit;
    }

    void RopeLength()
    {
        //order of statements is essential for proper functionality
        float dist = Vector3.Distance(transform.position, curHook.transform.position);

        //if beyond limit, only move in

        if (dist < maxRopeRange + .1f && dist > minRopeRange)
        {
            ropeLengthLimit.limit = Mathf.Clamp(joint.linearLimit.limit + (-pInput.vertical * climbSpeed), minRopeRange, maxRopeRange);
        }
        else
        {
            if (dist<=minRopeRange)
            {
                ropeLengthLimit.limit = minRopeRange;
            }
            else
            {
                ropeLengthLimit.limit -= Mathf.Abs(pInput.vertical * climbSpeed);
            }
        }

        if (dist < joint.linearLimit.limit && Mathf.Abs(pInput.vertical)==0 && !hookIsSetinRb)
        {
            ropeLengthLimit.limit = dist;
        }

        curRopeLength = joint.linearLimit.limit;
        joint.linearLimit = ropeLengthLimit;
    }

    void HandleSwingingPhysics(float horizontal)
    {
        //find angle perpendicular to raduis of swing by swapping x and y of direction vector
        Vector3 dir = curHook.transform.position - pCtrl.transform.position;

        Vector3 perpendicularDir = new Vector3(-dir.y, dir.x, 0f).normalized;
        perpendicularDir = -perpendicularDir;

        float curSwingAngle = Mathf.Abs(Vector2.SignedAngle(dir, Vector3.up));
        float normalizedPower = (effectiveSwingAngle - curSwingAngle) / effectiveSwingAngle;

        
        if (SwingDebug)
        {
            Debug.DrawRay(pCtrl.transform.position, perpendicularDir, Color.red);
            //Debug.Log("Angle: " + Mathf.Abs(curSwingAngle));
            Debug.Log("Power: " + normalizedPower);

            Vector3 rotatedVector1 = Quaternion.AngleAxis(effectiveSwingAngle, Vector3.forward) * -Vector3.up;
            Vector3 rotatedVector2 = Quaternion.AngleAxis(-effectiveSwingAngle, Vector3.forward) * -Vector3.up;
            Debug.DrawRay(pCtrl.transform.position, dir, Color.red);
            Debug.DrawRay(curHook.transform.position, rotatedVector1.normalized * dir.magnitude, Color.yellow);
            Debug.DrawRay(curHook.transform.position, rotatedVector2.normalized * dir.magnitude, Color.yellow);
        }

        
        if (curSwingAngle <= effectiveSwingAngle)//fakes parametric resonance (aplifies motion by changing length of raduis).  On a real life swing you pull yourself up, here just adding force
        {
            float targetPower;

            if(pCtrl.currentVelocity<20f && pCtrl.currentVelocity < 10f)
            {
                targetPower = swingPower / 2f;

                if (canPlaySfx && pCtrl.currentVelocity > 4f)
                {
                    SoundManager.PlaySFX(swingSound, true, .1f);
                    canPlaySfx = false;
                }

            }
            else
            {
                targetPower = swingPower;
                if (canPlaySfx)
                {
                    SoundManager.PlaySFX(swingSound, true, .3f);
                    canPlaySfx = false;
                }

            }
            pCtrl.playerRb.AddForce((perpendicularDir * (horizontal * targetPower)) * normalizedPower, ForceMode.Impulse);
            //canPlaySfx = false;
        }
        else //exagerates gravity when outside of angle threshold
        {
            canPlaySfx = true;

            if (pCtrl.yVelocity <= 0f && curSwingAngle > 45f)
            {
                pCtrl.playerRb.AddForce(-Vector3.up * (fallPower), ForceMode.Force);
            }
            else
            {
                pCtrl.playerRb.AddForce(perpendicularDir * Mathf.Sign(curHook.transform.position.x - pCtrl.transform.position.x) * (fallPower * .4f), ForceMode.Force);
            }
        }
    }

    void HandleRopeCollision()
    {
        Vector3 curDir = anchors[anchors.Count-1] - transform.position;//vect between player and current hook

        Ray ropeRay = new Ray(transform.position + curDir * .1f, curDir);//ray of vector

        //add new anchor
        if (Physics.Raycast(ropeRay, out ropeCollision, curDir.magnitude*.8f,ropeCollisionMask)&&anchoredRb!=null) 
        {
            addNewAnchor = true;
            anchors.Add(ropeCollision.point);
            curHook.transform.position = ropeCollision.point;
            ResetRopeLength();
        }
        else
        {
            addNewAnchor = false;
        }

        //unwrap
        if (anchors.Count > 1)
        {
            Vector3 currentDir = anchors[anchors.Count - 1] - pCtrl.transform.position;
            Vector3 previousDir = anchors[anchors.Count - 1] - anchors[anchors.Count - 2];  //vect between player and last hook

            //all dot product does is get the amount that a vector is pointing the same direction as another vector
            //checking if the angle vector is pointing the same way as the surface normal
            float curRopeAngle =Mathf.Atan2(Vector3.Dot(ropeCollision.normal.normalized, Vector3.Cross(previousDir, curDir)),Vector3.Dot(previousDir, curDir)) * Mathf.Rad2Deg;

            if (curRopeAngle<0||curRopeLength<=minRopeRange)
            {
                curHook.transform.position = anchors[anchors.Count - 2];
                anchors.Remove(anchors[anchors.Count - 1]);
                ResetRopeLength();
            }
            if (collisionDebug & anchors[anchors.Count - 2] != null) 
            {
                Debug.DrawRay(anchors[anchors.Count - 2], previousDir, Color.yellow);
            }
            
        }
            if (collisionDebug)
        {
            Debug.DrawRay(pCtrl.transform.position, curDir, Color.green);
        }


        
    }

    void HandleLine()
    {
        //faceted line
        if (curHook == null || curHook && !pCtrl.isAnchored)
        {

            if (curHook == null)
            {
                lineEnd = barrel.transform.position + (barrel.transform.forward * .2f);
            }
            else
            {
                lineEnd = curHook.transform.position;
            }

            line.positionCount = ropeSmoothness;
            //sets position and count of nodes;
            for (int i = 0; i < line.positionCount; i++)
            {
                if (i == 0)//start position
                {
                    line.SetPosition(i, barrel.transform.position);
                }
                else if (i == line.positionCount - 1)//end position
                {
                    line.SetPosition(i, lineEnd);
                }
                else//intermediate nodes
                {
                    Vector3 iStart = line.GetPosition(i - 1);
                    Vector3 iEnd = line.GetPosition(i + 1);
                    float distRatio = Mathf.Clamp01(maxRopeRange - Vector3.Distance(barrel.transform.position, lineEnd));//normalizes based on dist
                    Vector3 iMid = Vector3.Lerp(iStart, iEnd, 0.5f) + new Vector3(0f, (lineCoef * distRatio) * Mathf.Sqrt(Vector3.Distance(barrel.transform.position, Vector3.Lerp(iStart, iEnd, 0.5f))), 0f);//parabolas! highschool did pay off!
                    line.SetPosition(i, iMid);
                }
            }
        }
        else if (hookIsSetinRb)
        {
            line.positionCount = 2;
            line.SetPosition(0, barrel.transform.position);
            line.SetPosition(1, curAnchorRb.transform.TransformPoint(joint.connectedAnchor));
        }
        else
        {
            line.positionCount = anchors.Count + 1;

            line.SetPosition(anchors.Count, barrel.transform.position);

            for (int i = 0; i < anchors.Count; i++)
            {
                line.SetPosition(i, anchors[i]);
            }
        }
    }
    //handles line renderer
    //handles reloading and shooting
    public void Shoot()
    {
        //checks if current hook exists, destroys if so and instantiates new one
        //"Click to shoot, click to remove, click to shoot again"
        ropeLengthLimit.limit = maxRopeRange;
        joint.linearLimit = ropeLengthLimit;
        curRopeLength = ropeLengthLimit.limit;

        if (curHook == null)//shoot
        {
            staticHook.SetActive(false);
            recoil.isShooting = true;

            //plays shoot sound ~~JK&HA
            SoundManager.PlaySFX(shootSound, true, 1f);

            

            curHook = Instantiate(HookPrefab, barrel.transform.position, barrel.transform.rotation);
            anchoredRb = curHook.GetComponent<Rigidbody>();

            Vector3 dir = (pInput.lookPos-curHook.transform.position).normalized;

            pCtrl.playerRb.AddForce(-dir * recoilForce, ForceMode.Impulse);//knock back player
            anchoredRb.AddForce(dir * power, ForceMode.Impulse);//shoot projectile


            curParticle = Instantiate(gunshotParticlePrefab, barrel.transform.position, barrel.transform.rotation);
            curParticle.transform.parent = barrel.transform;

            Destroy(curParticle, 5f);

            joint.connectedBody = anchoredRb;
            joint.connectedAnchor = Vector3.zero;
            joint.massScale = 0f;
            joint.enableCollision = false;

            if (gm.mode == GM.Modes.CLASSIC || gm.mode == GM.Modes.ENDLESS || gm.mode == GM.Modes.SOULLESS)
            {
                shots++;
            }
            else if (gm.mode == GM.Modes.LIMSWINGS && shots > -1)
            {
                shots--;
            }
        }
        else//if hook exists destroy
        {
            SoundManager.PlaySFX(retractSound, true, .1f);
            Retract();
            
        }
        
        
    }

    public void Retract()
    {
        
        anchors.Clear();
        Destroy(curParticle);
        staticHook.SetActive(true);
        pCtrl.isAnchored = false;
        curAnchorRb = null;
        joint.connectedBody = null;
        joint.massScale = 0f;
        Destroy(curHook);
        curHook = null;
    }

    //if hook collides make the joint anchor pos same as hook rigidbody pos
    //takes in the hit rigidbody as the anchor point
    //called from HookController.cs
    public void SetHook(Rigidbody anchor)
    {
        anchors.Add(curHook.transform.position);
        pCtrl.isAnchored = true;
        joint.massScale = massInfluence;
        ResetRopeLength();

        //checks to see whether the anchor is dynamic or static for pulling/swinging
        if (anchor == null)//look for rigidbody on object
        {
            hookIsSetinRb = false;
            anchoredRb.isKinematic = true;
            
        }
        else
        {
            //basic dragging mechanics
            hookIsSetinRb = true;
            curAnchorRb = anchor;
            joint.connectedBody = anchor;
            
            Vector3 target = anchor.transform.InverseTransformPoint(curHook.transform.position);

            joint.connectedAnchor = target;
            joint.enableCollision = true;
            curHook.SetActive(false);
        }
    }

    public void SetHookOnRopeCollision(Vector3 collisionPos)
    {
        curHook.transform.position = collisionPos;
        anchors.Add(curHook.transform.position);
        pCtrl.isAnchored = true;
        joint.massScale = massInfluence;
        ResetRopeLength();

        hookIsSetinRb = false;
        anchoredRb.isKinematic = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pCtrl.isAnchored)
        {
            Gizmos.DrawSphere(Vector3.Lerp(curHook.transform.position, transform.TransformPoint(pCtrl.playerRb.centerOfMass), jumpThreshold / curRopeLength), .4f);
        }
    }
}