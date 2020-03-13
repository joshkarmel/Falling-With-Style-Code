using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaRising : MonoBehaviour {

    public GM gm;
    private Vector3 lavaPos;

	// Use this for initialization
	void Start () {
        lavaPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        lavaPos = transform.position;
        if (gm.phase == GM.Phases.ESCAPE)
        {
            transform.position = new Vector3(lavaPos.x, lavaPos.y + .05f, lavaPos.z);
        }
	}
}
