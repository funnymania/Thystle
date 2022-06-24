using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// The circle represents offets from the actual entity's position. 
/// ex. for some Unit, begin: x: -1, y: 0, z: 0 means that the collider begins at 
///     one space to the left of the Unit's position.
/// </summary>
/// note: the circle we are using for collision is a PERFECT circle. However, 
///       for pathfinding we needed a set of vertices, and so we have a very aliased
///       circle used there. There should not be a determinism issue here.
/// note: CircleColliders are not being used for pathfinding yet. Currently we just
///       have pre-computed cirlces in CircleAtlas that take a radius (provided via
///       offset.
public class CircleMcCollider: MonoBehaviour
{
    // note: the radius set here will be multiplied by the scale factor on the base Unit.
    //       the result will be passed to collision detection, etc.
    public CircleOffset offset;

    private CircleCollider _circle;
    private CircleAliased _circleAliased;

    private void Awake()
    {
        // note: when scaling up, if CircleOffset.begin is not set to Vector3.zero (there is some
        //       positional offset), the adjustment of where this collider goes will be lossy.
        //       This is because we need to preserve integers, and so will round floats down.
        _circle.begin = VectorFixed.FromVector3(
            transform.parent.position + (offset.begin * transform.parent.localScale.x * WorldValues.UNIT_SIZE)
        );
        _circle.radius = offset.radius * (ulong)transform.parent.localScale.x;
        _circleAliased = generateAliasedCircle();
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

    public CircleAliased getAliased()
    {
        return _circleAliased;
    }

    private CircleAliased generateAliasedCircle()
    {
        CircleAliased aliased = new CircleAliased();
        aliased.vertices = CircleAtlas.getByRadius((int)_circle.radius).ToArray();

        return aliased;
    }
}

[System.Serializable]
public struct CircleAliased
{
    public Vector3[] vertices;
}

[System.Serializable]
public struct CircleOffset
{
    public Vector3 begin;
    public ulong radius;
}

/// <summary>
/// Passed to backend for collision detection.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct CircleCollider
{
    public VectorFixed begin;
    public ulong radius;
}
