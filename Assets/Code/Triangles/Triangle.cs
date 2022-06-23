using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    //Corners
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    //If we are using the half edge mesh structure, we just need one half edge
    //public HalfEdge halfEdge;

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }

    //public Triangle(HalfEdge halfEdge)
    //{
    //    this.halfEdge = halfEdge;
    //}

    //Change orientation of triangle from cw -> ccw or ccw -> cw
    public void ChangeOrientation()
    {
        Vector3 temp = this.v1;

        this.v1 = this.v2;

        this.v2 = temp;
    }

    public bool ContainsVertex(Vector3 vert)
    {
        if (v1 == vert || v2 == vert || v3 == vert)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// We will be using the method of determinants found here:
    /// https://en.wikipedia.org/wiki/Triangle#Using_coordinates
    /// We are not using the absolute value, as the negative value of the area is valuable for 
    /// determining the orientation of the triangle.
    /// Clockwise triangles have negative areas. CounterClockwise have positive.
    /// </summary>
    /// <returns></returns>
    // bug: float, if Triangle contains non-integer vector3s we are in trouble
    public float AreaTimesTwo()
    {
        Vector3 BA = v2 - v1;
        Vector3 CA = v3 - v1;

        float lhs = BA.x * CA.y;
        float rhs = CA.x * BA.y;

        return (BA.x * CA.y) - (CA.x * BA.y);
    }

    // bug: not deterministic.
    // ask: can we just multiply these by three and use them in a way where that should be OK? 
    public Vector3 GetCenter()
    {
        return new Vector3(
            (v1.x + v2.x + v3.x) / 3,
            (v1.y + v2.y + v3.y) / 3,
            (v1.z + v2.z + v3.z) / 3
        );
    }
}

//public class HalfEdge
//{
//    //The vertex the edge points to
//    public Vertex v;

//    //The face this edge is a part of
//    public Triangle t;

//    //The next edge
//    public HalfEdge nextEdge;
//    //The previous
//    public HalfEdge prevEdge;
//    //The edge going in the opposite direction
//    public HalfEdge oppositeEdge;

//    //This structure assumes we have a vertex class with a reference to a half edge going from that vertex
//    //and a face (triangle) class with a reference to a half edge which is a part of this face 
//    public HalfEdge(Vertex v)
//    {
//        this.v = v;
//    }
//}