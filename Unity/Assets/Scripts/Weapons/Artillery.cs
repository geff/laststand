using UnityEngine;
using System.Collections;

public class Artillery : BaseCapacity
{
    public Artillery()
    {
        this.Key = KeyCode.E;
        this.CoolDown = 30f;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}