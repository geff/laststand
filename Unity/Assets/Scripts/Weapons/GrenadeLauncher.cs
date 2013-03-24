using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Weapon/GrenadeLauncher")]
public class GrenadeLauncher : BaseCapacity
{
    public Rigidbody grenade;
    public Transform canon;

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();

        Rigidbody projectile = Instantiate(grenade, canon.position, canon.rotation) as Rigidbody;
        projectile.AddForce(canon.forward * 1000 + canon.up * 2 + ParentVehicle.rigidbody.velocity);
    }
}

