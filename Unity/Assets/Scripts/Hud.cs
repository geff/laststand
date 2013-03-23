using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour
{
	
	private GUISkill[] skills {
		get {
			if(null == _skills) {
				_skills = GetComponents<GUISkill>();
			}
			return _skills;
		}
	}
	
	private GUISkill[] _skills = null;
	
	void OnGUI () {
		foreach(GUISkill skill in skills) {
			Debug.Log("Blorg");
			skill.display();
		}
	}
}
