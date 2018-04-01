using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : IDotState
{

    public void Action(Dot d)
    {
        if (d.GetSelected())
        {
            //Debug.Log("LEfT");
            d.LeftAction();
        }

    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;
        if (!d.GetSelected())
        {
            d.DetachFromAbove();
            d.DetachFromBelow();
            next = d.checkState;
        }
        else
        {
            Vector2 dir = d.GetDirX();
            if (dir.x > .1f) next = d.rightState;
        }
        return next;
    }
}
