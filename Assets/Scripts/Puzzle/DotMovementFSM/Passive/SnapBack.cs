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
        d.NullOutPrevDot();
        d.NullOutMyDot();

        IDotState next = this;
       
        if (d.CheckIfAtStartPosition()) next = d.idleState;
        return next;
    }

}
