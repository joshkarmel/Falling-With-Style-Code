using UnityEngine;
using System.Collections;

public class Catapult : MonoBehaviour
{
    // handles
    [SerializeField]
    private Transform _bullseye;    // target transform

    [Range(20f, 70f)]
    public float _angle;

    // Use this for initialization
    void Start ()
	{

    }
	
	// Update is called once per frame
	void Update ()
	{
		Launch ();
	}



    private void Launch()
    {
        // source and target positions
        Vector3 pos = transform.position;
        Vector3 target = _bullseye.position;

        // distance between target and source
        float dist = Vector3.Distance(pos, target);

        // rotate the object to face the target
        transform.LookAt(target);

        // calculate initival velocity required to land the cube on target using the formula (9)
        float Vi = Mathf.Sqrt(dist * -Physics.gravity.y / (Mathf.Sin(Mathf.Deg2Rad * _angle * 2)));
        float Vy, Vz;   // y,z components of the initial velocity

        Vy = Vi * Mathf.Sin(Mathf.Deg2Rad * _angle);
        Vz = Vi * Mathf.Cos(Mathf.Deg2Rad * _angle);

        // create the velocity vector in local space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);

        // transform it to global vector
        Vector3 globalVelocity = transform.TransformVector(localVelocity);

        // launch the cube by setting its initial velocity
        GetComponent<Rigidbody>().velocity = globalVelocity;

        // after launch revert the switch
        //_targetReady = false;
    }

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			Destroy (gameObject);
		}
	}
}

