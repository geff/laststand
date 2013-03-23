using UnityEngine;
using System.Collections;
using System;

public abstract class BaseCapacity : MonoBehaviour
{
    /// <summary>
    /// Cooldown en secondes
    /// </summary>
    public float CoolDown;
    public float LastActivity;
    public string Key;
    public string CapacityName;

    //[HideInInspector]
    public Vehicle ParentVehicle;

    public void Start()
    {
        this.ParentVehicle = this.transform.gameObject.GetComponentInChildren<Vehicle>();
        this.LastActivity = this.CoolDown;
    }

    public virtual void ApplyCapacity()
    {
        this.LastActivity = Time.time;
        Debug.Log(this.CapacityName + " " + this.LastActivity.ToString());
    }
}
