using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/Smooth Follow")]
public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10;
    // the height we want the camera to be above the target
    public float height = 5;
    // How much we
    public float heightDamping = 3;
    public float rotationDamping = 3;

    public AnimationCurve zoomCurve;
    public AnimationCurve tiltCurve;

    public float scrollValue;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        scrollValue -= Input.GetAxis("Mouse ScrollWheel");

        scrollValue = Mathf.Clamp(scrollValue, -1f, 1f);

        this.height = zoomCurve.Evaluate(scrollValue);
        this.distance = tiltCurve.Evaluate(scrollValue);

        if (target)
        {
            // Calculate the current rotation angles
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            Vector3 pos = target.position;
            pos -= Vector3.forward * distance;
            pos.y = currentHeight;
            transform.position = pos;

            // Always look at the target
            transform.LookAt(target);
        }

		// Adapt the outline shader thickness in [0.002;0.004] (1-percentage) * (max - min) + min
		foreach (VehicleController vh in GameObject.FindObjectsOfType(typeof(VehicleController)))
			vh.SetOutlineThickness((1-ZoomPercentage) * 0.002f + 0.002f);
    }

	public float ZoomPercentage
	{
		get
		{
			return (this.height - zoomCurve.Evaluate(-1)) / (zoomCurve.Evaluate(1) - zoomCurve.Evaluate(-1));
		}
	}
}