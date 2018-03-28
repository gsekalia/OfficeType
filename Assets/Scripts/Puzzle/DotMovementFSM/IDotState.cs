using System.Collections;
using System.Collections.Generic;
//using UnityEngine;

public interface IDotState
{
    void Action(Dot d);
    IDotState GetNextState(Dot d);

}
