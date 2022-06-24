using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMesh
{
    public static List<Triangle> meshNav = new List<Triangle>();

    // Each index of neighbors here corresponds to a triangle in the mesh above.
    public static List<List<int>> neighbors = new List<List<int>>(); 
    public static int startTri;
    public static int endTri;

    MeshFilter meshFilter;

    public NavMesh() {}

    /// <summary>
    /// Rebuilds the navmesh, run whenever something changes the mesh (Adding a Building).
    /// </summary>
    /// <param name="circles"></param>
    public static void Repath(List<CircleMcCollider> circles)
    {
        List<Vector3> spacePoints = new List<Vector3>();

        // note: testing values are very small.
        //       replace this for the game with actual bounds values for the map.
        //spacePoints.Add(new Vector3(0, 0, 0));
        //spacePoints.Add(new Vector3(0, Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES), 0));
        //spacePoints.Add(new Vector3(Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES), Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES), 0));
        //spacePoints.Add(new Vector3(Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES), 0, 0));
        spacePoints.Add(new Vector3(0, 0, 0));
        spacePoints.Add(new Vector3(0, Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES - 10), 0));
        spacePoints.Add(new Vector3(Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES - 10), Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES - 10), 0));
        spacePoints.Add(new Vector3(Mathf.Pow(2, WorldValues.MIN_SUPPORTED_BIT_RES - 10), 0, 0));

        // Add vertices of anything that must be pathed around (bases, walls, etc)
        // CircleMcCollider[] circles = GetComponentsInChildren<CircleMcCollider>();
        foreach (CircleMcCollider circle in circles)
        {
            // circle's vertices are like offsets, they need to be added to the initial
            // transform.position.
            CircleAliased pathCircle = circle.getAliased();
            List<Vector3> v3s = new List<Vector3>();
            foreach (Vector3 vertex in pathCircle.vertices)
            {
                v3s.Add(circle.transform.position + vertex);
            }

            spacePoints.AddRange(v3s);
        }

        // Our triangulation.
        //meshNav = Triangulate(spacePoints);
        meshNav = DelaunayTriangulation(spacePoints);
        neighbors = FindNeighbors(meshNav);

        // test: to see visuals.
        startTri = 1;
        endTri = meshNav.Count - 1;

        // todo: guarantee triangulation constraints are respected.

    }

    /// <summary>
    /// Neighbors are joined by having at least vertex in common.
    /// </summary>
    /// <param name="triangleMesh"></param>
    /// <returns></returns>
    // perf: Neighbors are triangles which share a vertex. This could be bad for A*
    public static List<List<int>> FindNeighbors(List<Triangle> triangleMesh)
    {
        List<List<int>> neighbors = new List<List<int>>();
        for(var i = 0; i < triangleMesh.Count; i++)
        {
            List<int> indexes = new List<int>();
            for(var j = 0; j < triangleMesh.Count; j++)
            {
                if (i != j)
                {
                    if (triangleMesh[i].v1 == triangleMesh[j].v1
                        || triangleMesh[i].v1 == triangleMesh[j].v2
                        || triangleMesh[i].v1 == triangleMesh[j].v3
                        || triangleMesh[i].v2 == triangleMesh[j].v1
                        || triangleMesh[i].v2 == triangleMesh[j].v2
                        || triangleMesh[i].v2 == triangleMesh[j].v3
                        || triangleMesh[i].v3 == triangleMesh[j].v1
                        || triangleMesh[i].v3 == triangleMesh[j].v2
                        || triangleMesh[i].v3 == triangleMesh[j].v3)
                    {
                        indexes.Add(j);
                    }
                }
            }

            neighbors.Add(indexes);
        }

        return neighbors;
    }

    ///Is a point d inside, outside or on the same circle as a, b, c
    //https://gamedev.stackexchange.com/questions/71328/how-can-i-add-and-subtract-convex-polygons
    //Returns positive if inside, negative if outside, and 0 if on the circle
    // bug: warning, floats.
    public static float IsPointInsideOutsideOrOnCircle(Vector2 aVec, Vector2 bVec, Vector2 cVec, Vector2 dVec)
    {
        //This first part will simplify how we calculate the determinant
        float a = aVec.x - dVec.x;
        float d = bVec.x - dVec.x;
        float g = cVec.x - dVec.x;

        float b = aVec.y - dVec.y;
        float e = bVec.y - dVec.y;
        float h = cVec.y - dVec.y;

        float c = a * a + b * b;
        float f = d * d + e * e;
        float i = g * g + h * h;

        float determinant = (a * e * i) + (b * f * g) + (c * d * h) - (g * e * c) - (h * f * a) - (i * d * b);

        return determinant;
    }

    //Is a triangle in 2d space oriented clockwise or counter-clockwise
    //https://math.stackexchange.com/questions/1324179/how-to-tell-if-3-connected-points-are-connected-clockwise-or-counter-clockwise
    //https://en.wikipedia.org/wiki/Curve_orientation
    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }

    // bug: warning, float usage.
    public static List<Triangle> DelaunayTriangulation(List<Vector3> mapPoints)
    {
        List<Triangle> triangles = new List<Triangle>();

        // idea here is to have a list of all points, find a minimal sized circle
        // that contains three points, check if that circle contains ANY other points.
        // if it DOES move on to another three points, if it DOES NOT, then record these
        // three as Delaunay.
        int i = 0;
        int j = 1;
        int k = 2;
        while (i < mapPoints.Count)
        {
            if (j >= mapPoints.Count)
            {
                i += 1;
                j = i + 1;
                k = j + 1;
                continue;
            }
            if (k >= mapPoints.Count)
            {
                j += 1;
                k = j + 1;
                continue;
            } 

            Triangle test = new Triangle(mapPoints[i], mapPoints[j], mapPoints[k]);

            // test if any of the other points are in this triangle!
            bool isDelaunay = true;
            for (int z = 0; z < mapPoints.Count; z++)
            {
                // note: ignore points that are already a part of the triangle.
                //       triangles must be oriented in the same direction. Here we force them
                //       to be counter-clockwise.
                if (z != i && z != j && z != k)
                {
                    if (IsTriangleOrientedClockwise(test.v1, test.v2, test.v3)) 
                    {
                        test.ChangeOrientation();
                    }
                    if (IsPointInsideOutsideOrOnCircle(test.v1, test.v2, test.v3, mapPoints[z]) >= 0)
                    {
                        isDelaunay = false;
                        break;
                    }
                }
            }

            if (isDelaunay)
            {
                triangles.Add(test);
            }

            k += 1;
        }

        return triangles;
    }

    /// <summary>
    /// Triangulates some collection of points from a mesh or map.
    /// This is gahbage, leads to overlapping triangles, does not produce
    /// unique triangles, and each Triangle will only have one neighbor.
    /// </summary>
    /// <param name="mapPoints"></param>
    /// <returns></returns>
    public static List<Triangle> Triangulate(List<Vector3> mapPoints)
    {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 2; i < mapPoints.Count; i++)
        {
            Vector3 a = mapPoints[0];
            Vector3 b = mapPoints[i - 1];
            Vector3 c = mapPoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }

    public static List<Vector3> ComputePath(VectorFixed start, VectorFixed end)
    {
        // Just query every triangle to see which triangle contains the begin/end of the path.
        int startTri = FindContainingTriangle(start);
        int endTri = FindContainingTriangle(end);

        if (startTri == -1 || endTri == -1)
        {
            Debug.Log("User cannot move to occupied space, or a space off the map.");
            // throw new System.Exception("Destination, or maybe even start triangle, not found.");
        }

        // A* through the list of triangles to find the shortest path of triangles
        List<Triangle> shortPath = FindShortestPath(startTri, endTri, start, end);

        // note: we don't need first Triangle, for TriangleCenters, so we remove it.
        //       We WILL need this for stupid funnel however.
        //if (shortPath.Count > 0)
        //{
        //    shortPath.RemoveAt(0);
        //}
        shortPath.Add(meshNav[endTri]);

        // todo: use the simple stupid funnel to get a path of Vector3.
        List<Vector3> bfish =  StupidFunnel(shortPath, start, end);

        // testing: just return the triangle centers for now. 
        return TriangleCenters(shortPath);
    }

    public static List<Vector3> TriangleCenters(List<Triangle> triangles)
    {
        List<Vector3> centers = new List<Vector3>();
        foreach(Triangle tri in triangles)
        {
            centers.Add(tri.GetCenter());
        }

        return centers;
    }

    /// <summary>
    /// Uses A* to find the shortest path between mesh of triangles.
    /// We should be using a priority queue and determining value of some path via distance without square root.
    /// </summary>
    /// <param name="begin"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static List<Triangle> FindShortestPath(int begin, int end, VectorFixed startVector, VectorFixed destination)
    {
        // Custom sort for this based on Triangle Centroid I guess. Sort will compare
        // each value by totalScore[]
        List<int> open = new List<int>();
        open.Add(begin);

        Dictionary<int,int> cameFrom = new Dictionary<int,int>();

        // gScore[i] is cost of cheapest path from begin to triangle i. Since we start at begin, this is 0.
        Dictionary<int, float> gScore = new Dictionary<int, float>();
        gScore[begin] = 0;

        // totalScore[i] = gScore[i] + the distance between triangle i and end (by Triangle Centroid...)
        Dictionary<int, float> totalScore = new Dictionary<int, float>();
        totalScore[begin] = Vector3.SqrMagnitude(meshNav[end].GetCenter() - meshNav[begin].GetCenter());
       
        while (open.Count > 0)
        {
            int current = open[0];
            if (current == end)
            {
                return CreatePath(cameFrom, current);
            }

            open.RemoveAt(0);

            // Sort based on totalScore. 
            open.Sort(delegate (int one, int other)
            {
                if (totalScore[one] < totalScore[other])
                {
                    return -1;
                }
                if (totalScore[one] > totalScore[other])
                {
                    return 1;
                }

                return 0;
            });

            for (var i = 0; i < neighbors[current].Count; i++)
            {
                float testScore = 0;
                Vector3 currentPosition;
                Vector3 nextPosition;
                if (neighbors[current][i] == end)
                {
                    if (current == begin)
                    {
                        currentPosition = startVector.AsUnityTransform();
                        nextPosition = destination.AsUnityTransform();
                    }
                    else
                    {
                        currentPosition = meshNav[current].GetCenter();
                        nextPosition = destination.AsUnityTransform();
                    }
                }
                else
                {
                    if (current == begin)
                    {
                        currentPosition = startVector.AsUnityTransform();
                        nextPosition = meshNav[neighbors[current][i]].GetCenter();
                    }
                    else
                    {
                        currentPosition = meshNav[current].GetCenter();
                        nextPosition = meshNav[neighbors[current][i]].GetCenter();
                    }
                }

                testScore = gScore[current]
                    + Vector3.SqrMagnitude(currentPosition - nextPosition);

                // If path from here is not yet recorded, or is smaller than what is currently recorded 
                // given the path to THIS POINT on record.
                if (gScore.ContainsKey(neighbors[current][i]) == false 
                    || testScore < gScore[neighbors[current][i]])
                {
                    cameFrom[neighbors[current][i]] = current;
                    gScore[neighbors[current][i]] = testScore;
                    totalScore[neighbors[current][i]] = testScore 
                        + Vector3.SqrMagnitude(destination.AsUnityTransform() - nextPosition);

                    if (open.Contains(neighbors[current][i]) == false)
                    {
                        open.Add(neighbors[current][i]);
                        open.Sort(delegate (int one, int other)
                        {
                            if (totalScore[one] < totalScore[other])
                            {
                                return -1;
                            }
                            if (totalScore[one] > totalScore[other])
                            {
                                return 1;
                            }

                            return 0;
                        });
                    }
                }
            }
        }

        throw new System.Exception("Path not found somehow.");
    }

    public static List<Triangle> CreatePath(Dictionary<int,int> cameFrom, int current)
    {
        List<Triangle> path = new List<Triangle>();

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, meshNav[current]);

            // test: avoid unity crashes.
            if (path.Count > 1000) return path;
        }

        return path;
    }

    //public struct PathNode
    //{
    //    int index;
    //    float distance;
    //}

    /// <summary>
    /// For some triangle in a list, return its vertex which has no shared triangle.
    /// If not found, raise exception. Requires that this triangle have a neighbor.
    /// </summary>
    /// <param name="triangles"></param>
    /// <param name="index"></param>
    /// <returns>Triplet, with first item being that with no shared triangle.</returns>
    public static (Vector3, Vector3, Vector3) VertexWithoutSharedTriangle(List<Triangle> triangles, int index)
    {
        bool isOne = true;
        bool isTwo = true;
        bool isThree = true;
        for (var i = 0; i < triangles.Count; i++)
        {
            if (i == index) continue;

            if (triangles[i].v1 == triangles[index].v1
                || triangles[i].v2 == triangles[index].v1
                || triangles[i].v3 == triangles[index].v1)
            {
                isOne = false;
            }
            
            if (triangles[i].v1 == triangles[index].v2
                || triangles[i].v2 == triangles[index].v2
                || triangles[i].v3 == triangles[index].v2)
            {
                isTwo = false;
            }

            if (triangles[i].v1 == triangles[index].v3
                || triangles[i].v2 == triangles[index].v3
                || triangles[i].v3 == triangles[index].v3)
            {
                isThree = false;
            }
        }

        if (isOne) return (triangles[index].v1, triangles[index].v2, triangles[index].v3);
        if (isTwo) return (triangles[index].v2, triangles[index].v1, triangles[index].v3);
        if (isThree) return (triangles[index].v3, triangles[index].v1, triangles[index].v2);

        throw new System.Exception("Chosen triangle must have a neighbor.");
    }

     //* is it within funnel? - lets say 'it' is newLeft. newLeft, if in funnel, would then be on the same side
     //                         of oldLeft - base as oldRight. This is our test.
     //*    if so, did it cross over?
     //*        if so, add to path, RESTART from here (get left and right as in the begining)
     //*        else update either oldLeft/oldRight to newLeft/right.
     //* else, just leave it.
     // todo: clean up the bad funnel alg.
    //public static List<Vector3> AltStupidFunnel(List<Triangle> path)
    //{
    //    List<Vector3> result = new List<Vector3>();

    //    // init: find verts with shared edges. The one without at first triangle is base.
    //    // note: 'left' is randomly one of the other two here. later we will ascribe it to be the vertex
    //    //       which is only neighboring with triangle current.
    //    (Vector3 apex, Vector3 left, Vector3 right) = VertexWithoutSharedTriangle(path, 0);
    //    int currentTriangle = 0;
        
    //    // we stop when we would add a point from last triangle to result. we would likely add the destination
    //    // that the user picked out in the first place to the end of this list.
    //    while (true)
    //    {
    //        // find triangle that is 
    //        // test newLeft

    //        // test newRight
    //    }
    //}

    // perf: will generate duplicate vertices in results.
    public static List<Vector3> VerticesSharingAnEdge(Vector3 focus, List<Triangle> amongst)
    {
        List<Vector3> verts = new List<Vector3>();
        // for every triangle that has focus in it, return all other vertices.
        foreach(Triangle tri in amongst)
        {
            if (tri.v1 == focus)
            {
                verts.Add(tri.v2);
                verts.Add(tri.v3);
            }
            else if (tri.v2 == focus)
            {
                verts.Add(tri.v1);
                verts.Add(tri.v3);
            }
            else if (tri.v3 == focus)
            {
                verts.Add(tri.v1);
                verts.Add(tri.v2);
            }
        }

        return verts;
    }

    public static List<Vector3> StupidFunnel(List<Triangle> path, VectorFixed start, VectorFixed end)
    {
        List<Vector3> vectorPath = new List<Vector3>();
        if (path.Count == 0)
        {
            return vectorPath;
        }

        // pick any two vectors from triangle
        Vector3 apex = start.AsUnityTransform();
        (Vector3 dontUse, Vector3 one, Vector3 other) = VertexWithoutSharedTriangle(path, 0);

        Triangle currentTriangle = new Triangle(apex, one, other);
        float currentArea = currentTriangle.AreaTimesTwo();

        int protection = 0;

        while (true) 
        {
            protection += 1;
            bool oneStuck = true;
            bool otherStuck = true;

            if (protection > 1000)
            {
                return new List<Vector3>();
            }

            // get list of vertices which share an edge with one.
            List<Vector3> sharedVertices = VerticesSharingAnEdge(one, path);

            // from list of vertices which one or other shares a path with, test each one.
            for (int i = 0; i < sharedVertices.Count; i++)
            {
                Triangle test = new Triangle(apex, sharedVertices[i], other);
                float areaTest = test.AreaTimesTwo();

                LineCollider line = new LineCollider();
                line.begin = VectorFixed.FromVector3(apex);
                line.end = VectorFixed.FromVector3(one);
                CircleCollider pointTest = new CircleCollider();
                pointTest.begin = VectorFixed.FromVector3(sharedVertices[i]);
                long testPoint = FFI.SideOfLine(line, pointTest);

                CircleCollider pointCurrent = new CircleCollider();
                pointCurrent.begin = VectorFixed.FromVector3(other);
                long currentPoint = FFI.SideOfLine(line, pointCurrent);
                if (areaTest == 0)
                {
                    // not a triangle.
                }
                // todo: first conditional here should be a test of whether point is between currentTriangle
                //       v2 - v1 and v3 - v1.
                else if (Mathf.Sign(testPoint) == Mathf.Sign(currentPoint) && Mathf.Sign(areaTest) == Mathf.Sign(currentArea))
                {
                    oneStuck = false;
                    one = sharedVertices[i];
                    currentTriangle = new Triangle(apex, one, other);
                    currentArea = currentTriangle.AreaTimesTwo();
                }
            } 

            if (oneStuck == false)
            {
                continue;
            }

            // get list of vertices which share an edge with one.
            sharedVertices = VerticesSharingAnEdge(other, path);

            // from list of vertices which one or other shares a path with, test each one.
            for (int i = 0; i < sharedVertices.Count; i++)
            {
                Triangle test = new Triangle(apex, one, sharedVertices[i]);
                float areaTest = test.AreaTimesTwo();

                LineCollider line = new LineCollider();
                line.begin = VectorFixed.FromVector3(apex);
                line.end = VectorFixed.FromVector3(other);
                CircleCollider pointTest = new CircleCollider();
                pointTest.begin = VectorFixed.FromVector3(sharedVertices[i]);

                // bug: this is marking 0 (colinear) where it is not.
                long testPoint = FFI.SideOfLine(line, pointTest);

                CircleCollider pointCurrent = new CircleCollider();
                pointCurrent.begin = VectorFixed.FromVector3(one);
                long currentPoint = FFI.SideOfLine(line, pointCurrent);
                if (areaTest == 0)
                {
                    // not a triangle.
                }
                else if (Mathf.Sign(testPoint) == Mathf.Sign(currentPoint) && Mathf.Sign(areaTest) == Mathf.Sign(currentArea))
                {
                    otherStuck = false;
                    other = sharedVertices[i];
                    currentTriangle = new Triangle(apex, one, other);
                    currentArea = currentTriangle.AreaTimesTwo();
                }
            } 

            // if the area of the triangle between itself, its shared vertex, and the apex
            // becomes larger, continue. If there is no point that makes the triangle smaller,
            // move one to the point where the triangle area is zero or opposing, mark as stuck.

            // if both are "stuck", the point of 'one' will be added to vectorPath, and this will be the
            // new apex.
            if (oneStuck && otherStuck)
            {
                vectorPath.Add(other);
                // take 'one' as v2, and take the new 'other' as a neighbor of apex such
                // that it is not ALSO a neighbor of 'one' AND is not in ANY triangle containing apex.
                // in addition, we need a corner case for the initial triangle in the path, as
                // this will need to not include "don't use"

                // the point which is CROSSING can be the new v2. There are two triangles that apex and 
                // this new v2 can be a part of. Pick the one whose orientation matches
                // Triangle(apex, OTHER, point which would have gone over)
                apex = other;
                other = FindVertexNotInTriangleConnectedToAVertex(currentTriangle, one, apex, dontUse);
                currentTriangle = new Triangle(apex, one, other);
            }

            if (path[path.Count - 1].ContainsVertex(apex))
            {
                vectorPath.Add(end.AsUnityTransform());
                return vectorPath;
            }

            // Pick v1 point from first triangle.
            // Vector3 basePoint = path[i].v1;

            // Determine whether v2 or v3 are shared with next triangle.
            // (Vector3 left, Vector3 right) = FindLeftAndRightVertices(basePoint, path[i], path[i + 1]);

            // move on to next triangle, move left
            // again, but move right

            // cross product to determine whether new point is on opposite side of funnel.
            // if so, add previous left/right to vectorPath, and restart with this left/right as base.
            // continue;
        }

        throw new System.Exception("Funnel algorithm failed.");
    }

    public static Vector3 FindVertexNotInAnyTriangleConnectedToAVertex
        (List<Triangle> triangles, Vector3 vertex, Vector3 focus, Vector3 otherVertex)
    {

        // get all neighbors of focus.
        List<Vector3> neighbors = VerticesSharingAnEdge(focus, meshNav);

        // get all neighbors of vertex
        List<Vector3> vertexNeighbors = VerticesSharingAnEdge(vertex, meshNav);

        // for each neighbor, return the first that is not a member of vertexNeighbors and also not
        //       in triangle.
        Vector3 result;
        foreach (Vector3 neighbor in neighbors)
        {
            if (neighbor != vertex && triangle.ContainsVertex(neighbor) == false
                && neighbor != otherVertex)
            {
                return neighbor;
            }
        }

        throw new System.Exception("Somehow, there is no fitting neighbor.");
    }

    /// <summary>
    /// basePoint will be one of the vertices of current, but we don't know which.
    /// </summary>
    /// <param name="basePoint"></param>
    /// <param name="current"></param>
    /// <param name="next"></param>
    /// <returns>A tuple, item1 is "left".</returns>
    public static (Vector3, Vector3) FindLeftAndRightVertices(Vector3 basePoint, Triangle current, Triangle next)
    {
        // get OTHER points of triangle.
        if (basePoint == current.v1)
        {
            if (current.v3 == next.v1
                || current.v3 == next.v2
                || current.v3 == next.v3)
            {
                return (current.v2, current.v3);
            }
            else
            {
                return (current.v3, current.v2);
            }
        } 
        else if (basePoint == current.v2)
        {
            if (current.v3 == next.v1
                || current.v3 == next.v2
                || current.v3 == next.v3)
            {
                return (current.v1, current.v3);
            }
            else
            {
                return (current.v3, current.v1);
            }
        } 
        else
        {
            if (current.v1 == next.v1
                || current.v1 == next.v2
                || current.v1 == next.v3)
            {
                return (current.v2, current.v1);
            }
            else
            {
                return (current.v1, current.v2);
            }
        }
    }

    public static float SaferDotProduct(Vector3 one, Vector3 other)
    {
        return (one.z * other.z) + (one.y * other.y) + (one.x + other.x);
    }

    /// note: I don't know of any other way to do this than via testing if the
    ///       object is on the same side of each side of the Triangle. There are
    ///       other ways, but they are not deterministic. 
    // perf: recalculating vectors. 
    public static int FindContainingTriangle(VectorFixed target)
    {
        Vector3 targ = target.AsUnityTransform();
        for (var i = 0; i < meshNav.Count; i++)
        {
            // We only care about the Z value of all of these.
            Vector3 TwoOneCrossVertex = Vector3.Cross
                (meshNav[i].v2 - meshNav[i].v1, meshNav[i].v3 - meshNav[i].v1);
            Vector3 TwoOneCrossTarget = Vector3.Cross
                (meshNav[i].v2 - meshNav[i].v1, targ - meshNav[i].v1);
            float oneRes = SaferDotProduct(TwoOneCrossVertex, TwoOneCrossTarget);

            Vector3 ThreeTwoCrossVertex = Vector3.Cross
                (meshNav[i].v3 - meshNav[i].v2, meshNav[i].v1 - meshNav[i].v2);
            Vector3 ThreeTwoCrossTarget = Vector3.Cross
                (meshNav[i].v3 - meshNav[i].v2, targ - meshNav[i].v2);
            float twoRes = SaferDotProduct(ThreeTwoCrossVertex, ThreeTwoCrossTarget);

            Vector3 ThreeOneCrossVertex = Vector3.Cross
                (meshNav[i].v3 - meshNav[i].v1, meshNav[i].v2 - meshNav[i].v1);
            Vector3 ThreeOneCrossTarget = Vector3.Cross
                (meshNav[i].v3 - meshNav[i].v1, targ - meshNav[i].v1);

            float threeRes = SaferDotProduct(ThreeOneCrossVertex, ThreeOneCrossTarget);

            if (Mathf.Sign(oneRes) == Mathf.Sign(twoRes) && Mathf.Sign(oneRes) == Mathf.Sign(threeRes))
            {
                return i;
            }
        }

        return -1;
    }

    public float isLeft(Vector3 a, Vector3 b, Vector3 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x));
    }

}

struct Line
{
    Vector3 begin;
    Vector3 end;

    public Line(Vector3 begin, Vector3 end)
    {
        this.begin = begin;
        this.end = end;
    }
}
