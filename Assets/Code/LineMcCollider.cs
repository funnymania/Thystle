using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// 'line' represents offets from the actual entity's position. 
/// ex. for some Unit, begin: x: -1, y: 0, z: 0 means that the line begins at 
///     one space to the left of the Unit's position.
/// </summary>
public class LineMcCollider : MonoBehaviour
{
    private LineOffset _offset;

    // todo: custom drawer for LineOffset...
    public LineOffset offset;

    private LineCollider _line;

    private void Awake()
    {
        _line.begin = VectorFixed.FromVector3(transform.parent.position + offset.begin);
        _line.end = VectorFixed.FromVector3(transform.parent.position + offset.end);
    }

    /// <summary>
    /// Returns collider in world space at point of calling.
    /// </summary>
    /// <returns></returns>
    public LineCollider getCollider()
    {
        _line.begin = VectorFixed.FromVector3(transform.parent.position + offset.begin);
        _line.end = VectorFixed.FromVector3(transform.parent.position + offset.end);
        return _line;
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct LineCollider
{
    public VectorFixed begin;
    public VectorFixed end;
}

[System.Serializable]
public class LineOffset
{
    public Vector3 begin;
    public Vector3 end;
}
