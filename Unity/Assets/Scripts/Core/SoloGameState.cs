using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Core/Solo Game State")]
/// <summary>
/// This class holds the current state of the game.
/// The state is dependent on the current level.
/// </summary>
public class SoloGameState : AbstractGameState
{

	#region Phases
	protected override IEnumerator State_Initialization()
	{
		Transform spawn = this.transform;
		if (base.m_arena != null)
		{
			spawn = base.m_arena.spawnPoints[0];
		}

		// Instantiate a new object for this player, remember
        // static function Instantiate (prefab : Object, position : Vector3, rotation : Quaternion, group : int) : Object
        VehicleController playerTank = (VehicleController) Instantiate(GameSingleton.Instance.assetHolder.lightTank, spawn.position, Quaternion.identity);
		// Replace the prefab reference with the instantiated tank
        this.m_context.player.playerTank = playerTank;
		this.m_context.player.playerID = 0;
		// Wait two frames (basically, wait for Awake and Start method to be called on the instantiated prefab)
        yield return null;
        yield return null;

		if (playerTank == null)
		{
			Debug.LogError("[SpawnPlayer]: Fatal Error!");
			yield break;
		}

		// Setup camera
		Camera.main.GetComponent<SmoothFollow>().target = playerTank.transform;

		// Ready to go
        base.currentPhase = Phase.CountDown;
		base.m_context.player.currentState = PlayerState.Playing;
	}

	protected override IEnumerator State_EndOfFight()
	{
		// FIXME: for now does nothing
		while (base.currentPhase == Phase.EndOfFight)
		{
			yield return null;
		}
	}
	#endregion // Phases
}

