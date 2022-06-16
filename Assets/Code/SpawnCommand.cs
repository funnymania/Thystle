using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCommand : ICommand
{
    public long id { get; set; }
    public uint playerId { get; set; }

    public VectorFixed position;
    public GameObject newUnit;
    // public System.UInt32 playerId;

    public SpawnCommand(VectorFixed position, GameObject newUnit, System.UInt32 playerId, long id)
    {
        this.newUnit = newUnit;
        this.position = position;
        this.playerId = playerId;
        this.id = id;
    }

    public bool Execute()
    {
        Transform parent = GameObject.Find("Map").transform;

        // instantiate new Unit at position.
        GameObject newGo = GameObject.Instantiate(
            newUnit,
            position.AsUnityTransform(),
            Quaternion.identity,
            parent
        );

        Unit unit = newGo.GetComponent<Unit>();
        unit.playerId = playerId;
        unit.id = Match.nextUnitId;
        unit.truePosition = position;
        Match.nextUnitId += 1;
        Match.fieldedUnits.Add(unit.id, newGo);
        Match.staticUnits.Add(unit.id, newGo);
        if (unit.isMovable == false)
            Match.ConstrainNavMesh();
        return true;
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
