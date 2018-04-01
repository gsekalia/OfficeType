using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragRight : IDotState
{
    public void Action(Dot d)
    {
        if (d.GetRightDrag() && d.GetSelected())
        {
            Debug.Log("DRAG RIGHT");
            d.RightAction();
        }
    }
    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        if (!d.GetRightDrag())
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
