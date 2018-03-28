using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CheckMatch : IDotState
{
    public void Action(Dot d)
    {
 
        d.CheckAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = d.idleState;
        if(!d.CheckForFoundMatch()) next = d.snapState;
        return next;
    }

}