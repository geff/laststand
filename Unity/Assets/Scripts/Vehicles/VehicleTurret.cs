using UnityEngine;
using System.Collections;

public class VehicleTurret : MonoBehaviour {

    public Transform myTransform;
    private Vector3 _originalRotation = Vector3.zero;

	void Awake()
	{
		if (!networkView.isMine)
		{
		//	this.enabled = false;
		}
	}

	// Use this for initialization
	void Start ()
    {
        myTransform = transform;
        _originalRotation = this.transform.localEulerAngles;
	}
	
	// Update is called once per frame
	void LateUpdate ()
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
        this.RotateTurret(angle);
    }

    public void RotateTurret(float angle)
    {
        myTransform.rotation = Quaternion.Euler(new Vector3(0 + _originalRotation.x, angle - 90 + _originalRotation.y, -90 + _originalRotation.z));
    }
}
