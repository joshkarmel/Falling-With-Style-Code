using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : Trap {

    Rigidbody doorRb;

	// Use this for initialization
	void Start () {
        active = false;
        doorRb = gameObject.GetComponent<Rigidbody>();
	}

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "Player")
        {
            active = true;
        }
    }

    public override void checkActive()
    {
        if (active)
        {
            doorRb.isKinematic = false;
        }
    }
}
