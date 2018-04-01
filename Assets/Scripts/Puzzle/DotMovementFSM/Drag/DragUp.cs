using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragUp : IDotState
{

    public void Action(Dot d)
    {
        if (d.GetUpDrag() && d.GetSelected())
        {
            Debug.Log("DRAG UP");
            d.UpAction();
        }
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        //if (!d.GetUpDrag()) next = d.snapState;
        //else if (!d.GetSelected()) next = d.checkState;

        if (!d.GetUpDrag())
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
