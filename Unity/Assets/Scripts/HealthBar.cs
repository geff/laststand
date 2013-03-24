using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	public const int maxWidth = 40;// pixels
	public const int height = 6; // pixels
	public const float offsetX = -0.028f, offsetY = -0.09f;
	
	public VehicleController vehicle;
	static private Color lowHPColor = new Color(1, 0, 0, 1);
	static private Color normalHPColor = new Color(0, 1, 0, 1);
	private Transform tr = null;
	// Use this for initialization
	void Start () {
		vehicle = GetComponent<VehicleController>();
		tr = vehicle.transform; // not to do it inside the main ONGUI loop
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnGUI () {
		float ratio = vehicle.Life / ((float)vehicle.MaxLife);
		Color c;
		if (ratio <= 0.25f) {
			c= lowHPColor;
		} else {
			c = normalHPColor;
		}
		var trpos = tr.position;
//		Debug.Log(trpos);
//		Debug.Log(Camera.mainCamera.rect);
		Vector3 pos = Camera.mainCamera.WorldToViewportPoint(trpos);
		if (pos.x < 0 || pos.y < 0 || pos.z < 0) {// if the thing is not in the screen, do not display it
			return;
		}
//		Debug.Log(pos);
		pos.x += offsetX;
		pos.y += offsetY	;
		
		Rect r = new Rect(pos.x * Screen.width, pos.y * Screen.height, ((int)maxWidth * ratio), height); // TODO fine adjustment of halthbar positioning
		Hud.drawRectangle(c, r);
	}
}
