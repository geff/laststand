using UnityEngine;
using System.Collections;

public enum SceneMode
{
	None = 0,
	Title = 1,
	Menu = 3,
	Fight = 4,
	Loading = 5,
}

[AddComponentMenu("Last Stand/Core/Entry Point")]
public class EntryPoint : MonoBehaviour
{
	public SceneMode sceneMode = SceneMode.None;

	void Awake()
	{
		// Get the game singleton to make sure it is initialized here in the first scene
		GameSingleton singleton = GameSingleton.Instance;
	}

	bool bShowMessage;
	string sMessage = "";
	
	IEnumerator Start()
	{
		// Get the game singleton
		GameSingleton singleton = GameSingleton.Instance;

		switch (this.sceneMode)
		{
		case SceneMode.Title:
			// Show splash screen
			yield return new WaitForSeconds(2.0f);
			// Show message
			this.bShowMessage = true;
			this.sMessage = "Press any key to continue ...";
			while (!Input.anyKey)
			{
				yield return null;
			}
			this.bShowMessage = false;
			// Go to the menu
			Application.LoadLevel("Menu");
			break;
		case SceneMode.Menu:
			// Play the menu background music
			// Enable the menu
			singleton.menu.state = MenuState.Login;
			break;
		case SceneMode.Fight:
			// Create the game state
			singleton.gameState = gameObject.AddComponent<GameState>();
			break;
		case SceneMode.Loading:
			break;
		default:
			break;
		}
	}

	void OnGUI()
	{
		if (this.bShowMessage)
		{
			GUILayout.Label(this.sMessage);
		}
	}
	
	void OnDestroy()
	{
		// Add cleanup here...
	}
}
