using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * JOSH KARMEL
 * USED
 * 
 * ATTACHED TO COLLECTABLES
*/

public class Collect : MonoBehaviour {

    //PUBLIC SCRIPT REFERENCES
    public GM gm;
    public HealthDepletion hd;
    public SideScrollController pCtrl;

    //PUBLIC ATTRIBUTES
    public GameObject collectPfx;
    public AudioClip gaspClip;

    private void Start()
    {
        gm = FindObjectOfType<GM>();
        hd = FindObjectOfType<HealthDepletion>();
        pCtrl = FindObjectOfType<SideScrollController>();
        
    }

    private void Update()
    {
        rotateBox();
    }

    //rotates the mesh, not the collider
    public void rotateBox()
    {
        if (tag == "Collectable")
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 60, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //handles collectible count, then special collectible attribute, then the object deactivates
        if (other.gameObject.tag == "Player" && tag == "CollectableCollider")
        {
            if(transform.parent.tag == "normieCollectible")
            {
                //normie special TBD
				hd.handleHealth(5, false);
                gm.handleColCount();
            }
            else if(transform.parent.tag == "healCollectible")
            {
                hd.handleHealth(3, true);
                gm.handleColCount();
            }
            else if(transform.parent.tag == "scoreCollectible")
            {
                gm.handleGoldColCount();
            }

            SoundManager.PlaySFX(gaspClip, true, .6f);

            Transform clone = Instantiate(collectPfx.transform, transform.position, Quaternion.identity);
            clone.transform.rotation = Quaternion.Euler(new Vector3(-90, transform.rotation.y, 0));
            collectPfx.SetActive(true);

            //clone gameobject is destroyed after .75s
            Destroy(clone.gameObject, 0.75f);

            transform.parent.gameObject.SetActive(false);
        }
    }

    
}
