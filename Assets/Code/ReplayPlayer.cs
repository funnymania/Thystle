using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : MonoBehaviour
{
    public Replay replay;
    public bool play;

    private System.UInt64 frameCount;

    void FixedUpdate()
    {
        if (play)
        {

            frameCount += 1;
        }        
    }
}
