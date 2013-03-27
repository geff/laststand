using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class VehicleController : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<KeyCode, BaseCapacity> capacities;

    public int Life;
    public int MaxLife;

    private PlayerCar m_car;
    public bool IsDebug = false;

    void Awake()
    {
        this.m_car = GetComponent<PlayerCar>();

        // This script should never be updated for proxies.
        if (!networkView.isMine && !this.IsDebug)
        {
            this.enabled = false;
            this.m_car.enabled = false;
        }

        capacities = new Dictionary<KeyCode, BaseCapacity>();
        foreach (BaseCapacity capacity in GetComponents<BaseCapacity>())
        {
            this.capacities.Add(capacity.Key, capacity);
        }
    }

    void Start()
    {
        // Disable update on this script, waiting for the server to really start the game (Phase.Fighting).
        // Script is already disabled for proxies.
        if (networkView.isMine && !this.IsDebug)
        {
            this.enabled = false;
            this.m_car.enabled = false;
        }
    }

    void OnDestroy()
    {
        this.capacities.Clear();
        this.capacities = null;
        this.m_car = null;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyCode key in capacities.Keys)
        {
            BaseCapacity capacity = capacities[key];

            if (capacity.enabled && !capacity.Cooling && Input.GetKey(key))
            {
                capacity.ApplyCapacity();
                networkView.RPC("DoAction", RPCMode.Others, key);
            }
        }
    }

    [RPC]
    public void TakeDamage(int damage)
    {
        Life -= damage;
        if (Life < 0)
            Life = 0;
    }

    internal void SetPlayerControl(bool isControlledByPlayer)
    {
        if (networkView.isMine)
        {
            this.enabled = isControlledByPlayer;
            this.m_car.enabled = isControlledByPlayer;
        }
    }

    [RPC]
    internal void InitializeController(NetworkPlayer owner)
    {
        // TODO...
    }

    [RPC]
    internal void DoAction(KeyCode key)
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

    public void SetCapacitiesActivation(bool enabled)
    {
        BaseCapacity[] capacities = this.GetComponents<BaseCapacity>();

        foreach (BaseCapacity capacity in capacities)
        {
            capacity.enabled = enabled;
        }
    }

    public void AutoShoot(Vector3 target)
    {
        //---> Orientation de la tourelle
        Vector2 angleVec = new Vector3(target.x - this.transform.position.x, target.z - this.transform.position.z);
        float angle = Mathf.Atan2(angleVec.x, angleVec.y) * Mathf.Rad2Deg;

        this.gameObject.GetComponentInChildren<VehicleTurret>().RotateTurret(angle);

        StdCanon canon = this.gameObject.GetComponent<StdCanon>();

        if (canon != null && !canon.Cooling)
        {
            canon.ApplyCapacity();
        }
    }
}
