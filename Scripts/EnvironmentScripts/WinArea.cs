using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * JOSH KARMEL
 * USED
 * 
 * ATTACHED TO THE WIN AREA OBJECT
*/

public class WinArea : MonoBehaviour {

    //PUBLIC REFERENCED SCRIPTS
    public GM gm;
    public AudioClip winSound;

    public bool win;

	// Use this for initialization
	void Start () {
        gm = FindObjectOfType<GM>();
        win = false;
	}
	
	// Update is called once per frame
	void Update () {
        //transitions from exploration to escape at relic area
        if (tag == "relicArea")
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 60, Space.World);

            if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab) && gm.phase == GM.Phases.EXPLORE)
            {
                SoundManager.PlaySFX(winSound, false, .6f);
                gm.triggerEscape();
                gameObject.SetActive(false);
            }
        }
    }

    //transitions between phases
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //transitions from exploration to escape at relic area
            if (tag == "relicArea" && gm.phase == GM.Phases.EXPLORE)
            {
                SoundManager.PlaySFX(winSound, false, .6f);
                gm.triggerEscape();
                gameObject.SetActive(false);
            }

            //win condition for escape area
            if (tag == "exitArea" && gm.phase == GM.Phases.ESCAPE)
            {
                SoundManager.PlaySFX(winSound, false, .6f);
                win = true;
                gm.frozen = true;
            }
        }
    }
}
