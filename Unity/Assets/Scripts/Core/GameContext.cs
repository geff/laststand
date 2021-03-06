using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameMode
{
	Solo,
	Multi
}

[AddComponentMenu("Last Stand/Core/Game Context")]
[RequireComponent(typeof(NetworkView))]
public class GameContext : MonoBehaviour
{
	public GameMode gameMode = GameMode.Solo;

	#region Audio
	internal const int AUDIOSRC_AMB = 0;
	internal const int AUDIOSRC_SFX = 0;

	public AudioListener audioListener {
		get;
		private set;
	}

	private AudioSource[] m_bgmAudioSourceArray = null;
	#endregion // Audio

	#region Players
	public PlayerData player {
		get {
			switch (this.gameMode)
			{
			case GameMode.Multi:
				PlayerData tmp;
				if (this.playerList.TryGetValue (Network.player.GetHashCode (), out tmp)) {
					return tmp;
				}
	
				return PlayerData.INVALID;

			default:
			case GameMode.Solo:
				return this.tempPlayer;
			}
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

	void Awake ()
	{
		// Initialize the master server
#if false
        MasterServer.ipAddress = GameContext.MASTER_SERVER_IP_ADDRESS;
        MasterServer.port = GameContext.MASTER_SERVER_PORT;
        Network.natFacilitatorIP = GameContext.MASTER_SERVER_IP_ADDRESS;
        Network.natFacilitatorPort = GameContext.NAT_FACILITATOR_PORT;
#endif
		// Request hosts list
		MasterServer.ClearHostList ();
		MasterServer.RequestHostList (GameContext.GAME_TYPE_NAME);

		// Initialize players
		this.playerList = new Dictionary<int, PlayerData> (8);
		this.tempPlayer = new PlayerData();

		DontDestroyOnLoad (this);
		networkView.group = 1;

		this.m_bgmAudioSourceArray = new AudioSource[2];
		// Add an audio source for BGM music
		this.m_bgmAudioSourceArray[AUDIOSRC_AMB] = gameObject.AddComponent<AudioSource>();
		this.m_bgmAudioSourceArray[AUDIOSRC_AMB].playOnAwake = false;
		// Add an audio source for SFXs
		this.m_bgmAudioSourceArray[AUDIOSRC_SFX] = gameObject.AddComponent<AudioSource>();
		this.m_bgmAudioSourceArray[AUDIOSRC_SFX].playOnAwake = false;
	}

	void OnDestroy ()
	{
		this.audioListener = null;
		this.m_bgmAudioSourceArray = null;
	}

	void OnLevelWasLoaded (int level)
	{
		// Force fullscreen clearing (color AND depth)
		GL.Clear (true, true, Color.black);
		InitializeAudioListener ();
	}

	#region Audio Management
	private void InitializeAudioListener ()
	{
		this.audioListener = (AudioListener)FindObjectOfType (typeof(AudioListener));
		if (this.audioListener)
		{
			AudioSource snd = GetAudioSource (AUDIOSRC_AMB);
			if (snd != null) {
				// TODO: set volume based on GameConfig
			}
			// TODO: set SFX volume
		}
	}
	
	internal AudioSource GetAudioSource (int nNumAudioSource)
	{
		if (this.m_bgmAudioSourceArray != null && this.m_bgmAudioSourceArray.Length > nNumAudioSource)
			return this.m_bgmAudioSourceArray [nNumAudioSource];
		return null;
	}
	
	// Ambiance Management
	internal void PlayAmbiance (AudioClip clip)
	{
		// No Clip
		if (clip == null) {
			return;
		}
		// Try to initialize the AudioListener
		if (!this.audioListener) {
			InitializeAudioListener ();
		}
		// No AudioListener found
		if (!this.audioListener) {
			Debug.LogError ("AudioListener not found !!!");
			return;
		}

		AudioSource snd = GetAudioSource (AUDIOSRC_AMB);
		if (snd != null && snd.clip != clip)
		{
			snd.ignoreListenerVolume = true;
			if( snd.isPlaying)
			{
				StartCoroutine (FadeAmbiance(snd, clip));
			}
			else
			{
				snd.Stop();
				snd.clip = clip;
				snd.loop = true;
				// TODO: set volume based on GameConfig
				snd.Play();
			}
		}
	}

	private IEnumerator FadeAmbiance (AudioSource snd, AudioClip clip)
	{
		float prevVolume = snd.volume;
		// FIXME: fade speed and fade min (GameConfig?)
		while( snd.volume > 0.05f )
		{
			snd.volume = Mathf.Lerp (snd.volume, 0.0f, 1.5f*Time.deltaTime);
			yield return null;
		}
		snd.Stop ();
		snd.clip = clip;
		snd.loop = true;
		snd.Play ();
		// FIXME: fade speed and fade min (GameConfig?)
		while( snd.volume < (prevVolume - 0.01f) )
		{
			snd.volume = Mathf.Lerp (snd.volume, prevVolume + 0.01f, 2.0f*Time.deltaTime);
			yield return null;
		}
		snd.volume = prevVolume;
	}

	internal void StopAmbiance ()
	{
		AudioSource snd = GetAudioSource (AUDIOSRC_AMB);
		if (snd) {
			snd.Stop ();
			snd.volume = 0;
		} else {
			Debug.Log ("audioSrc not set !!!");
		}
	}
	
	// SFX Management
	internal void PlaySFX (SFX sfx)
	{
		PlaySFX (GameSingleton.Instance.assetHolder.SFXs [(int)sfx]);
	}
	
	internal void PlaySFX (AudioClip clip)
	{
		if (clip != null) {
			AudioSource snd = GetAudioSource (AUDIOSRC_SFX);
			if (snd != null && !snd.isPlaying) {
				snd.clip = clip;
				snd.loop = false;
				snd.volume = AudioListener.volume;
				snd.Play ();
			}
		}
	}
	
	internal void PlayOneShotSFX (SFX sfx)
	{
		PlayOneShotSFX (sfx, 1.0f);
	}

	internal void PlayOneShotSFX (SFX sfx, float fPitch)
	{
		PlayOneShotSFX (sfx, 1.0f, 1.0f);
	}

	internal void PlayOneShotSFX (SFX sfx, float fPitch, float fVolume)
	{
		PlayOneShotSFX (GameSingleton.Instance.assetHolder.SFXs [(int)sfx], fPitch, fVolume);
	}

	internal void PlayOneShotSFX (AudioClip clip, float fPitch, float fVolume)
	{
		if (clip != null) {
			AudioSource snd = GetAudioSource (AUDIOSRC_SFX);
			if (snd != null) {
				snd.pitch = fPitch;
				snd.volume = fVolume;
				snd.PlayOneShot (clip);
			}
		}
	}

	#endregion // AudioManagement

	IEnumerator LoadStreamedLevel (string levelName, bool additive, bool testBeforeLoad)
	{
		if (testBeforeLoad) {
			while (!Application.CanStreamedLevelBeLoaded(levelName)) {
				yield return null;
			}
		}

		if (additive) {
#if UNITY_PRO
            AsyncOperation async = Application.LoadLevelAdditiveAsync(levelName);
            yield return async;
#else
			Application.LoadLevelAdditive (levelName);
#endif
		} else {
#if UNITY_PRO
            AsyncOperation async = Application.LoadLevelAsync(levelName);
            yield return async;
#else
			Application.LoadLevel (levelName);
#endif
		}
	}

	#region Event Handlers
	[RPC]
	IEnumerator LoadLevel (string levelName, int levelPrefix)
	{
		this.m_nLastLevelPrefix = levelPrefix;

		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled (0, false);

		// We need to stop receiving because the level must be loaded first.
		// Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;

		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix (levelPrefix);

		yield return StartCoroutine(LoadStreamedLevel(levelName, false, true));
		// Awake and Start are called on each objects, so wait one or two frames.
		yield return null;
		yield return null;

		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data to clients
		Network.SetSendingEnabled (0, true);

		// Broadcast OnNetworkLevelLoaded event on all GameObject
		foreach (Object go in FindObjectsOfType(typeof(GameObject))) {
			(go as GameObject).SendMessage ("OnNetworkLevelLoaded", SendMessageOptions.DontRequireReceiver);
		}
	}

	#region Client
	void OnConnectedToServer ()
	{
		Debug.Log ("[GameContext]: OnConnectedToServer");

		// Temporary player should have been created when logging in.
		if (tempPlayer != null) {
			// First initialization of the player in all clients through RPC
			networkView.RPC ("InitializePlayer", RPCMode.AllBuffered, Network.player, tempPlayer.username, PlayerData.INVALID_ID);
		} else {
			Debug.LogError ("[OnConnectedToServer]: connected to server while player is not set.");
		}
	}

	void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		Debug.Log ("[GameContext]: OnDisconnectedFromServer");

		// Clean up network
		if (Network.isServer) {
			Network.RemoveRPCsInGroup (0);
			Network.RemoveRPCsInGroup (1);
		}

		// Clean up players
		this.playerList.Clear ();

		// Load the default scene
		Application.LoadLevel("Menu");
	}
    #endregion

	[RPC]
	void InitializePlayer (NetworkPlayer networkPlayer, string username, int playerID)
	{
		Debug.Log (string.Format ("[InitializePlayer]: networkID={0}, username={1}, playerID={2}",
                                networkPlayer, username, playerID));
		PlayerData player;
		if (!this.playerList.TryGetValue (networkPlayer.GetHashCode (), out player)) {
			player = new PlayerData ();
			this.playerList.Add (networkPlayer.GetHashCode (), player);
		}

		// Update player
		player.networkPlayer = networkPlayer;
		player.username = username;
		player.playerID = playerID;

		Debug.Log (string.Format ("[InitializePlayer]: player {0} is {1}valid", username, player.isValid ? "" : "in"));
	}

	#region Server
	void OnServerInitialized ()
	{
		Debug.Log ("[GameContext]: OnServerInitialized");

		// Temporary player should have been created when logging in.
		if (tempPlayer != null) {
			// First initialization of the player in all clients through RPC
			networkView.RPC ("InitializePlayer", RPCMode.AllBuffered, Network.player, tempPlayer.username, 0);
		} else {
			Debug.LogError ("[OnServerInitialized]: server created while player is not set.");
		}
	}

	void OnPlayerDisconnected (NetworkPlayer player)
	{
		Debug.Log ("[GameContext]: OnPlayerDisconnected");

		// Clean up player in all clients
		this.playerList.Remove (player.GetHashCode ());
		Network.RemoveRPCs (player);
		Network.DestroyPlayerObjects (player);
	}

	internal void StartLevel ()
	{
		if (Network.isClient) {
			return;
		}

		// Finish player initialization
		// 0: is the server
		int lastPlayerID = 1;
		foreach (KeyValuePair<int, PlayerData> pair in this.playerList) {
			if (pair.Key != Network.player.GetHashCode ()) {
				pair.Value.playerID = lastPlayerID++;
			}
		}

		// Remove any remaining event in server buffer
		Network.RemoveRPCsInGroup (0);
		Network.RemoveRPCsInGroup (1);

		// Send initialized player data to all clients
		foreach (PlayerData player in this.playerList.Values) {
			networkView.RPC ("InitializePlayer", RPCMode.OthersBuffered, player.networkPlayer, player.username, player.playerID);
		}

		// Load the level on all clients (including server)
		networkView.RPC ("LoadLevel", RPCMode.OthersBuffered, "Arena", this.m_nLastLevelPrefix + 1);
		StartCoroutine (LoadLevel ("Arena", this.m_nLastLevelPrefix + 1));
	}
	#endregion // Server
	#endregion // Event Handlers

	[RPC]
	void Ready (NetworkMessageInfo info)
	{
		PlayerData player;
		if (this.playerList.TryGetValue (info.sender.GetHashCode (), out player))
		{
			player.currentState = PlayerState.Ready;
			Debug.Log (string.Format ("[Ready]: {0}", player.username));
		}
		else
		{
			Debug.LogError ("[Ready]: unknown player");
		}
	}

	[RPC]
	void TankChoice(int tankID, NetworkPlayer sender)
	{
		PlayerData player;
		if (this.playerList.TryGetValue (sender.GetHashCode(), out player))
		{
			player.lastTankType = (TankType)tankID;
			Debug.Log (string.Format ("[TankChoice]: {0}", player.lastTankType.ToString ()));
		}
		else
		{
			Debug.LogError ("[TankChoice]: unknown player");
		}
	}

	[RPC]
	void RemovePlayer(NetworkPlayer sender)
	{
		// Clean up player in all clients
		this.playerList.Remove (player.GetHashCode ());
	}
}

