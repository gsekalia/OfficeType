using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveUp : IDotState
{

    public void Action(Dot d)
    {
        if (d.GetSelected())
        {
            Debug.Log("MoveUp");
            d.UpAction();
        }
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
            if (dir.y < -.1f) next = d.downState;
        }
        return next;
    }

}
