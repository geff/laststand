using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Last Stand/Core/Game State")]
/// <summary>
/// This class holds the current state of the game.
/// The state is dependent on the current level.
/// </summary>
[RequireComponent(typeof(NetworkView))]
public class GameState : MonoBehaviour
{
	public enum Phase
	{
		Idle,
		Initialization,
		CountDown,
		Fighting,
		EndOfFight,
		Results,
	}
	public Phase currentPhase = Phase.Initialization;

	private GameContext m_context;
	private Arena m_arena;
    private HashSet<int> m_synchronizedClients;

    [HideInInspector]
    public string countDown = "";
    [HideInInspector]
    public string message = "";

	void Awake()
    {
		// Cache references
		this.m_context = GameSingleton.Instance.context;
		this.m_arena = (Arena) GameObject.FindObjectOfType(typeof(Arena));
		this.m_synchronizedClients = new HashSet<int>();
	}

	IEnumerator Start()
	{
		// Wait one frame, so that everything else is properly initialized
		yield return null;

		// Start the finite state machine
		StartCoroutine(FSM());
	}

    /// <summary>
    /// State Machine entry point, acts as a state scheduler
    /// </summary>
    /// <returns></returns>
	IEnumerator FSM()
	{
        /// Execute the current coroutine (State)
        while (true)
        {
			switch (this.currentPhase)
			{
			default:
			case Phase.Idle:
				yield return StartCoroutine(State_Idle());
				break;
			case Phase.Initialization:
				yield return StartCoroutine(State_Initialization());
				break;
			case Phase.CountDown:
				yield return StartCoroutine(State_CountDown());
				break;
			case Phase.Fighting:
				yield return StartCoroutine(State_Fighting());
				break;
			case Phase.EndOfFight:
				yield return StartCoroutine(State_EndOfFight());
				break;
			case Phase.Results:
				yield return StartCoroutine(State_Results());
				break;
			}
        }
	}

	#region Phases
	IEnumerator State_Idle()
	{
		yield break;
	}

	IEnumerator State_Initialization()
	{
        /// Entering warmup state
        Debug.Log("State_Initialization");

		// Create the local player
		yield return StartCoroutine(SpawnPlayer());

		// Server waits for all players before starting the game
		if (Network.isServer)
		{
            // Executing warmup state
            bool waitingPlayers = true;
            while (waitingPlayers)
            {
                // Check wether all player are synchronized, or not
                waitingPlayers = (this.m_synchronizedClients.Count < this.m_context.playerList.Count - 1);
                yield return new WaitForFixedUpdate();
            }

            networkView.RPC("StartFight", RPCMode.OthersBuffered, (float) Network.time + 2.0f);
            StartCoroutine(StartFight((float) Network.time + 2.0f));
		}

		while (this.currentPhase == Phase.Initialization)
		{
			// Wait for state to change
			yield return null;
		}
	}

	IEnumerator State_CountDown()
	{
		float seconds = 3;
        while (seconds > 0)
        {
            // update GUI string
            countDown = seconds.ToString();
            --seconds;
            yield return new WaitForSeconds(1.0f);
        }
        
        countDown = "";

		this.currentPhase = Phase.Fighting;
	}

	IEnumerator State_Fighting()
	{
		// Give control back to players
		this.m_context.player.playerTank.SetPlayerControl(true);


		while (this.currentPhase == Phase.Fighting)
		{
			yield return null;
		}
	}

	IEnumerator State_EndOfFight()
	{
		while (this.currentPhase == Phase.EndOfFight)
		{
			yield return null;
		}
	}

	IEnumerator State_Results()
	{
		while (this.currentPhase == Phase.Results)
		{
			yield return null;
		}
	}
	#endregion // Phases

	#region Event Handlers
	[RPC]
	IEnumerator StartFight(float rendezvous)
	{
        // Re-initialize player in case all clients did not receive it
        this.m_context.player.playerTank.networkView.RPC("InitializeController", RPCMode.Others, Network.player);
        yield return null;

        Debug.Log("StartFight: rendezvous = " + rendezvous);
        float realRDV = matchToServerTime(rendezvous);
        Debug.Log("StartFight: realDV = " + realRDV);

        float delay = realRDV - (float) Network.time;
        Debug.Log("StartFight: delay = " + delay);
        if (delay > 0.0f)
        {
            yield return new WaitForSeconds(delay);
        }
        this.currentPhase = Phase.CountDown;
		this.m_context.player.currentState = PlayerState.Playing;
	}
	#endregion // Event Handlers

