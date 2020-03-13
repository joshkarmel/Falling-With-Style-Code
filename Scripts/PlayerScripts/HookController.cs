using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * BEN SPURR
 * 
 * sends information to GrappleController.cs
*/

public class HookController : MonoBehaviour {

    //privates
    private Rigidbody hookRb;
    private GrappleController grapple;
    private SideScrollController player;
    public AudioClip hitSound1;
    public AudioClip hitSound2;
    public AudioClip hitSound3;
    public AudioClip ropeSound1;
    public AudioClip ropeSound2;
    public GameObject HookHitParticles;
    AudioClip[] hitSounds;
    AudioClip[] ropeSounds;
    RaycastHit ropeHit;
    public LayerMask ropeCollisionMask;
    bool isAnchored;
    float impulse;
	// Use this for initialization
	void Start ()
    {
        hitSounds = new AudioClip[] { hitSound1, hitSound2, hitSound3 };
        ropeSounds = new AudioClip[] {ropeSound1};
        isAnchored = false;
        hookRb = gameObject.GetComponent<Rigidbody>();
        grapple = FindObjectOfType<GrappleController>();
        player = FindObjectOfType<SideScrollController>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {

        float dist = Vector3.Distance(grapple.barrel.transform.position, transform.position);
        

		if(Physics.Raycast(grapple.barrel.transform.position, gameObject.transform.position - grapple.barrel.transform.position, out ropeHit, dist, ropeCollisionMask) && !isAnchored)//shouldnt collide with dynamic objects
        {
            Debug.Log("yes");
            isAnchored = true;
            grapple.SetHookOnRopeCollision(ropeHit.point);
            StartCoroutine(PlayImpact(ropeSounds,.11f));
        }

        if(!isAnchored)
        {
            Debug.DrawRay(grapple.barrel.transform.position, (gameObject.transform.position - grapple.barrel.transform.position).normalized*dist, Color.red);
        }
	}

    private void OnCollisionEnter(Collision col)
    {
        isAnchored = true;
        StartCoroutine(PlayImpact(hitSounds, Mathf.Clamp(impulse / 20f, .05f, .1f)));
        impulse = col.impulse.magnitude;
        GameObject curImpactParticle = Instantiate(HookHitParticles,col.contacts[0].point, Quaternion.LookRotation(col.contacts[0].normal,Vector3.up)) as GameObject;
        Destroy(curImpactParticle, 3f);
        //hook can't hook the player
        if (col.gameObject.tag != "Player")
        {
            //calls SetHook() from GrappleController.cs
            grapple.SetHook(col.gameObject.GetComponent<Rigidbody>());
        }
        else
        {
            Debug.Log("hookRb instantiated inside player");
            Physics.IgnoreCollision(col.gameObject.GetComponent<CapsuleCollider>(), GetComponent<SphereCollider>());
        }
    }

    //so plays slightly after gunshot, doesnt get muffled, hook moves fast af
    public IEnumerator PlayImpact(AudioClip[] sound, float volume)
    {
        yield return new WaitForSecondsRealtime(.1f);
        SoundManager.PlaySFX(sound[Random.Range(0,sound.Length-1)], true, volume);
    }
}
