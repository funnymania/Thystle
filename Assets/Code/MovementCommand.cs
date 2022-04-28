using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Movement
// Destination Position
// Leader (closest in some selection of GameObjects to the Target position)
//          ClosestTo() will need to be calculated on backend.
// SwarmMembers 

// todo: will need to support movement of members with various speeds (for now we just use leader's)
public struct MovementCommand : ICommand
{
    public VectorFixed destination;
    public GameObject leader;
    public GameObject[] members;

    public MovementCommand(VectorFixed destination, GameObject leader, GameObject[] members)
    {
        this.destination = destination;
        this.leader = leader;
        this.members = members;
    }

    public bool Execute()
    {
        // move leader until point.
        VectorFixed moveTo = FFI.CrunchMovement(destination, VectorFixed.FromVector3(leader.transform.position), 2);
        // leader.transform.position = moveTo.AsUnityTransform();

        for (var i = 0; i < members.Length; i += 1)
        {
            members[i].transform.position = moveTo.AsUnityTransform();
        }

        // when leader hits point, we are done.
        return destination == moveTo;
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
