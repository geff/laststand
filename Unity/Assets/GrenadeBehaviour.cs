using UnityEngine;
using System.Collections;

public class GrenadeBehaviour : MonoBehaviour
{

    private Transform myTransform;
    public ZoneWeaponData data;

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
            Destroy(gameObject);
    }
}
