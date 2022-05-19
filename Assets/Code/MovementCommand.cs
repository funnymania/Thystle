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
        GameObject leaderGo = Match.fieldedUnits[leaderId];

        Unit leader = leaderGo.GetComponent<Unit>();
        // VectorFixed moveTo = FFI.CrunchMovement(destination, VectorFixed.FromVector3(leader.transform.position), 2);

        // we can easily get Movement information here using fieldedUnits[memberIds[i]].
        List<Movement> movements = new List<Movement>();
        for (var i = 0; i < memberIds.Length; i += 1)
        {
            // unit now moving. 
            Match.staticUnits.Remove(memberIds[i]);

            Unit unit = Match.fieldedUnits[memberIds[i]].GetComponent<Unit>();
            var (lines, circles) = unit.GetColliders();

            Movement movement = new Movement();
            movement.begin = VectorFixed.FromVector3(Match.fieldedUnits[memberIds[i]].transform.position);
            movement.end = destination;
            movement.circles = circles;
            movement.lines = lines;
            movement.speed = unit.defaultSpeed;

            movements.Add(movement);
        }

        // perf: possibly too much memory churn
        List<Line> staticLines = new List<Line>();
        List<Circle> staticCircles = new List<Circle>();
        foreach (GameObject gameObject in Match.staticUnits.Values)
        {
            var (lines, circles) = gameObject.GetComponent<Unit>().GetColliders();
            staticLines.AddRange(lines);
            staticCircles.AddRange(circles);
        }

        VectorFixed[] moveTo = FFI.MoveEverything(
            staticLines.ToArray(),
            staticCircles.ToArray(),
            movements.ToArray(),
            (ulong)staticLines.Count,
            (ulong)staticCircles.Count,
            (ulong)movements.Count
        );

        for (var i = 0; i < memberIds.Length; i += 1)
        {
            Match.fieldedUnits[memberIds[i]].transform.position += moveTo[i].AsUnityTransform() - leader.transform.position; // potential solution
        }

        // when leader hits point, we are done.
        if (destination == moveTo[0])
        {
            // add all memberIds back to table.
            for (var i = 0; i < memberIds.Length; i += 1)
            {
                Match.staticUnits[memberIds[i]] = Match.fieldedUnits[memberIds[i]];
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
