using UnityEngine;
using System.Collections;

[AddComponentMenu("UI/Manager")]
public class UIManager : MonoBehaviour
{
	#region Singleton
	internal static UIManager Instance
	{
		get
		{
			if (UIManager.s_instance == null)
			{
				UIManager.s_instance = FindObjectOfType(typeof(UIManager)) as UIManager;
			}
			return UIManager.s_instance;
		}
	}
	private static UIManager s_instance = null;

	internal static bool Exists()
	{
		return UIManager.s_instance != null;
	}
	#endregion // Singleton

	internal UICamera uiCamera
	{
		get { return this.m_uiCamera; }
	}
	private UICamera m_uiCamera;

	internal void SetCamera(UICamera camera)
	{
		this.m_uiCamera = camera;
	}

	void Awake()
	{
		Debug.Log("UIManager.Awake()");
	}

	void Start()
	{
		Debug.Log("UIManager.Start()");
	}

	void OnDestroy()
	{
		Debug.Log("UIManager.OnDestroy()");

		// Free references
		this.m_uiCamera = null;
		UIManager.s_instance = null;
	}

	void Update()
	{
		PollInput();
		PollKeyboard();
		DispatchInput();
	}

	private void PollInput()
	{

	}

	private void PollKeyboard()
	{

	}

	private void DispatchInput()
	{
		
	}
}