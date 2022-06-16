using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo: visualization tools.
public class MeshDrawing : MonoBehaviour
{
    public int triangleHighlight = -1;

    // Start is called before the first frame update
    //void Start()
    //{
    //    meshNav = NavMesh.meshNav;
    //    startTri = NavMesh.startTri;
    //    endTri = NavMesh.endTri;
    //}

    // perf: creating a mesh (and destroying) every paint!
    // todo: color mesh (green = start, red = stop)
    void OnDrawGizmos()
    {
        for (var i = 0; i < NavMesh.meshNav.Count; i++)
        {
            if (i == NavMesh.startTri - 9000)
            {
                Gizmos.color = Color.green;

                Mesh m = new Mesh();
                m.vertices = new Vector3[3]
                {
                    NavMesh.meshNav[i].v1,
                    NavMesh.meshNav[i].v2,
                    NavMesh.meshNav[i].v3,
                };

                m.triangles = new int[] { 0, 1, 2 };
                m.normals = new Vector3[]
                {
                    Vector3.forward,
                    Vector3.forward,
                    Vector3.forward
                };

                Graphics.DrawMeshNow(m, Vector3.zero, Quaternion.identity);
                DestroyImmediate(m);
            }
            else if (i == NavMesh.endTri - 9000)
            {
                Gizmos.color = Color.red;

                Mesh m = new Mesh();
                m.vertices = new Vector3[3]
                {
                    NavMesh.meshNav[i].v1,
                    NavMesh.meshNav[i].v2,
                    NavMesh.meshNav[i].v3,
                };

                m.triangles = new int[] { 0, 1, 2 };
                m.normals = new Vector3[]
                {
                    Vector3.forward,
                    Vector3.forward,
                    Vector3.forward
                };

                Graphics.DrawMeshNow(m, Vector3.zero, Quaternion.identity);
                DestroyImmediate(m);
            }
            else if (i == triangleHighlight)
            {
                Gizmos.color = Color.yellow;

                Mesh m = new Mesh();
                m.vertices = new Vector3[3]
                {
                    NavMesh.meshNav[i].v1,
                    NavMesh.meshNav[i].v2,
                    NavMesh.meshNav[i].v3,
                };

                m.triangles = new int[] { 0, 1, 2 };
                m.normals = new Vector3[]
                {
                    Vector3.forward,
                    Vector3.forward,
                    Vector3.forward
                };

                Graphics.DrawMeshNow(m, Vector3.zero, Quaternion.identity);
                DestroyImmediate(m);
            }
            else
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(NavMesh.meshNav[i].v1, NavMesh.meshNav[i].v2);
                Gizmos.DrawLine(NavMesh.meshNav[i].v1, NavMesh.meshNav[i].v3);
                Gizmos.DrawLine(NavMesh.meshNav[i].v2, NavMesh.meshNav[i].v3);
            }
        }
    }
}
