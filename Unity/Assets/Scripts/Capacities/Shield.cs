using UnityEngine;
using System.Collections;

public class Shield : BaseCapacity
{
    public Shield()
    {
        this.Key = KeyCode.E;
        this.CoolDown = 3f;
        this.AnimationName = "";
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}
