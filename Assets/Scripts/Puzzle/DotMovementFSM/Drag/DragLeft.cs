using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragLeft : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("DRAG LEFT");
        //d.DragLeftAction();
        d.LeftAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        if(!d.GetLeftDrag()) next = d.idleState;
        else if (!d.GetSelected()) next = d.checkState;

        return next;
    }
}
