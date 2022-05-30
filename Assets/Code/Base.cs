using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Unit
{
    public Vector3 spawnOffset = Vector3.zero;

    protected override void Start()
    {
        base.Start();
        name = "Base";
        isMovable = false;
        spawnOffset = new Vector3(-3 * WorldValues.UNIT_SIZE, -2 * WorldValues.UNIT_SIZE, 0);
    }
}
