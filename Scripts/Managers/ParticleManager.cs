using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * USED
 * JOSHUA KARMEL
 * 
*/

public class ParticleManager : MonoBehaviour {

    //PUBLIC REFERENCES
    public SideScrollController pCtrl;

    //Particle References
    public GameObject jumpPfx;
    public GameObject landingPfx;
    public Vector3 playerVel;

	// Use this for initialization
	void Start ()
    {
        pCtrl = FindObjectOfType<SideScrollController>();
    }
	
	// Update is called once per frame
	void Update () {
        handleJumpPEs();

        if (!pCtrl.isGrounded) {
            playerVel = pCtrl.localVelocity;
        }

        handleLandingPEs();
    }

    //instantiates the jump particle effects
    public void handleJumpPEs()
    {
        //checks if the player jumps from the ground
        if (Input.GetButtonDown("Jump") && pCtrl.isGrounded){
            Transform clone = Instantiate(jumpPfx.transform, pCtrl.transform.position, Quaternion.identity);
            clone.transform.rotation = Quaternion.Euler(new Vector3(-90, pCtrl.transform.rotation.y, 0));
            jumpPfx.SetActive(true);
            
            //clone gameobject is destroyed after .75s
            Destroy(clone.gameObject,0.75f);
        }
    }

    public void handleCollectiblePEs()
    {

    }
    
    //Need to figure out how to find velocity when landing
    public void handleLandingPEs()
    {
        //if player falling faster than 5
        if(playerVel.z > 0.0f && pCtrl.isGrounded)
        {
            Transform clone = Instantiate(landingPfx.transform, pCtrl.transform.position, Quaternion.identity);
            clone.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
            landingPfx.SetActive(true);

            //destroys after .75s
            Destroy(clone.gameObject, 0.75f);
            playerVel.z = 0;
        }
    }
    
}
