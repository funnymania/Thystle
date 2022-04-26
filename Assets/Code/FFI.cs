using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FFI
{
    public static VectorFixed CrunchMovement(VectorFixed chosenPos, VectorFixed currentPos, UInt32 speed)
    {
        return crunch_movement(
            chosenPos.x, chosenPos.y, chosenPos.z,
            currentPos.x, currentPos.y, currentPos.z,
            speed
        );
    }

    // note: if you do not pass the correct arguments, for instance 'speed' is neglected,
    //       there may not be a runtime error or compiler error.
    [DllImport("thystle_move")]
    static extern VectorFixed crunch_movement(UInt32 x, UInt32 y, UInt32 z, UInt32 fromX, UInt32 fromY, UInt32 fromZ, UInt32 speed);
}
