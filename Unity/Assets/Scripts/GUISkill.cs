using UnityEngine;
using System.Collections;

public class GUISkill : MonoBehaviour {

	public Texture2D skillPicture;
	public int positionX = 10;
	public int positionY = 10;
	public int height = 100;
	public int width = 100;
	
	public void display() {
		GUI.Label (new Rect (positionX, positionY, width, height), skillPicture);	
	}
	
	
}
