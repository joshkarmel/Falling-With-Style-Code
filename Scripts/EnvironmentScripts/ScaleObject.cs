using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObject : MonoBehaviour {
	private float x;
	private float y;
	private float z;

	// Use this for initialization
	void Start () {
		x = transform.position.x;
		y = transform.position.y;
		z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		x = transform.position.x;
		y = transform.position.y;
		z = transform.position.z;

       // TextureScale.Bilinear();
	}
}
