using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
	public int capacityWidth = 100; // The width of the capacity button/texture2D to be displayed in the HUD
	public void Start() {
		int i = 0;
		//foreach (BaseCapacity capa in GameSingleton.Instance.context.playerList[GameSingleton.Instance.context.player.playerID].playerTank.GetComponents<BaseCapacity>()) {
		//	GUISkill gsk = new GUISkill();
		//	gsk.positionX = i * capacityWidth + 10;
		//	gsk.setLinkedCapa(capa);
		//	skills.Add(gsk);
		//	i++;
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
