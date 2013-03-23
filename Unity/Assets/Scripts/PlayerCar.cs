// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.
using UnityEngine;
using System.Collections;

public class PlayerCar : MonoBehaviour
{
	// ----------- CAR TUTORIAL-----------------
	
	// These variables allow the script to power the wheels of the car.
	public WheelCollider FrontLeftWheel;
	public WheelCollider FrontRightWheel;

	// These variables are for the gears, the array is the list of ratios. The script
	// uses the defined gear ratios to determine how much torque to apply to the wheels.
	public float[] GearRatio;
	public int CurrentGear = 0;

	// These variables are just for applying torque to the wheels and shifting gears.
	// using the defined Max and Min Engine RPM, the script can determine what gear the
	// car needs to be in.
	public float EngineTorque = 230.0f;
	public float MaxEngineRPM = 3000.0f;
	public float MinEngineRPM = 1000.0f;
	private float EngineRPM = 0.0f;

	void  Awake ()
	{
		// !! code bourrin
		if (!networkView.isMine) {
			Destroy (this);
		}
	}

	void  Start ()
	{
		// I usually alter the center of mass to make the car more stable. I'ts less likely to flip this way.
		rigidbody.centerOfMass += new Vector3 (0, -1, .25f);
	}

	void  Update ()
	{
	
		// Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
		EngineRPM = (FrontLeftWheel.rpm + FrontRightWheel.rpm) / 2 * GearRatio [CurrentGear];
		ShiftGears ();

		// set the audio pitch to the percentage of RPM to the maximum RPM plus one, this makes the sound play
		// up to twice it's pitch, where it will suddenly drop when it switches gears.
		audio.pitch = Mathf.Abs (EngineRPM / MaxEngineRPM) + 1.0f;
		// this line is just to ensure that the pitch does not reach a value higher than is desired.
		if (audio.pitch > 2.0f) {
			audio.pitch = 2.0f;
		}

		// finally, apply the values to the wheels.	The torque applied is divided by the current gear, and
		// multiplied by the user input variable.
		FrontLeftWheel.motorTorque = EngineTorque / GearRatio [CurrentGear] * Input.GetAxis ("Vertical");
		FrontRightWheel.motorTorque = EngineTorque / GearRatio [CurrentGear] * Input.GetAxis ("Vertical");
		
		// the steer angle is an arbitrary value multiplied by the user input.
		FrontLeftWheel.steerAngle = 10 * Input.GetAxis ("Horizontal");
		FrontRightWheel.steerAngle = 10 * Input.GetAxis ("Horizontal");
	}

	void  ShiftGears ()
	{
		// this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
		// the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
		if (EngineRPM >= MaxEngineRPM) {
			int AppropriateGear = CurrentGear;

			for (int i= 0; i < GearRatio.Length; i ++) {
				if (FrontLeftWheel.rpm * GearRatio [i] < MaxEngineRPM) {
					AppropriateGear = i;
					break;
				}
			}
		
			CurrentGear = AppropriateGear;
		}
	
		if (EngineRPM <= MinEngineRPM) {
			int AppropriateGear = CurrentGear;
		
			for (int j= GearRatio.Length-1; j >= 0; j --) {
				if (FrontLeftWheel.rpm * GearRatio [j] > MinEngineRPM) {
					AppropriateGear = j;
					break;
				}
			}

			CurrentGear = AppropriateGear;
		}
	}
}