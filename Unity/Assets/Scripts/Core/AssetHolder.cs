using UnityEngine;
using System.Collections;

internal enum SFX
{
	None = -1,
	ArtillerieBigTank,
	BigTurretShotImpact,
	DashVehicule,
	EMP_AOE_SoundEffect,
	Grenade_Explosion,
	Horn_Big_Vehicule,
	Horn_Medium_Vehicule,
	Horn_Small_Vehicule,
	MediumTurretShotImpact,
	MitrailleuseBigTank,
	RepulseSmallVehicule,
	ShieldActivation,
	SmallTurretShotImpact,
	TapisDeClousBigTank,
	TirAirCannonMediumTank,
	TirDuLanceGrenade,
	TirDuShotgun,
	TirTurretBigTank,
	TirTurretMediumVehicule,
	TirTurretSmallVehicule,
	Vehicule_Explosion,
}

public class AssetHolder : MonoBehaviour
{
	#region Vehicles
	public VehicleController lightTank;
	public VehicleController mediumTank;
	public VehicleController heavyTank;
	#endregion // Vehicles

	#region Sfx & Bgm
	public AudioClip menuMusic;
	public AudioClip inGameMusic;
	public AudioClip[] SFXs;
	#endregion // SFX & BGM
}

