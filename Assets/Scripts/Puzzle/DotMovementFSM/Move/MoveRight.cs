using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("Right");
        d.RightAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;
        //if (!d.GetSelected()) next = d.idleState;
        if (!d.GetSelected()) next = d.checkState;
        return next;
    }
}
