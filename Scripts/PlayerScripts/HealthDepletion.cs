using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDepletion : MonoBehaviour {

    //PUBLIC ATTRIBUTES
    //player has set number of touches he can make to the ground (instead of constant damage)
    //player can collect to fill up ground touches up to an alloted amount
	public int healthVal;       //current player health
    public int healthChunks;    //divisions of health value (ie. hearts)
    public int baseDmg;         //number of health chunks to remove upon ground touch
	public int baseHeal;        //number of units to fill per pickup/action
    public Slider healthBar;
    public AudioClip deathClip;

    //PRIVATE ATTRIBUTES
    private int maxHealth;
    private SideScrollController pCtrl;
    private FWSInput inputCtrl;
    private bool playerHasLanded;

	// Use this for initialization
	void Start ()
    {
        healthVal = 100;
        healthChunks = 10;
        baseDmg = 1;
        baseHeal = 10;
        maxHealth = healthVal;
        pCtrl = FindObjectOfType<SideScrollController>();
        inputCtrl = FindObjectOfType<FWSInput>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        healthVal = Mathf.Clamp(healthVal, 0, maxHealth);
		healthBar.value = healthVal;

        //keeps 
        if(healthVal > maxHealth)
        {
            healthVal = maxHealth;
        }
	}

	//algorithm for health depletion
	private void Touching()
	{

	}

    //JK~~
    //handles all health, removing or adding
    //healthMod: + for healing, - for damage. If useChunks, healthmod = # of chunks
    public void handleHealth(int healthMod, bool useChunks)
    {
        if (useChunks)
        {
            if ((healthMod > 0 && healthVal <= maxHealth) || healthMod < 0)
            {
                healthVal += (maxHealth / healthChunks) * healthMod;
            }
        }
        else
        {
            healthVal += healthMod;
        }
    }
		
    //JK~~
    //resets values
    public void resetHealth()
    {
        healthVal = maxHealth;
    }

	void OnCollisionEnter(Collision col)
	{
        /*
         * DEPLETES HEALTH FROM ANYTHING THATS NOT SAFE OR INTERACTABLE
		// needs to be changed to check for floor
        if(pCtrl.isGrounded && col.gameObject.tag != "safe" && col.gameObject.tag != "Interactable")
        {
            handleHealth(-1, true);
            SoundManager.PlaySFX(deathClip, true, 1f);
        }
        */

        //depletes health from dart
        if (col.gameObject.tag == "Dart")
        {
            SoundManager.PlaySFX(deathClip, true, 1f);
            handleHealth(-2, true);
        }

	}

	void OnCollisionExit(Collision col)
	{
		playerHasLanded = false;
	}
}
