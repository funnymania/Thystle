using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CircleAtlas
{
    private static List<List<Vector3>> _circleAtlas = new List<List<Vector3>>();

    public static void Init()
    {
        List<Vector3> zero = new List<Vector3>();
        _circleAtlas.Add(zero);

        // Like a diamond.
        List<Vector3> one = new List<Vector3>();
        one.Add(new Vector3(0, 1, 0));
        one.Add(new Vector3(1, 0, 0));
        one.Add(new Vector3(0, -1, 0));
        one.Add(new Vector3(-1, 0, 0));
        _circleAtlas.Add(one);

        // Somewhat like a ball.
        List<Vector3> two = new List<Vector3>(); 
        two.Add(new Vector3(0, 2, 0));
        two.Add(new Vector3(1, 2, 0));
        two.Add(new Vector3(2, 1, 0));
        two.Add(new Vector3(2, 0, 0));
        two.Add(new Vector3(2, -1, 0));
        two.Add(new Vector3(1, -2, 0));
        two.Add(new Vector3(0, -2, 0));
        two.Add(new Vector3(-1, -2, 0));
        two.Add(new Vector3(-2, -1, 0));
        two.Add(new Vector3(-2, 0, 0));
        two.Add(new Vector3(-2, 1, 0));
        two.Add(new Vector3(-1, 2, 0));
        _circleAtlas.Add(two);

        // Like a diamond.
        List<Vector3> three = new List<Vector3>();
        three.Add(new Vector3(0, 3, 0));
        three.Add(new Vector3(3, 0, 0));
        three.Add(new Vector3(0, -3, 0));
        three.Add(new Vector3(-3, 0, 0));
        _circleAtlas.Add(three);

        // Like a diamond.
        List<Vector3> four = new List<Vector3>();
        four.Add(new Vector3(0, 4, 0));
        four.Add(new Vector3(4, 0, 0));
        four.Add(new Vector3(0, -4, 0));
        four.Add(new Vector3(-4, 0, 0));
        _circleAtlas.Add(four);
    }

    public static List<Vector3> getByRadius(int radius)
    {
        return _circleAtlas[radius];
    }
}
