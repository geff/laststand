using System;
using UnityEngine;

internal class TouchableRectangle
{
    /// <summary>
    /// Rectangle définissant la zone d'interaction à l'écran
    /// </summary>
    public Rect rect;
    /// <summary>
    /// Le pointeur est dans le rectangle
    /// </summary>
    public bool isIn = false;
    /// <summary>
    /// Le boutton de la souris est pressé
    /// </summary>
    public bool isTouchedDown = false;

    public event EventHandler<MouseDownEventArgs> MouseDown;
    public event EventHandler<MouseUpEventArgs> MouseUp;
    public event EventHandler<MouseMoveEventArgs> MouseMove;
    public event EventHandler<MouseEnterEventArgs> MouseEnter;
    public event EventHandler<MouseLeaveEventArgs> MouseLeave;


    /// <summary>
    /// Méthode appelée par InputController.Update() afin de mettre à jour les évènements
    /// </summary>
    /// <param name="mousePosition"></param>
    public void UpdateEvents(Vector2 mousePosition, bool mouseButton0)
    {
        bool newTouchedDown = false;
        bool newTouchedUp = false;
        bool newIsIn = false;
        bool newIsOut = false;

        //---> Test la présence du pointeur dans le rectangle
        if (this.rect.Contains(mousePosition))
        {
            //---> Le pointeur entre dans le rectangle
            if (!this.isIn)
            {
                this.isIn = true;
                newIsIn = true;
            }
        }
        else
        {
            //---> Le pointeur sort du rectangle
            if (this.isIn)
            {
                this.isIn = false;
                newIsOut = true;
            }
        }

        if (this.isIn)
        {
			//---> Pression du bouton de la souris dans le rectangle
			if (mouseButton0 && !this.isTouchedDown)
            {
				newTouchedDown = true;
            }
			//---> Relâchement du bouton de la souris dans le rectangle
			else if (!mouseButton0 && this.isTouchedDown)
            {
				newTouchedUp = true;
            }
        }

		// Event handling
        if (newTouchedDown)
        {
			OnMouseDown(new MouseDownEventArgs(mousePosition));
            this.isTouchedDown = true;
        }
        if (newTouchedUp)
        {
            OnMouseUp(new MouseUpEventArgs(mousePosition));
            this.isTouchedDown = false;
        }
        if (newIsIn)
        {
            OnMouseEnter(new MouseEnterEventArgs(mousePosition));
        }
        if (newIsOut)
        {
            OnMouseLeave(new MouseLeaveEventArgs(mousePosition));
        }
        if (this.isIn && this.isTouchedDown)
        {
            OnMouseMove(new MouseMoveEventArgs(mousePosition));
        }
    }

	private void OnMouseDown(MouseDownEventArgs e)
	{
		e.Raise(this, ref MouseDown);
	}

	private void OnMouseUp(MouseUpEventArgs e)
	{
		e.Raise(this, ref MouseUp);
	}

	private void OnMouseMove(MouseMoveEventArgs e)
	{
		e.Raise(this, ref MouseMove);
	}

	private void OnMouseEnter(MouseEnterEventArgs e)
	{
		e.Raise(this, ref MouseEnter);
	}

	private void OnMouseLeave(MouseLeaveEventArgs e)
	{
		e.Raise(this, ref MouseLeave);
	}
}
