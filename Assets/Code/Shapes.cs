using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Shape
{
    VectorFixed begin;
    VectorFixed end;
    ulong ilk;
}

// the idea was that circles would be ALL circles, and lines would be ALL lines.
// circles and lines would be passed througgh staticShapes,
// and we would only be storing the INDICES here.
public struct Movement
{
    public VectorFixed begin;
    public VectorFixed end;
    public Circle[] circles;
    public Line[] lines;
    public ulong speed;
    // public ColliderRef[] collider; // used to index into the correct collider.
}

public struct ColliderRef
{
    public ColliderType colliderType;
    public int index;
}

public enum ColliderType
{
    Line,
    Circle
}
