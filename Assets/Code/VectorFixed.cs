using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Acceptable math is the following:
///      Addition (but check overflows)
///      Subtraction (but check overflows)
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[Serializable]
public struct VectorFixed: IEquatable<VectorFixed>
{
    public static VectorFixed zero = new VectorFixed(0, 0, 0);
    public UInt64 x, y, z;
    public VectorFixed(UInt64 x, UInt64 y, UInt64 z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Return VectorFixed from Vector3. Raises exception if Vector3 contains non-integers.
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public static VectorFixed FromVector3(Vector3 from)
    {
        if (from.x != (float)Math.Truncate(from.x)
            || (from.y) != (float)Math.Truncate(from.y)
            || (from.z) != (float)Math.Truncate(from.z)
        )
        {
            // testing: just ignoring for now.
            // throw new System.Exception("Tried to convert from float, only support ints for now");
        }

        return new VectorFixed(
            (UInt64)from.x,
            (UInt64)from.y,
            (UInt64)from.z
        );
    }

    /// <summary>
    /// Does NOT guard against overflow. Do not use unless you KNOW there can be no overflow.
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static VectorFixed AddUnsafe(VectorFixed one, VectorFixed other)
    {
        return new VectorFixed(
            one.x + other.x,
            one.y + other.y,
            one.z + other.z
        );
    }

    /// <summary>
    /// Does NOT guard against overflow. Do not use unless you KNOW there can be no overflow.
    /// </summary>
    /// <param name="one"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public VectorFixed AddV3(Vector3 other)
    {
        return new VectorFixed(
            (ulong)(this.x + other.x),
            (ulong)(this.y + other.y),
            (ulong)(this.z + other.z)
        );
    }

    public static VectorFixed TruncateToVectorFixed(Vector3 toCull)
    {
        return new VectorFixed(
            (ulong)Math.Truncate(toCull.x),
            (ulong)Math.Truncate(toCull.y),
            (ulong)Math.Truncate(toCull.z)
        );
    }

    /// <summary>
    /// Set VectorFixed to values of Vector3.
    /// </summary>
    /// <param name="unity"></param>
    public void SetToUnityTransform(Vector3 unity)
    {
        this.x = (UInt64)unity.x;
        this.y = (UInt64)unity.y;
        this.z = (UInt64)unity.z;
    }

    /// <summary>
    /// Return a Vector3 from this VectorFixed.
    /// </summary>
    /// <returns></returns>
    public Vector3 AsUnityTransform()
    {
        return new Vector3(
            this.x,
            this.y,
            this.z
        );
    }

    /// <summary>
    /// Is this equal to some Vector3?
    /// </summary>
    /// <param name="unityVec"></param>
    /// <returns></returns>
    public bool EqualToUnity(Vector3 unityVec)
    {
        return this.x == unityVec.x
            && this.y == unityVec.y
            && this.z == unityVec.z
        ;
    }

    public override string ToString()
    {
        return "" + x + " " + y + " " + z;
    }

    public override int GetHashCode() => (x, y, z).GetHashCode();

    public override bool Equals(object obj) => this.Equals(obj);

    public bool Equals(VectorFixed other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public static bool operator ==(VectorFixed lhs, VectorFixed rhs) => lhs.Equals(rhs);

    public static bool operator !=(VectorFixed lhs, VectorFixed rhs) => !(lhs == rhs);
}
