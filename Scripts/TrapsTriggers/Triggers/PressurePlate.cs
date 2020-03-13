using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//inherits from Toggle
public class PressurePlate : Toggle {

    //as long as player is in collider, is triggered and model is down
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            trigger();
            transform.position = new Vector3(pos.x, pos.y - .2f, pos.z);
        }
    }
}
