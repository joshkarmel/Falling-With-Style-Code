using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingLava : MonoBehaviour {
	public GM gm;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gm.phase == GM.Phases.ESCAPE) {
			transform.position += new Vector3(0,.1f,0);
		}
	}
}
