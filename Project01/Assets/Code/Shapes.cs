using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct Shape
{
    VectorFixed begin;
    VectorFixed end;
    ulong ilk;
}

// perf: wanted to store indexes here instead of pointers, so that the entirety of data
//       could be handled on the stack. As it is here, we have pointers to Circles and Lines
//       on the heap for each Movement. 
[StructLayout(LayoutKind.Sequential)]
public struct Movement
{
    public VectorFixed begin; // this corresponds to the gameObject, the entity's starting position.
    public VectorFixed end;
    public CircleCollider[] circles; // the Vectors here correspond with the OFFSET from the entity's origin.
    public LineCollider[] lines;
    public ulong circlesLen;
    public ulong linesLen;
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
