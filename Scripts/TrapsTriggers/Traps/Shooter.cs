using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//inherits from Trap class
public class Shooter : Trap {

    public enum ShooterType {STANDARD, TIMED};

    //PUBLIC REFERENCES
    public GameObject dart;
    public AudioClip shootSound;

    //PUBLIC ATTRIBUTES
    public bool canShoot;
    public float power;             //meters
    public bool LimitLifetime;
    public float dartLifetime;      //seconds
    public float timer;
    public float framesBetweenShots;
    public float fireCount;
    public ShooterType shooterType;
    public GameObject barrelTrans;

	// Use this for initialization
	void Start () {
        active = false;
        canShoot = true;
        fireCount = 0;
        framesBetweenShots = 30;
        if (LimitLifetime)
        {
            dartLifetime = 2f;
        }
	}

    private void FixedUpdate()
    {
        if(shooterType == ShooterType.TIMED)
        {
            active = true;
            framesBetweenShots = timer;
        }

        //delays the rapidfire shots
        if(canShoot == false && fireCount <= framesBetweenShots)
        {
            fireCount++;
        }
        if(fireCount > framesBetweenShots)
        {
            canShoot = true;

        }

    }

    //instantiates dart prefab to shoot
    public void shoot()
    {
        var bullet = (GameObject)Instantiate(dart, barrelTrans.transform.position, barrelTrans.transform.rotation);
        SoundManager.PlaySFX(shootSound, true, .03f);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * power;

        if (LimitLifetime)
        {
            Destroy(bullet, dartLifetime);
        }
        else
        {
            Destroy(bullet, 10f);
        }
        active = false;
    }

    //if active and can shoot, shoot, then reset rof
    public override void checkActive()
    {
        if (active && canShoot)
        {
            shoot();
            canShoot = false;
            fireCount = 0;
        }
    }
}
