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

        // Like a diamond.
        List<Vector3> tmp = new List<Vector3>();
        four.Add(new Vector3(0, 5, 0));
        four.Add(new Vector3(5, 0, 0));
        four.Add(new Vector3(0, -5, 0));
        four.Add(new Vector3(-5, 0, 0));
        _circleAtlas.Add(tmp);

        // Like a diamond.
        tmp = new List<Vector3>();
        four.Add(new Vector3(0, 6, 0));
        four.Add(new Vector3(6, 0, 0));
        four.Add(new Vector3(0, -6, 0));
        four.Add(new Vector3(-6, 0, 0));
        _circleAtlas.Add(tmp);

        // Scale with World Res.
        //for (int i = 0; i < _circleAtlas.Count; i += 1)
        //{
        //    for (int j = 0; j < _circleAtlas[i].Count; j++)
        //    {
        //        _circleAtlas[i][j] *= WorldValues.UNIT_SIZE;        
        //    }
        //}
    }

    // bad: this is problematic. A better way is to have a classes of circles.
    //      Ex. Between 2 and 10 returns "somewhat like a ball"
    //      Ex. Between 11 and 100 returns "even more like a ball"
    //      This instead of keeping a record of every single Integer, which is
    //      crazy.
    public static List<Vector3> getByRadius(int radius)
    {
        if (radius < 2)
        {
            return _circleAtlas[radius];
        }
        
        //if (radius % 2 != 0)
        //{
        //    radius -= 1;
        //}

        //radius /= 2;

        List<Vector3> somewhatBall = new List<Vector3>();
        foreach (Vector3 point in _circleAtlas[2])
        {
            somewhatBall.Add(point * radius);
        }

        return somewhatBall;

        // throw new System.Exception("Radius greater than 11 not supported!");
    }
}