	/// <summary>
	/// Spawn a prefab for the player to control it
    /// the link with the server and all will be made automatically
	/// </summary>
	IEnumerator SpawnPlayer()
	{
		Transform spawn = this.transform;
		if (this.m_arena != null)
		{
			spawn = this.m_arena.spawnPoints[this.m_context.player.playerID];
		}

		VehicleController playerTank = null;
		// We are in a multiplayer game
		if (Network.isClient || Network.isServer)
		{
			// Instantiate a new object for this player, remember
            // static function Instantiate (prefab : Object, position : Vector3, rotation : Quaternion, group : int) : Object
            playerTank = (VehicleController) Network.Instantiate(this.m_context.player.playerTank, spawn.position, Quaternion.identity, 0);
			// Replace the prefab reference with the instantiated tank
            this.m_context.player.playerTank = playerTank;
			// Wait two frames (basically, wait for Awake and Start method to be called on the instantiated prefab)
            yield return null;
            yield return null;

            playerTank.networkView.RPC("InitializeController", RPCMode.Others, Network.player);
		}
		else
		{
			// Instantiate a new object for this player, remember
            // static function Instantiate (prefab : Object, position : Vector3, rotation : Quaternion, group : int) : Object
            playerTank = (VehicleController) Instantiate(this.m_context.player.playerTank, spawn.position, Quaternion.identity);
			// Replace the prefab reference with the instantiated tank
            this.m_context.player.playerTank = playerTank;
			// Wait two frames (basically, wait for Awake and Start method to be called on the instantiated prefab)
            yield return null;
            yield return null;
		}

		if (playerTank == null)
		{
			Debug.LogError("[SpawnPlayer]: Fatal Error!");
			yield break;
		}

		playerTank.GetComponent<VehicleController>().InitializeController(Network.player);
		// Setup camera
		Camera.main.GetComponent<SmoothFollow>().target = playerTank.transform;
	}

	#region Synchronization
    // SYNCHRONIZATION SERVEUR <-> CLIENT
    // STEP 1 : client ask for synch
    // STEP 2 : serveur send ITS Network.time to the client
    // STEP 3 : client receive and process : deltaTime = ServerNetwork.time + TransitTime - CLientNetworkTime
    // STEP 4 : To synchronize further events, the server will ask to launch a event at time = x.
    //          Client must launch it at (x - delta).

    private float deltaTime = 0.0f;

    /**
    * The server tells the client to synchronize time.
    */
    // SERVER
    [RPC]
    void ClientReady(NetworkMessageInfo info)
    {
        Debug.Log("[ClientReady]: " + info.sender);
        networkView.RPC("FindOutDeltaTime", info.sender, (float) Network.time);
    }

    // SERVER
    [RPC]
    void ClientSynchronized(NetworkMessageInfo info)
    {
        Debug.Log("[ClientSynchronized]: " + info.sender);
        this.m_synchronizedClients.Add(info.sender.GetHashCode());
    }

    // CLIENT
    void OnNetworkLevelLoaded()
    {
        if (Network.isClient)
        {
            Debug.Log("[OnNetworkLevelLoaded]: " + Network.player);
            networkView.RPC("ClientReady", RPCMode.Server);
        }
    }

    /**
    * Calculating the server Network.time for synchronization
    * @param serverTime A float reprensenting the Network.time of the server at the time this RPC was sent
    * @param info Data structure containing information on a message
    */
    // CLIENT
    [RPC]
    void FindOutDeltaTime(float serverTime, NetworkMessageInfo info)
    {
        // deltaTime = (serverTime + (float) (Network.time - info.timestamp)) - (float) Network.time;
        // Debug.Log("FindOutDeltaTime -deltaTime: "+deltaTime+" serverTime: "+serverTime+" info.timestamp:"+(float)info.timestamp   ) ;
		deltaTime = (float) (serverTime - info.timestamp);
        networkView.RPC("ClientSynchronized", RPCMode.Server);
    }

    /**
    * Calculate the real time the server want us to use.
    * @param localT the time we received from the server
    * @return the time the server really meant
    */
    float matchToServerTime(float localT)
    {
        // Debug.Log("matchToServerTime - localT - deltaTime  ->   "+(localT - deltaTime)+" = "+localT+" - "+deltaTime) ;
		return (localT - deltaTime);
    }
    #endregion

}

