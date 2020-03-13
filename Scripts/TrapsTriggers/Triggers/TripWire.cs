using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//inherits from Toggle
public class TripWire : Toggle {

    //parent gameObject
    public GameObject parent;

    //triggers trap and destroys the gameObject after being used once
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            trigger();
            Destroy(parent);
        }
    }
}
