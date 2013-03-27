using UnityEngine;
using System.Collections;

public class AirCanon : BaseCapacity
{
    public AirCanon()
    {
        this.Key = KeyCode.R;
        this.CoolDown = 20f;
        this.AnimationName = "";
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}