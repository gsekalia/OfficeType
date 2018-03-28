using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DotCommand : MonoBehaviour
{
    Dot dot;

    public DotCommand(Dot d)
    {
        dot = d;
    }
    public void Execute()
    {
        dot.Kill();
    }

    
}
