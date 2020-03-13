using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : Trap {


    public GameObject visual;

    // Use this for initialization
    void Start () {
        active = false;
	}

    public override void checkActive()
    {
        if (active != false)
        {
            visual.SetActive(true);
        }
        else
        {
            visual.SetActive(false);
        }
    }
}
