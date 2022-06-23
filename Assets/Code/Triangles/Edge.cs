using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Edge
{
    public Vector3 v1;
    public Vector3 v2;

    public bool ContainsVertex(Vector3 vert)
    {
        return v1 == vert || v2 == vert;
    }
}
