using UnityEngine;
using System.Collections;

public class BaseBehaviour : MonoBehaviour {

    private Transform myTransform;
    public FireWeaponData data;
    public Detonator explosion;

    private Vector3 startPos;

    void Awake()
    {
        myTransform = transform;
        startPos = myTransform.position;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (Vector3.Distance(myTransform.position, startPos) >= data.range)
            Destroy(gameObject);
	}

    void OnCollisionEnter(Collision other)
    {
        var vh = other.transform.root.GetComponent<VehicleController>();
        if (vh != null)
        {
            vh.TakeDamage(data.damage);
            vh.networkView.RPC("TakeDamage", RPCMode.Others, data.damage);
            vh.transform.root.rigidbody.AddForce(myTransform.forward * 500 * (data.weight / 10));
        }
        Destroy(gameObject);
    }
}
