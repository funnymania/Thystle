using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FFI
{
    // DEPRECATED.
    public static VectorFixed CrunchMovement(VectorFixed chosenPos, VectorFixed currentPos, UInt64 speed)
    {
        return crunch_movement(
            chosenPos.x, chosenPos.y, chosenPos.z,
            currentPos.x, currentPos.y, currentPos.z,
            speed
        );
    }

    // StaticShapes: All map walls, all non-moving things (everything undergoing a "movement" will need to be
    //       added to some sort of movingUnit list (containing indices of fieldedUnits), and all other units
    //       will also need to be in a list as well). Something to keep in mind is that these lists will need
    //       to then be updated whenever an object is removed from play, or spawned.
    public static (IntPtr, VectorFixed[]) MoveEverything(
        LineCollider[] lineCols, 
        CircleCollider[] circleCols, 
        Movement[] movements, 
        ulong lineColsLen, 
        ulong circleColsLen, 
        ulong movementsLen
    ) {
        var size = Marshal.SizeOf(typeof(VectorFixed));
        VectorFixed[] managedArray = new VectorFixed[movements.Length];

        IntPtr res = move_everything(
           lineCols,
           circleCols,
           movements,
           lineColsLen,
           circleColsLen,
           movementsLen
        );

        for (int i = 0; i < managedArray.Length; i++)
        {
            IntPtr ins = new IntPtr(res.ToInt64() + i * size);
            managedArray[i] = Marshal.PtrToStructure<VectorFixed>(ins);
        }

        return (res, managedArray);
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

    /// <summary>
    /// Returns the closest of all units to some position.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="unitPositions"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static Int64 SideOfLine(LineCollider line, CircleCollider point)
    {
        return side_of_line(line, point);
    }

    /// <summary>
    /// Frees memory used to hold calculated movement on the backend.
    /// Neglecting to call this each frame is a memory leak, and is fatal.
    /// </summary>
    /// <param name="movementRes"></param>
    public static void ReleaseMovementResults(IntPtr movementRes)
    {
        release_movement_results(movementRes);
    }

    public static VectorFixed[] SimpleResults(
        LineCollider[] lineCols, 
        CircleCollider[] circleCols, 
        Movement[] movements, 
        ulong lineColsLen, 
        ulong circleColsLen, 
        ulong movementsLen
    )
    {
        var size = Marshal.SizeOf(typeof(VectorFixed));
        VectorFixed[] managedArray = new VectorFixed[movements.Length];

        IntPtr res = simple_result(
           lineCols,
           circleCols,
           movements,
           lineColsLen,
           circleColsLen,
           movementsLen
        );

        for (int i = 0; i < managedArray.Length; i++)
        {
            IntPtr ins = new IntPtr(res.ToInt64() + i * size);
            managedArray[i] = Marshal.PtrToStructure<VectorFixed>(ins);
        }

        return managedArray;
    }

    [DllImport("thystle_move")]
    static extern IntPtr simple_result(LineCollider[] static_lines, CircleCollider[] static_circles, Movement[] movements, ulong static_lines_len, ulong static_circles_len, ulong movements_len);

    [DllImport("thystle_move")]
    static extern void release_movement_results(IntPtr movement_results);

    // note: if you do not pass the correct arguments, for instance 'speed' is neglected,
    //       there may not be a runtime error or compiler error.
    [DllImport("thystle_move")]
    static extern VectorFixed crunch_movement(UInt64 x, UInt64 y, UInt64 z, UInt64 fromX, UInt64 fromY, UInt64 fromZ, UInt64 speed);

    [DllImport("thystle_move")]
    static extern IntPtr move_everything(LineCollider[] static_lines, CircleCollider[] static_circles, Movement[] movements, ulong static_lines_len, ulong static_circles_len, ulong movements_len);

    [DllImport("thystle_move")]
    static extern UInt64 closest_member_to(VectorFixed position, VectorFixed[] unit_positions, UInt64 length);

    [DllImport("thystle_move")]
    static extern Int64 side_of_line(LineCollider line, CircleCollider circle);
}
