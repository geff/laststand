using UnityEngine;
using System.Collections;

public class StdCanon : BaseCapacity {

    public Transform canon;
    public Rigidbody bullet;
    public BaseWeaponData data;

    private Transform canonTransform;

    void Awake()
    {
        CoolDown = data.coolDown;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
        Rigidbody projectile = Instantiate(bullet, canon.position, canon.rotation) as Rigidbody;
        projectile.AddForce(canon.forward * 1000);
        LastActivity = Time.time;
    }
}
