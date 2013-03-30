using UnityEngine;
using System.Collections;

public class Mine : BaseCapacity
{
    public Mine()
    {
        this.Key = KeyCode.Space;
        this.CoolDown = 15f;
        this.AnimationName = "";
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}
