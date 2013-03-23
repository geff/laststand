using System.Collections;
using UnityEditor;
using UnityEngine;

public class CustomAsset : MonoBehaviour
{
	#region Menu Entries
	[MenuItem("Assets/Create/Fire Weapon Data", priority = 40)]
	public static FireWeaponData CreateFireWeaponData()
	{
		return CreateAsset<FireWeaponData>();
	}

	[MenuItem("Assets/Create/Stun Weapon Data", priority = 40)]
	public static StunWeaponData CreateStunWeaponData()
	{
		return CreateAsset<StunWeaponData>();
	}

	[MenuItem("Assets/Create/Zone Weapon Data", priority = 40)]
	public static ZoneWeaponData CreateZoneWeaponData()
	{
		return CreateAsset<ZoneWeaponData>();
	}
	#endregion // Menu Entries

	#region Helper Methods
	private static T CreateAsset<T>()
	where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		string path = AssetDatabase.GenerateUniqueAssetPath(GetAssetPathFolder() + "/New"+typeof(T).ToString()+".asset");
		
		AssetDatabase.CreateAsset(asset, path);
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
		
		return asset;
	}

	private static string GetAssetPathFolder()
	{
		string path = "Assets";
		if(Selection.activeObject)
		{
			path = AssetDatabase.GetAssetPath(Selection.activeObject);
			if (path.LastIndexOf('.') != -1)
			{
				path = path.Substring(0, path.LastIndexOf('/'));
			}
		}
		return path;
	}
	#endregion // Helper Methods
}

