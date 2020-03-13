using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawn : MonoBehaviour {
    public Transform obj;
    public float spawnRate;
    public int max;
    public float force;

	// Use this for initialization
	void Start () {
        StartCoroutine(Stuff());
	}
	
    IEnumerator Stuff()
    {
        int i = 0;
        Transform obj2;
        while(i != max)
        {
            obj2 = Instantiate(obj, transform.position, transform.rotation);
            obj2.GetComponent<Rigidbody>().AddForce(obj2.transform.up * force);

            yield return new WaitForSeconds(spawnRate);
            i++;
        }
    }
}
