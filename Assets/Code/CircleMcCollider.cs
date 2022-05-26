using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// The circle represents offets from the actual entity's position. 
/// ex. for some Unit, begin: x: -1, y: 0, z: 0 means that the collider begins at 
///     one space to the left of the Unit's position.
/// </summary>
public class CircleMcCollider: MonoBehaviour
{
    public CircleOffset offset;

    private CircleCollider _circle;

    private void Awake()
    {
        _circle.begin = VectorFixed.FromVector3(transform.parent.position + offset.begin);
        _circle.radius = offset.radius;
    }
    
    /// <summary>
    /// Returns collider in world space at point of calling.
    /// </summary>
    /// <returns></returns>
    public CircleCollider getCollider()
    {
        _circle.begin = VectorFixed.FromVector3(transform.parent.position + offset.begin);
        _circle.radius = offset.radius;
        return _circle;
    }
}

[System.Serializable]
public struct CircleOffset
{
    public Vector3 begin;
    public ulong radius;
}

[StructLayout(LayoutKind.Sequential)]
public struct CircleCollider
{
    public VectorFixed begin;
    public ulong radius;
}
