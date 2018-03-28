using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragUp : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("DRAG RIGHT");
        d.UpAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        //if (!d.GetUpDrag()) next = d.idleState;
        //else if (!d.GetSelected()) next = d.checkState;

        return next;
    }
}
