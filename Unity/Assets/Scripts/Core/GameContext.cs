using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Last Stand/Core/Game Context")]
[RequireComponent(typeof(NetworkView))]
public class GameContext : MonoBehaviour
{
	#region Players
	public PlayerData player
	{
		get {
			PlayerData tmp;
			if (this.playerList.TryGetValue(Network.player.GetHashCode(), out tmp))
			{
				return tmp;
			}

			return PlayerData.INVALID;
		}
	}
	public Dictionary<int, PlayerData> playerList;
	internal PlayerData tempPlayer;
	#endregion // Players

	#region Const Master Server Values
    internal const string MASTER_SERVER_IP_ADDRESS = "10.45.12.93";
    internal const int MASTER_SERVER_PORT = 23466;
    internal const int NAT_FACILITATOR_PORT = 50005;
	internal const string GAME_TYPE_NAME = "LastStand_Multi";
    #endregion

    private int m_nLastLevelPrefix = 0;

	void Awake()
	{
        // Initialize the master server
#if false
        MasterServer.ipAddress = GameContext.MASTER_SERVER_IP_ADDRESS;
        MasterServer.port = GameContext.MASTER_SERVER_PORT;
        Network.natFacilitatorIP = GameContext.MASTER_SERVER_IP_ADDRESS;
        Network.natFacilitatorPort = GameContext.NAT_FACILITATOR_PORT;
#endif
		// Request hosts list
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(GameContext.GAME_TYPE_NAME);

		// Initialize
		this.playerList = new Dictionary<int, PlayerData>(8);

		DontDestroyOnLoad(this);
		networkView.group = 1;
	}

    IEnumerator LoadStreamedLevel(string levelName, bool additive, bool testBeforeLoad)
    {
        if (testBeforeLoad)
        {
            while (!Application.CanStreamedLevelBeLoaded(levelName))
            {
                yield return null;
            }
        }

        if (additive)
        {
#if UNITY_PRO
            AsyncOperation async = Application.LoadLevelAdditiveAsync(levelName);
            yield return async;
#else
			Application.LoadLevelAdditive(levelName);
#endif
        }
        else
        {
#if UNITY_PRO
            AsyncOperation async = Application.LoadLevelAsync(levelName);
            yield return async;
#else
			Application.LoadLevel(levelName);
#endif
        }
    }

	#region Event Handlers
	[RPC]
	IEnumerator LoadLevel(string levelName, int levelPrefix)
	{
		this.m_nLastLevelPrefix = levelPrefix;

        // There is no reason to send any more data over the network on the default channel,
        // because we are about to load the level, thus all those objects will get deleted anyway
        Network.SetSendingEnabled(0, false);

        // We need to stop receiving because the level must be loaded first.
        // Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
        Network.isMessageQueueRunning = false;

        // All network views loaded from a level will get a prefix into their NetworkViewID.
        // This will prevent old updates from clients leaking into a newly created scene.
        Network.SetLevelPrefix(levelPrefix);

		yield return StartCoroutine(LoadStreamedLevel(levelName, false, true));
        // Awake and Start are called on each objects, so wait one or two frames.
        yield return null;
        yield return null;

        // Allow receiving data again
        Network.isMessageQueueRunning = true;
        // Now the level has been loaded and we can start sending out data to clients
        Network.SetSendingEnabled(0, true);

		// Broadcast OnNetworkLevelLoaded event on all GameObject
        foreach (Object go in FindObjectsOfType(typeof(GameObject)))
        {
            (go as GameObject).SendMessage("OnNetworkLevelLoaded", SendMessageOptions.DontRequireReceiver);
        }
	}

	#region Client
    void OnConnectedToServer()
    {
        Debug.Log("[GameContext]: OnConnectedToServer");

		// Temporary player should have been created when logging in.
        if (tempPlayer != null)
        {
			// First initialization of the player in all clients through RPC
            networkView.RPC("InitializePlayer", RPCMode.AllBuffered, Network.player, tempPlayer.username, PlayerData.INVALID_ID);
        }
        else
        {
            Debug.LogError("[OnConnectedToServer]: connected to server while player is not set.");
        }
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("[GameContext]: OnDisconnectedFromServer");

        // Clean up network
        if (Network.isServer)
        {
            Network.RemoveRPCsInGroup(0);
            Network.RemoveRPCsInGroup(1);
        }

        // Clean up players
        this.playerList.Clear();

        // Load the default scene
        // TODO...
    }
    #endregion

	[RPC]
	void InitializePlayer(NetworkPlayer networkPlayer, string username, int playerID)
	{
        Debug.Log(string.Format("[InitializePlayer]: networkID={0}, username={1}, playerID={2}",
                                networkPlayer, username, playerID));
        PlayerData player;
        if (!this.playerList.TryGetValue(networkPlayer.GetHashCode(), out player))
        {
            player = new PlayerData();
            this.playerList.Add(networkPlayer.GetHashCode(), player);
        }

        // Update player
        player.networkPlayer = networkPlayer;
        player.username = username;
        player.playerID = playerID;

		Debug.Log(string.Format("[InitializePlayer]: player {0} is {1}valid", username, player.isValid? "": "in"));
	}

	#region Server
    void OnServerInitialized()
    {
        Debug.Log("[GameContext]: OnServerInitialized");

		// Temporary player should have been created when logging in.
        if (tempPlayer != null)
        {
			// First initialization of the player in all clients through RPC
            networkView.RPC("InitializePlayer", RPCMode.AllBuffered, Network.player, tempPlayer.username, PlayerData.INVALID_ID);
        }
        else
        {
            Debug.LogError("[OnServerInitialized]: server created while player is not set.");
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("[GameContext]: OnPlayerDisconnected");

        // Clean up player in all clients
        this.playerList.Remove(player.GetHashCode());
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

	internal void StartLevel()
	{
		if (Network.isClient)
		{
			return;
		}

		// Finish player initialization
		int lastPlayerID = 1;
		foreach (KeyValuePair<int, PlayerData> pair in this.playerList)
		{
			if (pair.Key != Network.player.GetHashCode())
			{
				pair.Value.playerID = lastPlayerID++;
			}
		}

		// Remove any remaining event in server buffer
        Network.RemoveRPCsInGroup(0);
        Network.RemoveRPCsInGroup(1);

		// Send initialized player data to all clients
        foreach (PlayerData player in this.playerList.Values)
        {
            networkView.RPC("InitializePlayer", RPCMode.OthersBuffered, player.networkPlayer, player.username, player.playerID);
        }

		// Load the level on all clients (including server)
        networkView.RPC("LoadLevel", RPCMode.OthersBuffered, "Arena", this.m_nLastLevelPrefix + 1);
        StartCoroutine(LoadLevel("Arena", this.m_nLastLevelPrefix + 1));
	}
	#endregion // Server
	#endregion // Event Handlers

	[RPC]
	void Ready(NetworkMessageInfo info)
	{
		PlayerData player;
        if (this.playerList.TryGetValue(info.sender.GetHashCode(), out player))
        {
            player.ready = true;
			Debug.Log(string.Format("[Ready]: {0}", player.username));
        }
		else
		{
			Debug.LogError("[Ready]: unknown player");
		}
	}

	[RPC]
	void TankChoice(int tankID, NetworkPlayer sender)
	{
		PlayerData player;
        if (this.playerList.TryGetValue(sender.GetHashCode(), out player))
        {
            player.lastTankType = (TankType) tankID;
			Debug.Log(string.Format("[TankChoice]: {0}", player.lastTankType.ToString()));
        }
		else
		{
			Debug.LogError("[TankChoice]: unknown player");
		}
	}
}

