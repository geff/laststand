using UnityEngine;
using System.Collections;

public class BaseBehaviour : MonoBehaviour {

    private Transform myTransform;

    void Awake()
    {
        myTransform = transform;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision other)
    {
        other.transform.root.rigidbody.AddForce(myTransform.forward * 50);
        Destroy(gameObject);
    }
}
