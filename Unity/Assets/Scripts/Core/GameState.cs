using UnityEngine;
using System.Collections;

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

	void Awake()
    {
		// Cache references
		this.m_context = GameSingleton.Instance.context;
		this.m_arena = (Arena) GameObject.FindObjectOfType(typeof(Arena));
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

		while (this.currentPhase == Phase.Initialization)
		{
			// Check arrival of players
			// TODO
			yield return null;
		}
	}

	IEnumerator State_CountDown()
	{
		while (this.currentPhase == Phase.CountDown)
		{
			yield return null;
		}
	}

	IEnumerator State_Fighting()
	{
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

}

