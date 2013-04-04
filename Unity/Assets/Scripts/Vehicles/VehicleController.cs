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
	public float MinSidewayFriction = 20;
	public float MaxSidewayFriction = 400;

    private PlayerCar m_car;
    public bool IsDebug = false;

    private WheelCollider[] wheelColliders;

	private List<Material> toonOutlineShaders;

    void Awake()
    {
        wheelColliders = GetComponentsInChildren<WheelCollider>();
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

		toonOutlineShaders = new List<Material>();
		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			foreach (Material m in r.materials)
			{
				if (m.HasProperty("_Outline")) // only add the outline shaders
					toonOutlineShaders.Add(m);
			}
		}
		Debug.Log("Found " + toonOutlineShaders.Count + " outline shaders.");
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
				if (Network.isClient || Network.isServer)
				{
					networkView.RPC("DoAction", RPCMode.Others, (int)key);
				}
            }
        }
    }

	public void SetOutlineThickness(float value)
	{
		foreach (Material m in toonOutlineShaders)
		{
			m.SetFloat("_Outline", value);
		}
	}

   /* public void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag != "Ground")
            StartCoroutine(Derapage());
    }*/


    [RPC]
    public void TakeDamage(int damage)
    {
        Life -= damage;
        if (Life < 0)
            Life = 0;
    }

    private WheelFrictionCurve sidewayFriction;
    IEnumerator Derapage()
    {
        foreach(WheelCollider wc in wheelColliders)
        {
            sidewayFriction = wc.sidewaysFriction;
            sidewayFriction.extremumValue = 20;
            wc.sidewaysFriction = sidewayFriction;
        }

        float fStart = Time.time;
        float fPassed = 0.0f;

        do {
            foreach(WheelCollider wc in wheelColliders)
            {
                sidewayFriction = wc.sidewaysFriction;
                sidewayFriction.extremumValue = Mathf.Lerp(MinSidewayFriction,MaxSidewayFriction, fPassed);
                wc.sidewaysFriction = sidewayFriction;
            }
            fPassed = (Time.time - fStart) / Mathf.Abs(transform.InverseTransformDirection(rigidbody.velocity).x)*0.5f;
            yield return null;
        } while (fPassed < 1.0f);
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
    internal void DoAction(int key)
    {
        BaseCapacity capacity;
        if (this.capacities.TryGetValue((KeyCode) key, out capacity))
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
