using UnityEngine;
using System.Collections;

public class StdCanon : BaseCapacity {

    public Transform canon;
    public Rigidbody bullet;
    public FireWeaponData data;

    private Transform canonTransform;
    public BoxCollider hitbox;

    public StdCanon()
    {
        this.Key = KeyCode.Mouse0;
    }

    void Awake()
    {
        CoolDown = data.coolDown;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();

        Rigidbody projectile = Instantiate(bullet, canon.position, canon.rotation) as Rigidbody;
        Physics.IgnoreCollision(hitbox, projectile.collider);
        projectile.AddForce(canon.forward * data.speed + ParentVehicle.rigidbody.velocity);
    }
}
