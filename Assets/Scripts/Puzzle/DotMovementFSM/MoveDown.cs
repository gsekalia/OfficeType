using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveDown : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("DOWN");
        d.DownAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;
        //if (!d.GetSelected()) next = d.idleState;
        if (!d.GetSelected()) next = d.checkState;
        return next;
    }
}
