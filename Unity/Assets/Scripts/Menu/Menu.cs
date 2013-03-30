using UnityEngine;
using System.Collections;

public enum MenuState
{
	None = -1,
	Login,
	MainMenu,
	HostGame,
	JoinGame,
	SelectTank,
	StartOrWait,
	Pause,
}

public class Menu : MonoBehaviour
{
	public MenuState state = MenuState.None;

	private AssetHolder m_assetHolder;
	private GameContext m_context;

	#region Login
	string sUsername = "";
	string sMasterServer = GameContext.MASTER_SERVER_IP_ADDRESS;
	#endregion // Login

	#region Host
	int nConnectPort = 8888;
	int nMaxPlayerNumber = 8;
	string sGameName = "";
	#endregion // Host

	#region Join
	bool bManualJoin = false;
	string sConnectToIP = "127.0.0.1";
	bool bShowOkButton = false;
	HostData selectedHost = null;
	float fTimeSinceLastListLoad;
	Vector2 scrollViewVector;
	#endregion // Join

	#region Select Tank
	bool bSelected = false;
	int nTankID = -1;
	#endregion // Select Tank

	void Start()
	{
		this.sUsername = "";
		// Get the asset holder and cache the reference
		this.m_assetHolder = GameSingleton.Instance.assetHolder;
		// Get the game context and cache the reference
		this.m_context = GameSingleton.Instance.context;
	}

	void OnDestroy()
	{
		this.m_assetHolder = null;
		this.m_context= null;
		this.selectedHost = null;
	}

