using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Idle : IDotState
{
    public void Action(Dot d)
    {
        //Debug.Log("IDLE");
        d.IdleAction();
    }

    public IDotState GetNextState(Dot d)
    {
        // Debug.Log("IDLE")
        IDotState next = d.idleState;
        if      (d.CheckForFalling())   next = d.fallState;
        else if (d.GetSelected())       next = d.selState;     
        return next;
    }
    
}
