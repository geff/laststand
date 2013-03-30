using UnityEngine;
using System.Collections;

public abstract class AbstractGameState : MonoBehaviour
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

    [HideInInspector]
    public string countDown = "";
    [HideInInspector]
    public string message = "";

	protected GameContext m_context;
	protected Arena m_arena;

	protected virtual void Awake()
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
	/// <summary>
	/// Default state.
	/// Does nothing.
	/// </summary>
	IEnumerator State_Idle()
	{
		while (this.currentPhase == Phase.Idle)
		{
			yield return null;
		}
	}

	/// <summary>
	/// Initialization state.
	/// </summary>
	protected abstract IEnumerator State_Initialization();

	protected virtual IEnumerator State_CountDown()
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

	protected virtual IEnumerator State_Fighting()
	{
		// Give control back to players
		this.m_context.player.playerTank.SetPlayerControl(true);
		Transform playerTransform = this.m_context.player.playerTank.transform;

		while (this.currentPhase == Phase.Fighting)
		{
			if (this.m_context.player.playerTank.Life <= 0 || playerTransform.position.y < -10.0f)
			{
				Camera.main.GetComponent<SmoothFollow>().target = null;
				this.currentPhase = Phase.EndOfFight;
			}
			yield return null;
		}
	}

	protected abstract IEnumerator State_EndOfFight();

	protected virtual IEnumerator State_Results()
	{
		while (this.currentPhase == Phase.Results)
		{
			if (Network.isServer && Network.connections.Length < 2)
			{
				this.currentPhase = Phase.Idle;
				Network.Disconnect();
				yield break;
			}
			yield return null;
		}
	}
	#endregion // Phases
}

