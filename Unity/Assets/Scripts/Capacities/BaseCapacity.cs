using UnityEngine;
using System.Collections;
using System;

public abstract class BaseCapacity : MonoBehaviour
{
    /// <summary>
    /// Cooldown en secondes
    /// </summary>
    public float CoolDown;
    [HideInInspector]
    public float LastActivity;
    public KeyCode Key;
    public Texture2D GUIPicture;
    public AudioClip Sound;
    public int Volume;
    public string AnimationName;

    [HideInInspector]
    public VehicleController ParentVehicle;

    private Transform myTransform;
    private Transform mesh;
    private float lastSoundPlaying = 0f;

    public bool Cooling
    {
        get
        {
            return (Time.time - this.LastActivity) < this.CoolDown;
        }
    }

    public void Start()
    {
        myTransform = this.transform;
        this.ParentVehicle = this.transform.gameObject.GetComponentInChildren<VehicleController>();
        this.mesh = this.transform.FindChild("Mesh");
        this.LastActivity = -this.CoolDown;
    }

    public virtual void ApplyCapacity()
    {
        this.LastActivity = Time.time;
        
        if (Sound != null && (Time.time - lastSoundPlaying > Sound.length))
        {
            AudioSource.PlayClipAtPoint(Sound, myTransform.position, Volume);
            this.lastSoundPlaying = Time.time;
        }

        if (!String.IsNullOrEmpty(this.AnimationName) && this.mesh != null)
        {
            this.mesh.animation.Blend(this.AnimationName);
        }
        
        //Debug.Log(this.CapacityName + " " + this.LastActivity.ToString());
    }
}
