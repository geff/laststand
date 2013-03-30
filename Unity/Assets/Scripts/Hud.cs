using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour
{
	public const int capacityWidth = 50; // The width of the capacity button/texture2D to be displayed in the HUD	
	public void Start() {
		Debug.Log("HUD::Start()");
		int i = 0;
		foreach (BaseCapacity capa in GameSingleton.Instance.context.player.playerTank.GetComponents<BaseCapacity>()) {
			GUISkill gsk = new GUISkill();
			gsk.positionX = i * capacityWidth + elementsMargin;
			gsk.setLinkedCapa(capa);
			skills.Add(gsk);
			i++;
		}
	}

	private IList<GUISkill> skills = new List<GUISkill>();
	
	public void OnGUI () {
		PlayerData player = GameSingleton.Instance.context.player;
		// Cannot display HUD before player is at least in 'Playing' state
		if (player.currentState < PlayerState.Playing)
		{
			return;
		}
		
		if (null != GameSingleton.Instance.gameState && GameSingleton.Instance.gameState.currentPhase == MultiGameState.Phase.CountDown) {
			// TODO display thinngs
			var screen = new Rect(0,0,Screen.width, Screen.height);
			var screenCenterLabel = new Rect(Screen.width*(1/2.0f-1/6.0f), Screen.height*(1/2.0f-1/6.0f), Screen.width/3.0f, Screen.height/3.0f);
			Color overlay = new Color(0, 0, 0, 0.7f);// transparent black
			Hud.drawRectangle(overlay, screen);
			var style = new GUIStyle();
			style.fontSize = 350;
			style.normal.textColor = Color.white;
			Debug.Log ("hqaha:" + style.fontSize);
			GUI.Label(screenCenterLabel, GameSingleton.Instance.gameState.countDown, style);
		}
		
		
		// start debug code
		int i = 0;
//		skills = new List<GUISkill>();
		// the following code is as it is because of an unkown state when executing Start() method and no time for fixing that
		foreach (BaseCapacity capa in GameSingleton.Instance.context.player.playerTank.capacities.Values) {
			GUISkill gsk = new GUISkill();
			gsk.positionX = i * capacityWidth + elementsMargin;
			gsk.setLinkedCapa(capa);
			gsk.display();
//			skills.Add(gsk);
			i++;
		}
		
		int j = 0; // ourself
		foreach (PlayerData pdata in GameSingleton.Instance.context.playerList.Values) {// for all players connected
			if (pdata != null && pdata.playerTank != null && pdata.playerTank.Life > 0) {// if the player is alive
				j++;// count it in!
			}
		}
		dispAlivePlayers(j);
		// end debug code
//		foreach(GUISkill skill in skills) {
//			skill.display();
//		}
	}
	
	public int alivePlayersWidth = 60;
	public int alivePlayersHeight = 45;
	public int elementsMargin = 8; // in pixels
	
	private void dispAlivePlayers(int n_alive) {
		Color overlay = new Color(0, 0, 0, 0.7f);// transparent black
		var pos = new Rect(Screen.width - elementsMargin - alivePlayersWidth, elementsMargin, alivePlayersWidth, alivePlayersHeight);
		drawRectangle(overlay, pos);
		GUI.Label(pos, "  " + n_alive + "\n joueur(s)");
	}
	
	static public void drawRectangle(Color color, Rect r) {
		Color backup = GUI.color;
		GUI.color = color;
		var unicolor = new Texture2D(1, 1);
		unicolor.SetPixel(0,0, color);
		unicolor.wrapMode = TextureWrapMode.Repeat;
		unicolor.Apply();
		GUI.DrawTexture(r, unicolor);
		GUI.color = backup;
	}
}
