using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnapBack : IDotState
{
    public void Action(Dot d)
    {
        Debug.Log("SNAPSTATE");
        d.SnapBackAction();
    }

    public IDotState GetNextState(Dot d)
    {

        // Debug.Log("IDLE")
        IDotState next = this;
       
        if (d.CheckIfAtStartPosition()) next = d.idleState;
        return next;
    }

}
