using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WorldValues: MonoBehaviour
{
    [SerializeField]
    private int bitResolution = 16; // Private, but serialized. A trick to hide the variable in code, but expose in Inspector.
                                    // note: this value should only be a power of 2.

    public static int MIN_SUPPORTED_BIT_RES = 16;
    public const int FLOAT_BIT_RES = 24;
    public static int UNIT_SIZE = 1;

    private void OnValidate()
    {
        if (MIN_SUPPORTED_BIT_RES > 48)
        {
            MIN_SUPPORTED_BIT_RES = bitResolution;
            Debug.Log("In order to attempt, we would have to " +
                "\ndouble wrap within 2 gameobjects to scale from (this would give us 72-bit RANGE).");
        }
        else if (MIN_SUPPORTED_BIT_RES > FLOAT_BIT_RES)
        {
            Debug.Log("Scaling will occur to compensate. At numbers greater than 2^24bits expect deviation.");
            MIN_SUPPORTED_BIT_RES = bitResolution;
            UNIT_SIZE = (int)Mathf.Pow(2, MIN_SUPPORTED_BIT_RES - FLOAT_BIT_RES);
            transform.localScale = new Vector3(UNIT_SIZE, UNIT_SIZE, UNIT_SIZE);
        }
        else if (MIN_SUPPORTED_BIT_RES >= 16)
        {
            MIN_SUPPORTED_BIT_RES = bitResolution;
            UNIT_SIZE = (int)Mathf.Pow(2, MIN_SUPPORTED_BIT_RES - 16);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Debug.Log("We only suppoort 16-bit and up.");
        }
    }
}
