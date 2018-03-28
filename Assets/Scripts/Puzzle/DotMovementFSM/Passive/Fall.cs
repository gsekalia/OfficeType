using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : IDotState
{

    public void Action(Dot d)
    {
        // Debug.Log("LEfT");
        // d.LeftAction();
        d.FallAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;
        if(!d.CheckForFalling()) next = d.checkState;
        //if (!d.CheckForFalling()) next = d.idleState;
       
        return next;
    }
}
