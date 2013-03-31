using UnityEngine;
using System.Collections;

public class Repulse : BaseCapacity
{
    public float Range;
    public float Force;

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();

        foreach (VehicleController vh in GameObject.FindObjectsOfType(typeof(VehicleController)))
        {
            //VehicleController vh = p.playerTank;
            if (Vector3.Distance(vh.transform.position, ParentVehicle.transform.position) <= Range)
            {
                Vector3 direction = vh.transform.position - ParentVehicle.transform.position;
                //direction.y = 0;
                direction.Normalize();
                vh.rigidbody.AddForce(direction * Force);
            }
        }
    }

}
