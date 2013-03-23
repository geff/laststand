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
	public Texture2D GUIPicture;

    //[HideInInspector]
    public VehicleController ParentVehicle;

    public void Start()
    {
        this.ParentVehicle = this.transform.gameObject.GetComponentInChildren<VehicleController>();
        this.LastActivity = this.CoolDown;
    }

    public virtual void ApplyCapacity()
    {
        this.LastActivity = Time.time;
        Debug.Log(this.CapacityName + " " + this.LastActivity.ToString());
    }
}
