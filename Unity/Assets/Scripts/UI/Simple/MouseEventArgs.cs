using System;
using UnityEngine;

internal class MouseEventArgs : EventArgs
{
	public enum MouseEventType
	{
		Up,
		Down,
		Move,
		Enter,
		Leave
	}

	private readonly MouseEventType m_eventType;
	private readonly Vector2 m_mousePosition;

	public MouseEventArgs(MouseEventType eventType, Vector2 mousePosition)
	{
		this.m_eventType = eventType;
		this.m_mousePosition = mousePosition;
	}

	public MouseEventType EventType { get { return this.m_eventType; } }
	public Vector2 MousePosition { get { return this.m_mousePosition; } }
}

internal class MouseUpEventArgs : MouseEventArgs
{
	public MouseUpEventArgs(Vector2 mousePosition)
		: base(MouseEventType.Up, mousePosition)
	{

	}
}

internal class MouseDownEventArgs : MouseEventArgs
{
	public MouseDownEventArgs(Vector2 mousePosition)
		: base(MouseEventType.Down, mousePosition)
	{

	}
}

internal class MouseMoveEventArgs : MouseEventArgs
{
	public MouseMoveEventArgs(Vector2 mousePosition)
		: base(MouseEventType.Move, mousePosition)
	{

	}
}

internal class MouseEnterEventArgs : MouseEventArgs
{
	public MouseEnterEventArgs(Vector2 mousePosition)
		: base(MouseEventType.Enter, mousePosition)
	{

	}
}

internal class MouseLeaveEventArgs : MouseEventArgs
{
	public MouseLeaveEventArgs(Vector2 mousePosition)
		: base(MouseEventType.Leave, mousePosition)
	{

	}
}
