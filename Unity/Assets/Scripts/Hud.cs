using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
	public int capacityWidth = 100; // The width of the capacity button/texture2D to be displayed in the HUD	
	public void Start() {
		Debug.Log("HUD::Start()");
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
	
	public void OnGUI () {
		// start debug code
		int i = 0;
//		skills = new List<GUISkill>();
		// the following code is as it is because of an unkown state when executing Start() method and no time for fixing that
		foreach (BaseCapacity capa in GameSingleton.Instance.context.player.playerTank.capacities.Values) {
			GUISkill gsk = new GUISkill();
			gsk.positionX = i * capacityWidth + 10;
			gsk.setLinkedCapa(capa);
			gsk.display();
//			skills.Add(gsk);
			i++;
		}
		// end debug code
//		foreach(GUISkill skill in skills) {
//			skill.display();
//		}
	}
}
