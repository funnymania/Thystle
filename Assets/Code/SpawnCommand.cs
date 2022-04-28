using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// note: ICommand might be unnecessary, this might be better off decoupled
//       in which case a ReplayFrame is <int, object>.
public class SpawnCommand : ICommand
{
    public VectorFixed position;
    public GameObject newUnit;

    public SpawnCommand(VectorFixed position, GameObject newUnit)
    {
        this.newUnit = newUnit;
        this.position = position;
    }

    public bool Execute(System.UInt32 playerId)
    {
        Transform parent = GameObject.Find("Match").transform;

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
        Match.fieldedUnits.Add(unit.id, newGo);
        return true;
    }

    public void Record()
    {
        // Add to Match's replay data.
        Match.replay.AddCommand(this);
    }
}
