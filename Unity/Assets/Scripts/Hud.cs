using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
	
	public void Start() {
		foreach (var capa in GameSingleton.Instance.context.playerList[GameSingleton.Instance.context.player.playerID].playerTank.GetComponents<BaseCapacity>()) {
			GUISkill gsk = new GUISkill();
			gsk.setLinkedCapa(capa);
			skills.Add(gsk);
		}
	}

	private IList<GUISkill> skills = new List<GUISkill>();
	
	void OnGUI () {
		Debug.Log("There is " + skills.Count);
		foreach(GUISkill skill in skills) {
			skill.display();
		}
	}
}
