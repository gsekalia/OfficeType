using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragDown : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("DRAG RIGHT");
        d.DownAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        //if (!d.GetDownDrag()) next = d.idleState;
        //else if (!d.GetSelected()) next = d.checkState;

        return next;
    }
}
