using UnityEngine;
using System.Collections;

[AddComponentMenu("Last Stand/Core/Game State")]
/// <summary>
/// This class holds the current state of the game.
/// The state is dependent on the current level.
/// </summary>
public class GameState : MonoBehaviour
{
	public enum Phase
	{
		None,
		Initialization,
		CountDown,
		Fighting,
		EndOfFight,
	}
	public Phase phase = GameState.Phase.None;

	// TODO...
}

