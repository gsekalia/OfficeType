using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragDown : IDotState
{

    public void Action(Dot d)
    {
        if (d.GetDownDrag() && d.GetSelected())
        {
            //Debug.Log("DRAG Down");
            d.DownAction();
        }
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        if (!d.GetDownDrag())
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
