using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour
{
	public Texture2D skillPicture;

	// Use this for initialization
	void Start ()
	{
	
	}

		void OnGUI ()
		{
			GUI.Label (new Rect (0,0,100,50), skillPicture);
		}
}
