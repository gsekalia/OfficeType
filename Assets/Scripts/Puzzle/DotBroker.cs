using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DotBroker : MonoBehaviour
{
    List<DotCommand> dotCommands = new List<DotCommand>();

    public void AddObject(Dot dot)
    {
        DotCommand cmd = new DotCommand(dot);
        dotCommands.Add(cmd);
    }
    public void ProcessCommands( )
    {
        while(dotCommands.Capacity > 0)
        {
            DotCommand cmd = dotCommands[0];
            dotCommands.RemoveAt(0);
            cmd.Execute();
        }

    }

}