using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Replay
{
    // List of <int, Command> which represents 
    public List<ReplayFrame> actions = new List<ReplayFrame>();

    // we will try fixedupdate for recording and playing BACK the replays.

    public void AddCommand(ICommand command)
    {
        actions.Add(new ReplayFrame(Match.currentFrame, command));
    }
}
