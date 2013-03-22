using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Core/Game Singleton")]
public class GameSingleton : MonoBehaviour
{
	public static GameSingleton Instance
	{
		get
		{
			GameObject go = null;
			if (s_instance == null)
			{
				go = GameObject.Find("GameSingleton");
				if (go == null)
					go = new GameObject("GameSingleton");
				s_instance = go.AddComponent<GameSingleton>();
				
				DontDestroyOnLoad(go);
			}
			return s_instance;
		}
	}
	private static GameSingleton s_instance = null;
	
	void Awake()
	{
		
	}
	
	void Start()
	{
		// Nothing to do. Need to kept this method to ensure OnDestroy() will be called upon destruction.
	}
	
	void OnDestroy()
	{
		// Add cleanup here...
	}
}
