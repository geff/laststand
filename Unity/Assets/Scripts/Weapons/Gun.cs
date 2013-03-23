using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Weapon/Gun")]
public class Gun : MonoBehaviour
{
	public FireWeaponData data;
    public Rigidbody bullet;

    private Transform myTransform;
    private float lastShot;

    void Awake()
    {
        myTransform = transform;
        lastShot = -10f;
    }

	// Use this for initialization
	void Start()
	{
 
	}
	
	// Update is called once per frame
	void Update()
	{
        if (Input.GetAxis("Tir principal") > 0 && Time.time - lastShot >= data.coolDown)
            Shoot();
	}

    void Shoot()
    {
        Rigidbody projectile = Instantiate(bullet, myTransform.position, myTransform.rotation) as Rigidbody;
        projectile.AddForce(myTransform.forward * 1000);
        lastShot = Time.time;
    }
}

