using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMoveCommand : ICommand
{
    public long id { get; set; }

    // public VectorFixed destination;
    public List<VectorFixed> path;
    public System.UInt32 leaderId;
    public System.UInt32[] memberIds;
    public int pointInPath;

    public PathMoveCommand(VectorFixed destination, System.UInt32 leader, System.UInt32[] members, long id)
    {
        leaderId = leader; 
        memberIds = members;
        this.id = id;

        path = new List<VectorFixed>();
        List<Vector3> v3Path = NavMesh.ComputePath(
            Match.fieldedUnits[leaderId].GetComponent<Unit>().truePosition,
            destination
        );

        foreach (Vector3 v3 in v3Path)
        {
            path.Add(VectorFixed.FromVector3(v3));
            Debug.Log("Path: " + v3);
        }

        // note: replace last Vector3 in the path with the destination. 
        //if (path.Count > 0)
        //{
        //    path.RemoveAt(path.Count - 1);
        //}

        path.Add(destination);

        pointInPath = 0;
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
            // movement.begin = VectorFixed.FromVector3(Match.fieldedUnits[memberIds[i]].transform.position);
            movement.begin = unit.truePosition;
            movement.end = path[pointInPath];
            movement.circles = circles;
            movement.lines = lines;
            movement.circlesLen = (ulong)circles.Length;
            movement.linesLen = (ulong)lines.Length;
            movement.speed = unit.defaultSpeed;

            movements.Add(movement);
        }

        // perf: possibly too much memory churn
        List<LineCollider> staticLines = new List<LineCollider>();
        List<CircleCollider> staticCircles = new List<CircleCollider>();

        // bug: should be bases in staticCircles
        foreach (GameObject gameObject in Match.staticUnits.Values)
        {
            var (lines, circles) = gameObject.GetComponent<Unit>().GetColliders();
            staticLines.AddRange(lines);
            staticCircles.AddRange(circles);
        }

        // idea: what if there is another entity that needs to move past?
        //       Maybe the answer is to jump over them? just like if it was a wall
        //       the answer would be to climb it ofc
        // note: moveTo is a copy of the data passed from backend. 
        // perf: memory inefficient...
        (System.IntPtr releaseMe, VectorFixed[] moveTo) = FFI.MoveEverything(
            staticLines.ToArray(),
            staticCircles.ToArray(),
            movements.ToArray(),
            (ulong)staticLines.Count,
            (ulong)staticCircles.Count,
            (ulong)movements.Count
        );

        for (var i = 0; i < memberIds.Length; i += 1)
        {
            Match.fieldedUnits[memberIds[i]].GetComponent<Unit>().velocity =
                moveTo[i].AsUnityTransform() - Match.fieldedUnits[memberIds[i]].transform.position;
            Match.fieldedUnits[memberIds[i]].transform.position = moveTo[i].AsUnityTransform();
            Match.fieldedUnits[memberIds[i]].GetComponent<Unit>().truePosition = moveTo[i];
        }

        // note: right now, we only conclude the movement when the leader
        //       reaches their destination. Even if the other objects haven't made it, or have bumped into
        //       a collider. This may be fine for now, but you may WANT the Units lagging behind to "catch up"
        //       and conclude right next to the leader. Right now, this does not happen.
        //       In a way, it might be better to not conclude the MovementCommand AT ALL. But this is a topic for
        //       later review...
        // when leader hits point, we are done.
        // if leader's starting position is returned back, they have collided, and we need to return true.
        // else we are presumably still in motion.
        if (path[pointInPath] == moveTo[0]) // We have completed this part of the path.
        {
            // path officially complete, add all memberIds back to table.
            if (pointInPath == path.Count - 1)
            {
                for (var i = 0; i < memberIds.Length; i += 1)
                {
                    Match.fieldedUnits[memberIds[i]].GetComponent<Unit>().velocity = Vector2.zero;
                    Match.staticUnits[memberIds[i]] = Match.fieldedUnits[memberIds[i]];
                }

                FFI.ReleaseMovementResults(releaseMe);
                return true;
            }

            // path has not finished.
            pointInPath += 1;
            FFI.ReleaseMovementResults(releaseMe);
            return false;
        }
        else if (movements[0].begin == moveTo[0]) // We have collided with something 
        {
            // add all memberIds back to table.
            for (var i = 0; i < memberIds.Length; i += 1)
            {
                Match.fieldedUnits[memberIds[i]].GetComponent<Unit>().velocity = Vector2.zero;
                Match.staticUnits[memberIds[i]] = Match.fieldedUnits[memberIds[i]];
            }

            FFI.ReleaseMovementResults(releaseMe);
            return true;
        }
        else
        {
            FFI.ReleaseMovementResults(releaseMe);
            return false;
        }
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
