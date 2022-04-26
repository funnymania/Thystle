using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    void Start()
    {
        GameObject match = GameObject.Find("Match");
        match.transform.localScale = new Vector3(WorldValues.UNIT_SIZE, WorldValues.UNIT_SIZE, WorldValues.UNIT_SIZE);
        GameObject cam = GameObject.Find("Main Camera");
        cam.transform.position = new Vector3(0, 0, cam.transform.position.z * WorldValues.UNIT_SIZE);
    }
}
