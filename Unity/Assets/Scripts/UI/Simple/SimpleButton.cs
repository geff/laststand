using System;
using UnityEngine;

public class SimpleButton : MonoBehaviour
{
	public Vector2 initialScreenPosition;
	public Texture2D defaultTexture;
	public Texture2D overTexture;
	public Texture2D pressedTexture;

	public enum TextureAnchor
	{
		TopLeft = 0,
		TopRight,
		BottomLeft,
		BottomRight,
		MiddleCenter
	}
	public TextureAnchor anchor;

	internal enum ButtonState
	{
		Default = 0,
		Over,
		Pressed
	}
	internal ButtonState buttonState;

	internal TouchableRectangle Rectangle
	{
		get;
		private set;
	}

    internal event EventHandler<EventArgs> ButtonClicked;

	void Awake()
	{
		float xOffset, yOffset;
		float width = this.defaultTexture.width;
		float height = this.defaultTexture.height;
		switch (this.anchor)
		{
		default:
		case TextureAnchor.TopLeft:
			xOffset = 0.0f;
			yOffset = 0.0f;
			break;

		case TextureAnchor.TopRight:
			xOffset = this.defaultTexture.width;
			yOffset = 0.0f;
			break;

		case TextureAnchor.BottomLeft:
			xOffset = 0.0f;
			yOffset = this.defaultTexture.height;
			break;

		case TextureAnchor.BottomRight:
			xOffset = this.defaultTexture.width;
			yOffset = this.defaultTexture.height;
			break;

		case TextureAnchor.MiddleCenter:
			xOffset = 0.5f*this.defaultTexture.width;
			yOffset = 0.5f*this.defaultTexture.height;
			break;
		}

		// Create a new touchable rectangle
		this.Rectangle = new TouchableRectangle();
		this.Rectangle.rect = new Rect(
			this.initialScreenPosition.x - xOffset,
			this.initialScreenPosition.y - yOffset,
			width, height);

		RegisterEvents();
	}

	void OnDestroy()
	{
		UnRegisterEvents();
		this.Rectangle = null;
	}

	void OnGUI()
	{
		switch (this.buttonState)
		{
		default:
		case ButtonState.Default:
			GUI.DrawTexture(this.Rectangle.rect, this.defaultTexture);
			break;

		case ButtonState.Over:
			GUI.DrawTexture(this.Rectangle.rect, this.overTexture);
			break;

		case ButtonState.Pressed:
			GUI.DrawTexture(this.Rectangle.rect, this.pressedTexture);
			break;
		}
	}

	protected void RegisterEvents()
	{
		this.Rectangle.MouseDown += OnMouseDown;
		this.Rectangle.MouseUp += OnMouseUp;
		this.Rectangle.MouseEnter += OnMouseEnter;
		this.Rectangle.MouseLeave += OnMouseLeave;
	}

	protected void UnRegisterEvents()
	{
		this.Rectangle.MouseDown -= OnMouseDown;
		this.Rectangle.MouseUp -= OnMouseUp;
		this.Rectangle.MouseEnter -= OnMouseEnter;
		this.Rectangle.MouseLeave -= OnMouseLeave;
	}

	internal void OnMouseDown(object sender, MouseDownEventArgs args)
	{
		this.buttonState = ButtonState.Pressed;
	}

	internal void OnMouseUp(object sender, MouseUpEventArgs args)
	{
//		this.buttonState = ButtonState.Over;
		EventArgs.Empty.Raise(this, ref ButtonClicked);
	}

	internal void OnMouseEnter(object sender, MouseEnterEventArgs args)
	{
		this.buttonState = ButtonState.Over;
	}

	internal void OnMouseLeave(object sender, MouseLeaveEventArgs args)
	{
		this.buttonState = ButtonState.Default;
	}
}

