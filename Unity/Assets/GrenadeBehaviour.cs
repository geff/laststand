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
            foreach (PlayerData p in GameSingleton.Instance.context.playerList.Values)
            {
                VehicleController vh = p.playerTank;
                if (Vector3.Distance(vh.transform.position, myTransform.position) <= data.radius)
                {
                    vh.TakeDamage(data.damage);
                    Explode();
                }
                Debug.Log(Vector3.Distance(vh.transform.position, myTransform.position) + " <= " + data.radius);
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
