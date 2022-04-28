using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// note: ICommand might be unnecessary, this might be better off decoupled
//       in which case a ReplayFrame is <int, object>.
public class SpawnCommand : ICommand
{
    public VectorFixed position;
    public Unit newUnit;

    public SpawnCommand(VectorFixed position, Unit newUnit)
    {
        this.newUnit = newUnit;
        this.position = position;
    }

    public bool Execute()
    {
         
        return true;
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
