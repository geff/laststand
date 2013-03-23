using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Weapon/GrenadeLauncher")]
public class GrenadeLauncher : MonoBehaviour
{
	public ZoneWeaponData data;
    public Rigidbody grenade;

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
        if (Input.GetAxis("Tir special 1") > 0 && Time.time - lastShot >= data.coolDown)
            Shoot();
    }

    void Shoot()
    {
        Rigidbody projectile = Instantiate(grenade, myTransform.position, myTransform.rotation) as Rigidbody;
        projectile.AddForce(myTransform.forward * 1000 + myTransform.up * 2);
        lastShot = Time.time;
    }
}

