using UnityEngine;
using System.Collections;

public class Dash : BaseCapacity
{
    public float Factor = 1f;

    public Dash()
    {
        this.Key = KeyCode.Mouse1;
        this.CoolDown = 12f;
        this.AnimationName = "";
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
        //TODO : code du Dash
		this.ParentVehicle.rigidbody.AddForce(this.ParentVehicle.transform.forward * 5000f * this.Factor , ForceMode.Force);
    }
}
