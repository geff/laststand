using UnityEngine;
using System.Collections;

public class BaseBehaviour : MonoBehaviour {

    private Transform myTransform;
    public FireWeaponData data;

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
        other.transform.root.rigidbody.AddForce(myTransform.forward * 50 * (data.weight / 10));
        Destroy(gameObject);
    }
}
