using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selected : IDotState
{

    public void Action(Dot d)
    {
        //Debug.Log("SELECTED");
        d.SelectedAction();
    }

    public IDotState GetNextState(Dot d)
    {
        IDotState next = this;

        //Figure out which way the players pulling more and go that way
        Vector2 dir = d.GetDir();

        if (!d.GetSelected())   next = d.idleState;
        else if (dir.x < -.1f)  next = d.leftState;
        else if (dir.x >  .1f)  next = d.rightState;
        else if (dir.y >  .1f)  next = d.upState;
        else if (dir.y < -.1f)  next = d.downState;

        return next;

    }
}

