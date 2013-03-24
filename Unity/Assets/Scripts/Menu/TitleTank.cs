using UnityEngine;
using System.Collections;

public class TitleTank : MonoBehaviour
{
	public Transform[] wheels;
	public float rotationSpeed = 10.0f;
	
	// Update is called once per frame
	void Update ()
	{
		foreach (Transform wheel in this.wheels)
		{
			wheel.Rotate(Vector3.forward*rotationSpeed*Time.deltaTime);
		}
	}
}
