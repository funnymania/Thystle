using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMcCollider: MonoBehaviour
{
    public Circle circle;

    // editor scripting here....
}

[System.Serializable]
public struct Circle
{
    public VectorFixed begin;
    public ulong radius;
}
