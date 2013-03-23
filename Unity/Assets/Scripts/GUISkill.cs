using UnityEngine;
using System.Collections;

public class GUISkill : MonoBehaviour {

	public int positionX = 10;
	public int positionY = 10;
	public int height = 100;
	public int width = 100;
	
	private BaseCapacity capa;
	
	public void display() {
		GUI.Label (new Rect (positionX, positionY, width, height), this.capa.GUIPicture);	
	}
	
	public GUISkill setLinkedCapa(BaseCapacity capa) {
		this.capa = capa;
	}
	
}
