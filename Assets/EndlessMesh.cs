using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
public class EndlessMesh : MonoBehaviour
{
    public static EndlessMesh mapGenerator;
    public class TerrainChunk
    {
        Mesh mesh;
        MeshFilter meshFilter;
        Vector2 position;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, int size, MeshFilter meshFiliter)
        {
            //mesh = MeshGenerator.CreateShape(coord * size, size, size);
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            meshFilter = Instantiate(meshFiliter);
            SetVisible(false);
            Debug.Log("청크생성");
            mapGenerator.RequesMapData(coord, OnMapDataRecived);
        }
        void OnMapDataRecived(MapData mapData)
        {
            Debug.Log("맵 데이터 리시브");
            mapGenerator.RequestMeshData(mapData, OnMeshDataRecived);
            
        }
        void OnMeshDataRecived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            Debug.Log("버텍스 겟수 : " + mesh.vertices.Length);
            meshFilter.mesh = mesh;
            meshFilter.gameObject.name = "Terrain chunk";
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);
            meshFilter.transform.position = positionV3;
            meshFilter.transform.localScale = Vector3.one;
        }
        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshFilter.gameObject.SetActive(visible);
        }
        public bool IsVisible()
        {
            return meshFilter.gameObject.activeSelf;
        }
    }
    public const float maxViewDst = 300;
    public Transform viewer;
    public float viewerDir;
    public float viewerHeight;
    public float viewerSpeed;
    public MeshFilter mesh;

    public static Vector2 viewerPosition;
    public int mapChunkSize = 241;
    int chunkSize;
    int chunkVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    void Start()
    {
        mapGenerator = this;
        chunkSize = mapChunkSize - 1;
        chunkVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }
    void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunk();

        if(mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                Debug.Log("데이터 전 : " + mapDataThreadInfoQueue.Count);
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                Debug.Log("데이터 후 : " + mapDataThreadInfoQueue.Count);
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0; i< meshDataThreadInfoQueue.Count; i++)
            {
                Debug.Log("메쉬 전 : " + meshDataThreadInfoQueue.Count);
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
                Debug.Log("메쉬 후 : " + meshDataThreadInfoQueue.Count);
            }
        }
    }
    void MoveViewer()
    {
        float dir = viewerDir * Mathf.Deg2Rad;
        Vector3 dirc = new Vector3(Mathf.Cos(dir), 0, Mathf.Sin(dir));
        viewer.transform.position += dirc * viewerSpeed * Time.deltaTime;

        Camera.main.transform.position = new Vector3(viewer.position.x, viewerHeight, viewer.position.z);
        Camera.main.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();
    public void RequesMapData(Vector2 coord, Action<MapData> callback)
    {
        Debug.Log("요청 맵 데이터");
        ThreadStart threadStart = delegate
        {
            MapDataThread(coord, callback);
        };

        new Thread(threadStart).Start();
    }
    void MapDataThread(Vector2 coord, Action<MapData> callback)
    {
        float[,] height = MeshGenerator.GetHeight(coord, chunkSize, chunkSize);
        MapData mapData = new MapData(coord, height, new Color[0]);
       
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate {
            MeshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
    }

    public void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.CreateMeshData(mapData.heightMap);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
    }
    void UpdateVisibleChunk()
    {
        for(int i =0;i < terrainChunksVisibleLastUpdate.Count;i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        for(int yOffset = -chunkVisibleInViewDst; yOffset <= chunkVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunkVisibleInViewDst; xOffset <= chunkVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if(terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainChunkDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                    }
                } else
                {
                    //Mesh terrainMesh = MeshGenerator.CreateShape(viewedChunkCoord, chunkSize, chunkSize);
                    //mesh.mesh = terrainMesh;
                    terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord , chunkSize, mesh));
                    Debug.Log("새 청크 생성 : " + viewedChunkCoord);
                }
            }
        }
    }
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

}
public struct MapData
{
    public readonly Vector2 pos;
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;

    public MapData(Vector2 pos, float[,] heightMap, Color[] colourMap)
    {
        this.pos = pos;
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}


