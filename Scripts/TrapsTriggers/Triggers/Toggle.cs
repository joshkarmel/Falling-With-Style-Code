using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : Trigger {

    //initial position
    protected Vector3 pos;
    
    //model of the toggle
    public GameObject buttonModel;

    // Use this for initialization
    void Start()
    {
        pos = buttonModel.transform.position;
    }

    //at collision w/ player, triggers and model moves down
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            trigger();
            buttonModel.transform.position = new Vector3(pos.x, pos.y - .2f, pos.z);
        }
    }

    //player leaves collider, button goes back up
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            buttonModel.transform.position = pos;
        }
    }
}
