using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : IDotState
{

    public void Action(Dot d)
    {
        if (d.GetSelected())
        {
            Debug.Log("Right");
            d.RightAction();
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
            if (dir.x < -.1f) next = d.leftState;
        }
        return next;
    }
}
