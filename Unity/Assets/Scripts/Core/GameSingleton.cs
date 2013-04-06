using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Core/Game Singleton")]
[RequireComponent(typeof(GameContext))]
[RequireComponent(typeof(Menu))]
public class GameSingleton : MonoBehaviour
{
	internal static GameSingleton Instance
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

	internal AssetHolder assetHolder
	{
		get;
		private set;
	}
	internal GameConfig config
	{
		get;
		private set;
	}
	internal GameContext context
	{
		get;
		private set;
	}
	internal AbstractGameState gameState;
	internal Menu menu
	{
		get;
		private set;
	}
	
	void Awake()
	{
		Debug.Log("GameSingleton.Awake()");

		// Get the game config.
		this.config = (GameConfig) Resources.Load("GameConfig", typeof(GameConfig));
		// Get the game context.
		this.context = GetComponent<GameContext>();
		// Get the menu.
		this.menu = GetComponent<Menu>();
		// Get the asset holder
		this.assetHolder = (AssetHolder) Resources.Load ("AssetHolder", typeof(AssetHolder));
	}
	
	void Start()
	{
		Debug.Log("GameSingleton.Start()");

		// Nothing to do.
		// Need to kept this method to ensure OnDestroy() will be called upon destruction.
	}
	
	void OnDestroy()
	{
		Debug.Log("GameSingleton.OnDestroy()");

		// Release references
		this.config = null;
		this.context = null;
		this.gameState = null;
		GameSingleton.s_instance = null;

		StopAllCoroutines();
	}
}
