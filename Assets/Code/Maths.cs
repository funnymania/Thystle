using System.Collections;
using System.Collections.Generic;

public static class Maths
{
    public static System.UInt64 IntPow(System.UInt64 theBase, System.UInt64 power)
    {
        System.UInt64 result = 1;
        while (power > 0)
        {
            result *= theBase;
            power -= 1;
        }

        return result;
    }
}
