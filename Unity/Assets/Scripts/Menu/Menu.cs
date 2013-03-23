using UnityEngine;
using System.Collections;

public enum MenuState
{
	None = -1,
	Login,
	MainMenu,
	HostGame,
	JoinGame
}

public class Menu : MonoBehaviour
{
	public MenuState state = MenuState.None;

	#region Login
	public string username;
	#endregion // Login

	#region Host
	public int connectPort;
	public int maxPlayerNumber;
	public string gameName;
	#endregion // Host

	#region Join
	#endregion // Host

	void Awake()
	{
		this.username = "";
	}

	void OnGUI()
	{
		switch (this.state)
		{
		case MenuState.Login:
			Menu_Login();
			break;

		case MenuState.MainMenu:
			Menu_Main();
			break;

		case MenuState.HostGame:
			Menu_Host();
			break;

		case MenuState.JoinGame:
			Menu_Join();
			break;
		default:
			break;
		}
	}

	void Menu_Login()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

        GUILayout.Label("-- Login --");

		GUILayout.BeginHorizontal();
        GUILayout.Label("User name");
        this.username = GUILayout.TextField(this.username, 25, GUILayout.MinWidth(125));
		GUILayout.EndHorizontal();

        if (this.username.Length != 0)
        {
            if (GUILayout.Button("Ok"))
            {
				this.state = MenuState.MainMenu;
            }
        }

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	void Menu_Main()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

        GUILayout.Label("-- Main Menu --");
        if (GUILayout.Button("Create Game"))
        {
			this.state = MenuState.HostGame;
        }
        if (GUILayout.Button("Join Game"))
        {
			this.state = MenuState.JoinGame;
        }
		if (GUILayout.Button("<< Back"))
		{
			this.state = MenuState.Login;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	void Menu_Host()
	{
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
	
	        GUILayout.Label("-- Create Game --");

			GUILayout.BeginHorizontal();
	        GUILayout.Label("Server Port");
	        this.connectPort = int.Parse(GUILayout.TextField(this.connectPort.ToString(), 25, GUILayout.MinWidth(125)));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
	        GUILayout.Label("Max Players Nb");
	        this.maxPlayerNumber = int.Parse(GUILayout.TextField(this.maxPlayerNumber.ToString(), 25, GUILayout.MinWidth(125)));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
	        GUILayout.Label("Game Name");
	        this.gameName = GUILayout.TextField(this.gameName, 25, GUILayout.MinWidth(125));
			GUILayout.EndHorizontal();

			if (this.connectPort > 0 && this.maxPlayerNumber > 1 && GUILayout.Button("Create!"))
			{
				// Start a server for playerMaxNumber clients using the "connectPort" given via the GUI
                bool useNat = !Network.HavePublicAddress();
                NetworkConnectionError error = Network.InitializeServer(Mathf.Clamp(this.maxPlayerNumber, 1, 8) - 1, connectPort, useNat);

                if (string.IsNullOrEmpty(this.gameName))
                {
                    this.gameName = "Unnamed";
                }
                MasterServer.RegisterHost( GameContext.GAME_TYPE_NAME, this.gameName);
                Debug.Log("Try to host :" + error);
			}

			if (GUILayout.Button("<< Back"))
			{
				this.state = MenuState.MainMenu;
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
        else if (Network.peerType == NetworkPeerType.Server)
        {
			// TODO...
		}
	}

	void Menu_Join()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

        GUILayout.Label("-- Join Game --");

		if (GUILayout.Button("<< Back"))
		{
			this.state = MenuState.MainMenu;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
}

