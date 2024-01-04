using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] int XSize = 1;
    [SerializeField] int YSize = 1;
    private void Start()
    {
        //GenerateMesh();
        GenerateBox();
        //meshFilter.mesh = GenerateCircleMesh();
    }
    private const int CircleSegmentCount = 32;
    private const int CircleVertexCount = CircleSegmentCount + 1;
    private const int CircleIndexCount = CircleSegmentCount * 3;

    private static Mesh GenerateCircleMesh()
    {
        Mesh circleMesh = new Mesh();
        Vector3[] vertices = new Vector3[CircleVertexCount];
        int[] triangle = new int[CircleIndexCount];
        vertices[0] = Vector3.zero;
        for (int i = 1; i <= CircleSegmentCount; i++)
        {
            float theta =  (float)360 / CircleSegmentCount * i;
            Debug.Log(theta);
            theta *= Mathf.Deg2Rad;
            vertices[i] = new Vector3(Mathf.Cos(-theta), 0, Mathf.Sin(-theta));
            Debug.Log(vertices[i]);
        }
        for(int i = 1; i <= CircleSegmentCount; i++)
        {
            triangle[i * 3- 3] = 0;
            triangle[i * 3 - 2] = i;
            if (i != CircleSegmentCount)
            {
                triangle[i * 3 - 1] = i + 1;
            }
            else
            {
                triangle[i * 3 - 1] = 1;
            }
        }
        circleMesh.vertices = vertices;
        circleMesh.triangles = triangle;
        return circleMesh;
    }

    void GenerateBox()
    {
        Mesh testmesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,0,1),
            new Vector3(1,0,1),
            new Vector3(0,1,1),
            new Vector3(1,1,1),
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
        };
        int[] triangle = new int[36];
        triangle[0] = 0;
        triangle[1] = 1;
        triangle[2] = 2;

        triangle[3] = 1;
        triangle[4] = 3;
        triangle[5] = 2;

        triangle[6] = 2;
        triangle[7] = 3;
        triangle[8] = 7;

        triangle[9] = 2;
        triangle[10] = 7;
        triangle[11] = 6;

        triangle[12] = 1;
        triangle[13] = 7;
        triangle[14] = 3;

        triangle[15] = 1;
        triangle[16] = 5;
        triangle[17] = 7;

        triangle[18] = 6;
        triangle[19] = 7;
        triangle[20] = 4;

        triangle[21] = 7;
        triangle[22] = 5;
        triangle[23] = 4;

        triangle[24] = 0;
        triangle[25] = 4;
        triangle[26] = 1;

        triangle[27] = 1;
        triangle[28] = 4;
        triangle[29] = 5;

        triangle[30] = 2;
        triangle[31] = 6;
        triangle[32] = 4;

        triangle[33] = 0;
        triangle[34] = 2;
        triangle[35] = 4;
        Vector3[] uvs = new Vector3[4];
        testmesh.vertices = vertices;
        testmesh.triangles = triangle;
        testmesh.RecalculateBounds();
        meshFilter.mesh = testmesh;
    }
    void GenerateMesh()
    {
        Mesh testmesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        int[] triangle = new int[3];
        Vector3[] uvs = new Vector3[4];


        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 1, 0);
        vertices[3] = new Vector3(1, 1, 0);

        triangle[0] = 0;
        triangle[1] = 2;
        triangle[2] = 1;
        triangle[3] = 2;
        triangle[4] = 3;
        triangle[5] = 1;
        testmesh.vertices = vertices;
        testmesh.triangles = triangle;
        testmesh.RecalculateBounds();
        meshFilter.mesh = testmesh;
    }
}