	void Update()
	{
		switch (this.state)
		{
		case MenuState.None:
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				this.state = MenuState.Pause;
				if (this.m_context.gameMode == GameMode.Solo)
				{
					Time.timeScale = 0.0f;
				}
			}
			break;

		case MenuState.Pause:
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				this.state = MenuState.None;
				if (this.m_context.gameMode == GameMode.Solo)
				{
					Time.timeScale = 1.0f;
				}
			}
			break;
		}
	}

	#region GUI
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

		case MenuState.SelectTank:
			Menu_SelectTank();
			break;

		case MenuState.StartOrWait:
			Menu_StartOrWait();
			break;

		case MenuState.Pause:
			switch (this.m_context.gameMode)
			{
			case GameMode.Multi:
				Menu_Pause_Multi();
				break;

			default:
			case GameMode.Solo:
				Menu_Pause_Solo();
				break;
			}
			break;

		default:
		case MenuState.None:
			break;
		}
	}

	private void Menu_Login()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

        GUILayout.Label("-- Login --");

		GUILayout.BeginHorizontal();
        GUILayout.Label("User name");
        this.sUsername = GUILayout.TextField(this.sUsername, 25, GUILayout.MinWidth(125));
		GUILayout.EndHorizontal();

        GUILayout.Label("-- Master Server --");

		GUILayout.BeginHorizontal();
        GUILayout.Label("IP");
        this.sMasterServer = GUILayout.TextField(this.sMasterServer, 25, GUILayout.MinWidth(125));
		GUILayout.EndHorizontal();

        if (this.sUsername.Length != 0 && this.sMasterServer.Length > 7)
        {
            if (GUILayout.Button("Ok"))
            {
				// Create the player
				PlayerData player = new PlayerData();
				player.username = this.sUsername;
				// Give it to the game context
				this.m_context.tempPlayer = player;
				// Goto main menu
				this.state = MenuState.MainMenu;

				if (!this.sMasterServer.Equals(GameContext.MASTER_SERVER_IP_ADDRESS))
				{
        			MasterServer.ipAddress = this.sMasterServer;
					MasterServer.ClearHostList ();
					MasterServer.RequestHostList (GameContext.GAME_TYPE_NAME);
				}
            }
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Auto host"))
        {
            this.sUsername = "AutoHostUser";
            this.sGameName = "AutoHostRoom";

            NetworkConnectionError error = Network.InitializeServer(Mathf.Clamp(this.nMaxPlayerNumber, 2, 8), nConnectPort, false);
            MasterServer.RegisterHost(GameContext.GAME_TYPE_NAME, this.sGameName);
            this.state = MenuState.SelectTank;
        }
        GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void Menu_Main()
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();

        GUILayout.Label("-- Main Menu --");
        if (GUILayout.Button("Create Game"))
        {
			// Go to host menu
			this.state = MenuState.HostGame;
        }
        if (GUILayout.Button("Join Game"))
        {
			// Go to join menu
			this.state = MenuState.JoinGame;
        }
		if (GUILayout.Button("<< Back"))
		{
			// Go back to login
			this.state = MenuState.Login;
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}

	private void Menu_Host()
	{
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
	
	        GUILayout.Label("-- Create Game --");

			GUILayout.BeginHorizontal();
	        GUILayout.Label("Server Port");
	        this.nConnectPort = int.Parse(GUILayout.TextField(this.nConnectPort.ToString(), 25, GUILayout.MinWidth(125)));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
	        GUILayout.Label("Max Players Nb");
	        this.nMaxPlayerNumber = int.Parse(GUILayout.TextField(this.nMaxPlayerNumber.ToString(), 25, GUILayout.MinWidth(125)));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
	        GUILayout.Label("Game Name");
	        this.sGameName = GUILayout.TextField(this.sGameName, 25, GUILayout.MinWidth(125));
			GUILayout.EndHorizontal();

			if (this.nConnectPort > 0 && this.nMaxPlayerNumber > 1 && GUILayout.Button("Create!"))
			{
				// Start a server for playerMaxNumber clients using the "connectPort" given via the GUI
//                bool useNat = !Network.HavePublicAddress();
                NetworkConnectionError error = Network.InitializeServer(Mathf.Clamp(this.nMaxPlayerNumber, 2, 8), nConnectPort, false);

                if (string.IsNullOrEmpty(this.sGameName))
                {
                    this.sGameName = "Unnamed";
                }
                MasterServer.RegisterHost( GameContext.GAME_TYPE_NAME, this.sGameName);
                Debug.Log("Try to host :" + error);
				// Goto select tank
				this.state = MenuState.SelectTank;
			}

			if (GUILayout.Button("<< Back"))
			{
				// Go back to main menu
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

	private void Menu_Join()
	{
        if (Network.peerType == NetworkPeerType.Disconnected)
        {
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
	
	        GUILayout.Label("-- Join Game --");

			if (GUILayout.Button("Manual settings"))
			{
				this.bManualJoin = !this.bManualJoin;

				if (!bManualJoin)
				{
                    MasterServer.RequestHostList(GameContext.GAME_TYPE_NAME);
				}
			}

			if (bManualJoin)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("ip: ");
				this.sConnectToIP = GUILayout.TextField(this.sConnectToIP);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("port: ");
				this.nConnectPort = int.Parse(GUILayout.TextField(this.nConnectPort.ToString()));
				GUILayout.EndHorizontal();

                if (GUILayout.Button("OK"))
                {
					// Connect to the "connectToIP" and "connectPort" as entered via the GUI
                    NetworkConnectionError error = Network.Connect(sConnectToIP, nConnectPort);
                    Debug.Log("Try to join :" + error);
                }
            }
            else
            {
                // refresh list if it is stalled (more than 10 seconds old)
                if ((Time.time - fTimeSinceLastListLoad) > 10.0f)
                {
                    fTimeSinceLastListLoad = Time.time;
                    MasterServer.ClearHostList();
                    MasterServer.RequestHostList(GameContext.GAME_TYPE_NAME);
                }

	            // Existing hosted games list
            	GUILayout.Label("Liste des parties");
				int nbGame = MasterServer.PollHostList().Length;
                if (nbGame != 0)
                {
				 	this.scrollViewVector = GUILayout.BeginScrollView(scrollViewVector);
					int index = 0;
                    foreach (HostData hostData in MasterServer.PollHostList())
                    {
						string buttonContent = hostData.gameName +"\n" +
							"["+ hostData.connectedPlayers +"/"+(hostData.playerLimit-1).ToString()+"]";

						if(GUILayout.Button(buttonContent))
						{
                            bShowOkButton = true;
                            selectedHost = hostData;
						}
						index++;
					}
					GUILayout.EndScrollView();
					

				}
				//refresh button
                if (GUILayout.Button("Refresh Host List"))
                {
                    MasterServer.ClearHostList();
                    MasterServer.RequestHostList(GameContext.GAME_TYPE_NAME);
                    bShowOkButton = false;
                }
				
	            // Selected game
				if (selectedHost != null)
				{
				}

				// Join button
                if (bShowOkButton && GUILayout.Button("OK"))
                {
                    // Connect to the "connectToIP" and "connectPort" as entered via the GUI
                    NetworkConnectionError error = Network.Connect(selectedHost);
                    Debug.Log("Try to join :" + error);
					// Goto select tank
					this.state = MenuState.SelectTank;
					this.m_context.player.currentState = PlayerState.SelectingTank;
                }
			}
	
			if (GUILayout.Button("<< Back"))
			{
				// Go back to main menu
				this.state = MenuState.MainMenu;
			}
	
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
	}

	private void Menu_SelectTank()
	{
		if (GUILayout.Button("Light Tank"))
		{
			this.nTankID = 0;
			this.bSelected = true;
		}
		if (GUILayout.Button("Medium Tank"))
		{
			this.nTankID = 1;
			this.bSelected = true;
		}
		if (GUILayout.Button("Heavy Tank"))
		{
			this.nTankID = 2;
			this.bSelected = true;
		}

		if (this.bSelected)
		{
			// Go to start or wait
			this.state = MenuState.StartOrWait;

			if (Network.isClient)
			{
				// Notify the server
				this.m_context.networkView.RPC("Ready", RPCMode.Server);
			}

			// Notify everyone
			this.m_context.networkView.RPC("TankChoice", RPCMode.AllBuffered, this.nTankID, Network.player);
			this.m_context.player.currentState = PlayerState.Ready;

			// Get the prefab
			VehicleController playerTank = null;
            switch ((TankType)this.nTankID)
			{
			case TankType.Light:
				playerTank = this.m_assetHolder.lightTank;
				break;

			case TankType.Medium:
				playerTank = this.m_assetHolder.mediumTank;
				break;

			case TankType.Heavy:
				playerTank = this.m_assetHolder.heavyTank;
				break;
			}
			this.m_context.player.playerTank = playerTank;
		}
	}

	private void Menu_StartOrWait()
	{
		if (Network.isServer)
		{
			bool bCanGo = true;
			// Check whether all players are ready
			foreach (PlayerData player in this.m_context.playerList.Values)
			{
				if (!player.isReady)
				{
					bCanGo = false;
				}
			}

			if (bCanGo)
			{
				if (GUILayout.Button("Go!"))
				{
					// Don't allow any more players
        			Network.maxConnections = -1;
					// Unregister to prevent new players from coming
//					MasterServer.UnregisterHost();
					// Disable menu
					this.state = MenuState.None;
					// Start level
					this.m_context.StartLevel();
				}
			}
			else
			{
				GUILayout.Label("Waiting clients...");
			}
		}
		else
		{
			GUILayout.Label("Waiting start of the game...");
		}
	}

	private void Menu_Pause_Multi()
	{

	}

	private void Menu_Pause_Solo()
	{
		if (GUILayout.Button("Resume"))
		{
			Time.timeScale = 1.0f;
			this.state = MenuState.None;
		}
		if (GUILayout.Button("Restart"))
		{
			Application.LoadLevel(Application.loadedLevel);
			this.state = MenuState.None;
		}
		if (GUILayout.Button("Main Menu"))
		{
			Application.LoadLevel("Menu");
			this.state = MenuState.None;
		}
	}
	#endregion // GUI
}

