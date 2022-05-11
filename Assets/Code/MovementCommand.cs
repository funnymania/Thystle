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
    public long id { get; set; }

    public VectorFixed destination;
    public System.UInt32 leaderId;
    public System.UInt32[] memberIds;

    public MovementCommand(VectorFixed destination, System.UInt32 leader, System.UInt32[] members, long id)
    {
        this.destination = destination;
        this.leaderId = leader;
        memberIds = members;
        this.id = id;
        //for (var i = 0; i < members.Length; i += 1)
        //{
        //    this.memberIds[i] = members[i].GetComponent<Unit>().id;
        //}
    }

    public bool Execute()
    {
        // move leader until point.
        Unit leader = Match.fieldedUnits[leaderId].GetComponent<Unit>();
        VectorFixed moveTo = FFI.CrunchMovement(destination, VectorFixed.FromVector3(leader.transform.position), 2);
        // leader.transform.position = moveTo.AsUnityTransform();

        for (var i = 0; i < memberIds.Length; i += 1)
        {
            // bug: this moves all members of a group to leader's positiion.
            Match.fieldedUnits[memberIds[i]].transform.position = moveTo.AsUnityTransform();
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
