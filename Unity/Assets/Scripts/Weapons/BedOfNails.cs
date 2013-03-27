using UnityEngine;
using System.Collections;

public class BedOfNails : BaseCapacity
{
    public BedOfNails()
    {
        this.Key = KeyCode.Space;
        this.CoolDown = 20f;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}