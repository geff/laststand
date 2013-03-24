using UnityEngine;
using System.Collections;

public class GUISkill : MonoBehaviour {

	public int positionX = 10;
	public int positionY = 10;
	public int height = 100;
	public int width = 100;
	
	private BaseCapacity capa;
	
	private bool isReady() {
		if (1.0f <= getPercentage()) {
			return true;
		}
		return false;
	}
	
	private float getPercentage() { 
//		Debug.Log("LastActivity::" + capa.LastActivity.ToString());
		float result = Mathf.Clamp((Time.time - capa.LastActivity) / capa.CoolDown, 0.0f, 1.0f);
//		Debug.Log("GUISkill::getPercentage()=" + result);
		return result;
	}
	
	public void display() {
		if (isReady()) {
//			Debug.Log("FFFFFFFF");
			GUI.Label (new Rect (positionX, positionY, width, height), this.capa.GUIPicture);		
		} else {
//			Debug.Log("ABCABCABC");
			GUI.Label (new Rect (positionX, positionY, width, height), this.capa.GUIPicture);		
			Color backup = GUI.color;
			Color overlay = new Color(0, 0, 0, 0.7f);
			GUI.color = overlay;
			//GUI.Label(new Rect(positionX, positionY, width * getPercentage(), height), "");
			
			var unicolor = new Texture2D(1, 1);
			unicolor.SetPixel(0,0, overlay);
			unicolor.wrapMode = TextureWrapMode.Repeat;
			unicolor.Apply();
			int result = (int)Mathf.Clamp(width * (1.0f - getPercentage()), 0, width-7);
//			Debug.Log("result width=" + result);
			GUI.DrawTexture(new Rect(positionX, positionY+4, result, height-8), unicolor);
			GUI.color = backup;
		}
		
	}
	
	public GUISkill setLinkedCapa(BaseCapacity capa) {
		this.capa = capa;
		return this;
	}
	
}
