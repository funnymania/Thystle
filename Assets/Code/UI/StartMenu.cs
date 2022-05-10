using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public int replayOrNot; // -1 = not, all else is the replay number to look up
    public ReplayPlayer replayer;
    public Match match;

    private void Awake()
    {
        if (replayOrNot == -1)
        {
            match.Play();
        }
        else
        {
            // look up and play replay by name.
            replayer.Play("" + replayOrNot);
        }
    }
}

// Ideal: ReplayPlayer instantiated upon option...
//        
