using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
	
	public void Start() {
		//foreach (BaseCapacity capa in GameSingleton.Instance.context.playerList[GameSingleton.Instance.context.player.playerID].playerTank.GetComponents<BaseCapacity>()) {
		//	GUISkill gsk = new GUISkill();
		//	gsk.setLinkedCapa(capa);
		//	skills.Add(gsk);
		//}
		// debug code :
		BaseCapacity dash = new Dash();
		GUISkill gsk2 = new GUISkill();
		gsk2.setLinkedCapa(dash);
		skills.Add(gsk2);
	}

	private IList<GUISkill> skills = new List<GUISkill>();
	
	void OnGUI () {
		foreach(GUISkill skill in skills) {
			skill.display();
		}
	}
}
