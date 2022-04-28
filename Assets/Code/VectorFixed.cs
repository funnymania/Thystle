using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

//todo:  every character is some gameobject with a TRANSFORM used for collisions/etc and a 
//           TRANSFORM used for sprite rendering
[StructLayout(LayoutKind.Sequential)]
public struct VectorFixed: IEquatable<VectorFixed>
{
    public static VectorFixed zero = new VectorFixed(0, 0, 0);
    public UInt32 x, y, z;
    public VectorFixed(UInt32 x, UInt32 y, UInt32 z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Return VectorFixed from Vector3. Raises exception if Vector3 contains 
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
            throw new System.Exception("Tried to convert from float, only support ints for now");
        }

        return new VectorFixed(
            (UInt32)from.x,
            (UInt32)from.y,
            (UInt32)from.z
        );
    }

    /// <summary>
    /// Set VectorFixed to values of Vector3.
    /// </summary>
    /// <param name="unity"></param>
    public void SetToUnityTransform(Vector3 unity)
    {
        this.x = (UInt32)unity.x;
        this.y = (UInt32)unity.y;
        this.z = (UInt32)unity.z;
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
