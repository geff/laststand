using UnityEngine;

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

				// Mark this game object as persistent between scenes.
				DontDestroyOnLoad(go);
			}
			return s_instance;
		}
	}
	private static GameSingleton s_instance = null;

	public GameConfig config;
	public GameState gameState;
	
	void Awake()
	{
		// Get the game config.
		this.config = (GameConfig) Resources.Load("GameConfig", typeof(GameConfig));
	}
	
	void Start()
	{
		// Nothing to do.
		// Need to kept this method to ensure OnDestroy() will be called upon destruction.
	}
	
	void OnDestroy()
	{
		// Release references
		this.config = null;
		this.gameState = null;
		GameSingleton.s_instance = null;

		StopAllCoroutines();
	}
}
