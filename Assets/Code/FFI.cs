using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FFI
{
    public static VectorFixed CrunchMovement(VectorFixed chosenPos, VectorFixed currentPos, UInt64 speed)
    {
        return crunch_movement(
            chosenPos.x, chosenPos.y, chosenPos.z,
            currentPos.x, currentPos.y, currentPos.z,
            speed
        );
    }

    /// <summary>
    /// Returns the closest of all units to some position.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="unitPositions"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static UInt64 ClosestMemberTo(VectorFixed position, VectorFixed[] unitPositions, UInt64 length)
    {
        return closest_member_to(position, unitPositions, length);
    }

    // note: if you do not pass the correct arguments, for instance 'speed' is neglected,
    //       there may not be a runtime error or compiler error.
    [DllImport("thystle_move")]
    static extern VectorFixed crunch_movement(UInt64 x, UInt64 y, UInt64 z, UInt64 fromX, UInt64 fromY, UInt64 fromZ, UInt64 speed);

    [DllImport("thystle_move")]
    static extern UInt64 closest_member_to(VectorFixed position, VectorFixed[] unit_positions, UInt64 length);
}
