using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToIdle : IDotState
{
    public void Action(Dot d)
    {
        Debug.Log("SELECTED");
        d.IdleAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;
        if (!d.GetSelected()) next = d.idleState;
        return next;
    }
}
