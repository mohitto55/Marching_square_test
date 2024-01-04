using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator 
{
    //public static MapData CreateMapData(float[,] heightMap)
    //{
    //    //MapData mapData = new MapData(heightMap, new Color[0] );
    //    return mapData;
    //}
    public static float[,] GetHeight(Vector2 pos, int SizeX, int SizeZ)
    {
        float[,] height = new float[SizeX + 1, SizeZ + 1];
        for (int z = 0; z <= SizeZ; z++)
        {
            for (int x = 0; x <= SizeX; x++)
            {
                height[x, z] = Mathf.PerlinNoise((pos.x + x) * .2f, (pos.y + z) * .2f);
            }
        }
        return height;
    }
    public static MeshData CreateMeshData(float[,] height)
    {
        int SizeX = height.GetLength(0);
        int SizeZ = height.GetLength(1);
        MeshData meshData = new MeshData(SizeX, SizeZ);
        Vector3[] vertices;
        int[] triangels;
        vertices = new Vector3[(SizeX + 1) * (SizeZ + 1)];

        for (int i = 0, z = 0; z <= SizeZ; z++)
        {
            for (int x = 0; x <= SizeX; x++)
            {
                float y = height[x, z];
                vertices[i] = new Vector3(x - SizeX/2, y * 5, z - SizeZ/2);
                i++;
            }
        }
        triangels = new int[SizeX * SizeZ * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < SizeZ; z++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                triangels[tris + 0] = vert + 0;
                triangels[tris + 1] = vert + SizeX + 1;
                triangels[tris + 2] = vert + 1;
                triangels[tris + 3] = vert + SizeX + 1;
                triangels[tris + 4] = vert + SizeX + 2;
                triangels[tris + 5] = vert + 1;

                vert++;
                tris += 6;
                //yield return new WaitForSeconds(0.01f);
            }
            vert++;
        }
        meshData.vertices = vertices;
        meshData.triangles = triangels;

        return meshData;
    }
}

