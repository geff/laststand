using UnityEngine;
using System.Collections;

public class Invisibility : BaseCapacity
{
    public Invisibility()
    {
        this.Key = KeyCode.Space;
    }

    public override void ApplyCapacity()
    {
        base.ApplyCapacity();
    }
}