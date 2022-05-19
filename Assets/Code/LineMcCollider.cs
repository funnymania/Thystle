using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineMcCollider : MonoBehaviour
{
    public Line line;

    // editorscripting here...
}

[System.Serializable]
public struct Line
{
    public VectorFixed begin;
    public VectorFixed end;
}
