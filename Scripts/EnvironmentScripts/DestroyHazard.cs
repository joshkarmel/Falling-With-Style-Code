using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHazard : MonoBehaviour {


	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag ("Hazard")) {
		//	Destroy (col.gameObject);
		}
		if (col.gameObject.CompareTag ("Deathball")) {
			Destroy (col.gameObject);
		}
	}
}
