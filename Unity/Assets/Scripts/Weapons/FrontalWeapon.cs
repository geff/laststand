using UnityEngine;
using System.Collections;

public class FrontalWeapon : BaseCapacity
{
    public FrontalWeapon()
    {
        this.Key = KeyCode.R;
        this.CoolDown = 0f;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}
