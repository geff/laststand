using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	
	public int maxWidth = 200;// pixels
	public int height = 40; // pixels
	
	public VehicleController vehicle;
	static private Color lowHPColor = new Color(1, 0, 0, 1);
	static private Color normalHPColor = new Color(0, 1, 0, 1);
	private Transform tr = null;
	// Use this for initialization
	void Start () {
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
		Vector3 pos = tr.localPosition;
		Rect r = new Rect(pos.y, pos.z, ((int)maxWidth * ratio), height); // TODO fine adjustment of halthbar positioning
		Hud.drawRectangle(c, r);
	}
}
