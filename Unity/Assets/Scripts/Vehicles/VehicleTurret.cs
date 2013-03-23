using UnityEngine;
using System.Collections;

public class VehicleTurret : MonoBehaviour {

    private Transform myTransform;

	// Use this for initialization
	void Start ()
    {
        myTransform = transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        AimAtMouse();
	}

    private Vector3 mousePosition, turretPosition;
    private float angle;
    void AimAtMouse()
    {
        mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.x - Camera.mainCamera.transform.position.x;
        turretPosition = Camera.main.WorldToScreenPoint(myTransform.position);
        mousePosition.x = mousePosition.x - turretPosition.x;
        mousePosition.y = mousePosition.y - turretPosition.y;
        angle = Mathf.Atan2(mousePosition.x, mousePosition.y) * Mathf.Rad2Deg;
        myTransform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

}
