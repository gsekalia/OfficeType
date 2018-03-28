using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragRight : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("DRAG RIGHT");
        d.RightAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        if (!d.GetRightDrag()) next = d.snapState;
        else if (!d.GetSelected()) next = d.checkState;

        return next;
    }
}
