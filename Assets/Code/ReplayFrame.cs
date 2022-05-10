using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ReplayFrame
{
    public System.UInt64 frame;
    public ICommand command;

    public ReplayFrame(System.UInt64 frame, ICommand command)
    {
        this.frame = frame;
        this.command = command;
    }
}
