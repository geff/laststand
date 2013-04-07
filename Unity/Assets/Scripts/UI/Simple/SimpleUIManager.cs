using System.Collections.Generic;
using UnityEngine;

public class SimpleUIManager : MonoBehaviour
{

	private IList<TouchableRectangle> m_rectangles;

	// Use this for initialization
	void Start()
	{
		SimpleButton[] buttons = (SimpleButton[]) FindObjectsOfType(typeof(SimpleButton));
		this.m_rectangles = new List<TouchableRectangle>(buttons.Length);
		foreach(var button in buttons)
		{
			this.m_rectangles.Add(button.Rectangle);
		}
	}

	void OnDestroy()
	{
		this.m_rectangles = null;
	}

	// Update is called once per frame
	void Update()
	{
		bool mouseButton0 = Input.GetMouseButton(0);
		Vector2 mousePosition = Input.mousePosition;
		mousePosition.y = Screen.height - mousePosition.y;

		foreach(var rectangle in this.m_rectangles)
		{
			rectangle.UpdateEvents(mousePosition, mouseButton0);
		}
	}

	void OnGUI()
	{
		if (Debug.isDebugBuild)
		{
			Vector2 mousePosition = Input.mousePosition;
			GUILayout.Label(string.Format("mouse x:{0}, y:{1}", mousePosition.x, mousePosition.y));
		}
	}
}

