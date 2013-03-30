using UnityEngine;
using System.Collections;

public class Mitraillette : BaseCapacity
{
    public Mitraillette()
    {
        this.Key = KeyCode.A;
        this.CoolDown = 12f;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}

