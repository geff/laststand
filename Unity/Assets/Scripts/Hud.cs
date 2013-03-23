using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
	public int capacityWidth = 100; // The width of the capacity button/texture2D to be displayed in the HUD	
	public void Start() {
		int i = 0;
		foreach (BaseCapacity capa in GameSingleton.Instance.context.player.playerTank.GetComponents<BaseCapacity>()) {
			GUISkill gsk = new GUISkill();
			gsk.positionX = i * capacityWidth + 10;
			gsk.setLinkedCapa(capa);
			skills.Add(gsk);
			i++;
		}
	}

	private IList<GUISkill> skills = new List<GUISkill>();
	
	void OnGUI () {
		foreach(GUISkill skill in skills) {
			skill.display();
		}
	}
}
