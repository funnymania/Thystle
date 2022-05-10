using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created to signify the end of a match. 
// Created on only one player remaining, due to all others
//      disconnecting, retreating, or losing.
public class EndCommand : ICommand
{
    public Func<bool> action;
    public long id { get; set; }

    public EndCommand(Func<bool> action)
    {
        this.action = action;
    }

    public bool Execute()
    {
        return (bool)action?.Invoke();
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
