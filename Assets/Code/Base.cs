using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Unit
{
    public VectorFixed spawnOffset = VectorFixed.zero;

    protected override void Start()
    {
        base.Start();
        name = "Base";
        isMovable = false;
    }
}
