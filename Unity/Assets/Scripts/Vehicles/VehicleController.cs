using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class VehicleController : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<string, BaseCapacity> capacities;

    public int Life;
    public int MaxLife;

    // Use this for initialization
    void Start()
    {
        capacities = new Dictionary<string, BaseCapacity>();

        foreach (BaseCapacity capacity in GetComponents<BaseCapacity>())
        {
            this.capacities.Add(capacity.Key, capacity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (string key in capacities.Keys)
        {
            BaseCapacity capacity = capacities[key];

            if (Input.GetKey(key) && capacity.enabled && (Time.time - capacity.LastActivity) > capacity.CoolDown)
            {
                capacity.ApplyCapacity();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        Life -= damage;
        if (Life < 0)
            Life = 0;
    }

	[RPC]
	internal void InitializeController(NetworkPlayer owner)
	{

	}
}
