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

	private PlayerCar m_car;

	void Awake()
	{
		this.m_car = GetComponent<PlayerCar>();

		if (!networkView.isMine)
		{
			this.enabled = false;
			this.m_car.enabled = false;
		}
	}

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

            if (capacity.enabled && (Time.time - capacity.LastActivity) > capacity.CoolDown && Input.GetKey(key) && capacity.enabled)
            {
                capacity.ApplyCapacity();
				networkView.RPC("DoAction", RPCMode.Others, key);
            }
        }
    }

	[RPC]
	internal void InitializeController(NetworkPlayer owner)
	{
		// Disable update on this script.
		this.enabled = false;
	}

	[RPC]
	internal void DoAction(string key)
	{
		BaseCapacity capacity;
		if (this.capacities.TryGetValue(key, out capacity))
		{
			// FIXME: synchronize network time
			if (capacity.enabled)
			{
				capacity.ApplyCapacity();
			}
		}
	}
}
