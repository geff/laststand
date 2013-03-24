using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerData
{
    #region Profile
	public string username;
	#endregion // Profile
	
	#region In-Game Properties
	public NetworkPlayer networkPlayer;
	public int playerID;
	public TankType lastTankType;
	public VehicleController playerTank;
	#endregion // In-Game Properties

	#region Out-Game Properties
	public bool ready;
	#endregion // Out-Game Properties

	public bool isValid
	{
		get {
			return (this.playerID != PlayerData.INVALID_ID);
		}
	}
	
	public static readonly PlayerData INVALID = new PlayerData();
	public static readonly int INVALID_ID = -1;
	
	public PlayerData()
	{
		this.username = "Anonymous";
		this.playerID = PlayerData.INVALID_ID;
		this.ready = false;
	}

	#region System.Object overrides
    public override bool Equals(object obj)
    {
        PlayerData p = obj as PlayerData;

        if (p != null)
        {
            return p.username == this.username && p.playerID == this.playerID;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return this.networkPlayer.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("[PlayerData] username: {0}, ID: {1}, NetworkPlayer: {2}", this.username, this.playerID, this.networkPlayer);
    }
    #endregion
}
