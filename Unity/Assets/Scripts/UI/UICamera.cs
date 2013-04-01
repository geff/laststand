using UnityEngine;
using System.Collections;

[AddComponentMenu("UI/Camera")]
[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	void Awake()
	{
		// Register itself to the UI manager
		UIManager.Instance.SetCamera(this);
	}
}