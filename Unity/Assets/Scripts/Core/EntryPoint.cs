using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Core/Entry Point")]
public class EntryPoint : MonoBehaviour
{
	void Awake()
	{
		// Get the game singleton
		GameSingleton singleton = GameSingleton.Instance;
	
	}
	
	IEnumerator Start()
	{
		// Get the game singleton
		GameSingleton singleton = GameSingleton.Instance;		
	}
	
	void OnDestroy()
	{
		// Add cleanup here...
	}
}
