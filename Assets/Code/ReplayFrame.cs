using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ReplayFrame
{
    public System.Int64 frame;
    public ICommand command;

    public ReplayFrame(System.Int64 frame, ICommand command)
    {
        this.frame = frame;
        this.command = command;
    }
}
