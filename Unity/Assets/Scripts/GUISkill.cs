using UnityEngine;
using System.Collections;

public class GUISkill : MonoBehaviour {

	public int positionX = 10;
	public int positionY = 10;
	public int height = 100;
	public int width = 100;
	
	private BaseCapacity capa;
	
	private bool isReady() {
		if (100 == getPercentage()) {
			return true;
		}
		return false;
	}
	
	private int getPercentage() { 
		return (int)Mathf.Round((Time.time - capa.LastActivity) / capa.CoolDown);
	}
	
	public void display() {
		Debug.Log("Blorg1");
		if (isReady()) {
			Debug.Log("Blorg3");
			GUI.Label (new Rect (positionX, positionY, width, height), this.capa.GUIPicture);		
		} else {
			Debug.Log("Blorg2");
			GUI.Label (new Rect (positionX, positionY, width, height), this.capa.GUIPicture);		
			Color backup = GUI.color;
			Color overlay = new Color(0, 0, 0, 0.7f);
			GUI.color = overlay;
			//GUI.Label(new Rect(positionX, positionY, width * getPercentage(), height), "");
			
			var unicolor = new Texture2D(1, 1);
			unicolor.SetPixel(0,0, overlay);
			unicolor.wrapMode = TextureWrapMode.Repeat;
			unicolor.Apply();
			GUI.DrawTexture(new Rect(positionX, positionY, (int)(width * getPercentage() / 100.0), height), unicolor);
			GUI.color = backup;
		}
		
	}
	
	public GUISkill setLinkedCapa(BaseCapacity capa) {
		this.capa = capa;
		return this;
	}
	
}
