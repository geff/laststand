using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Vehicle/Movement")]
/// <summary>
/// This class is responsible of moving the vehicle.
/// It should be used for both player and non-player vehicles, therefore must stay generic.
/// Player input should be read from another script.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VehicleMovement : MonoBehaviour
{
	// TODO...
}

