using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveDown : IDotState
{

    public void Action(Dot d)
    {
        Debug.Log("MOVE DOWN");
        d.DownAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;
        if (!d.GetSelected())
        {
            d.DetachFromLeft();
            d.DetachFromRight();
            next = d.checkState;
        }
        else
        {
            Vector2 dir = d.GetDirY();
            if (dir.y > .1f) next = d.upState;
        }
        return next;
    }
}
