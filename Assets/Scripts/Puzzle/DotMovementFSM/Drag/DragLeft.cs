using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragLeft : IDotState
{

    public void Action(Dot d)
    {
        if (d.GetLeftDrag() && d.GetSelected())
        {
            Debug.Log("DRAG LEFT");
            d.LeftAction();
        }
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        if (!d.GetLeftDrag())
        {
            d.DetachFromPrev();
            d.SetDeselected();

            next = d.snapState;
        }
        else if (!d.GetSelected())
        {
            d.DetachFromPrev();
            next = d.checkState;
        }
        return next;
    }
}
