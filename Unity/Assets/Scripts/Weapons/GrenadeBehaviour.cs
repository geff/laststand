using UnityEngine;
using System.Collections;

public class GrenadeBehaviour : MonoBehaviour
{

    private Transform myTransform;
    public ZoneWeaponData data;
    public Detonator explosion;

    private float startTime;

    void Awake()
    {
        myTransform = transform;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= startTime + data.timer)
        {
            foreach(VehicleController vh in GameObject.FindObjectsOfType(typeof(VehicleController)))
            {
                //VehicleController vh = p.playerTank;
                if (Vector3.Distance(vh.transform.position, myTransform.position) <= data.radius)
                {
                    Debug.Log("Boum grenade " + Vector3.Distance(vh.transform.position, myTransform.position) + " <= " + data.radius);
                    vh.TakeDamage(data.damage);
                    Explode();
                }  
            }
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(explosion, myTransform.position, myTransform.rotation);
        Destroy(gameObject);
    }
}
