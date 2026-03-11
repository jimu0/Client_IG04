using System;
using System.Collections;
using System.Collections.Generic;
using Mycelia;
using SUEngine;
using UnityEngine;

public class InstanceTransformation : MonoBehaviour
{
    private GameObject player;
    private GameObject tileGrid;
    private static WorldState state;
    private static int GObjsLength = 81;
    private GameObject[] GObjs = new GameObject[GObjsLength];
    private readonly List<GameObject> newGObjs;
    private GameObject TotemObjRoot;

    //private static GameObject[] WorldTiles = new GameObject[Igc.GetWorldState.tiles.Length];
    
    
    
    private Vector3 pos;
    void Start()
    {
        // MC.InitTables(new LubanConfigService());
        // state = MC.GetWorldState;
        // StartGObjs();
        // tileGrid = ResourceManager.LoadResSync<GameObject>("P_TileGrid@Art_Pawn_Grid");
        //
        // StartCreateWorldTiles();
    }

    private void OnEnable()
    {
        
    }

    void Update()
    {
        // state = MC.GetWorldState;
        // UpdateGObjsTsf(state);
    }


    void StartGObjs()
    {
        TotemObjRoot = new GameObject($"TotemObjRoot");
        GameObject playerPrefab = ResourceManager.LoadResSync<GameObject>("P_Player@Art_Pawn_Player");
        GameObject totemPrefab = ResourceManager.LoadResSync<GameObject>("P_Totem@Art_Pawn_Totem");
        for (int i = 0; i < GObjs.Length; i++)
        {
            GameObject unitPrefab = i==0 ? playerPrefab : totemPrefab;
            GObjs[i] = Instantiate(unitPrefab, TotemObjRoot.transform, false);
        }
        player = GObjs[0];
        UpdateGObjsTsf(state);
    }

    void UpdateGObjsTsf(in WorldState worldState)
    {
        for (int i = 0; i < GObjs.Length; i++)
        {
            GObjs[i].transform.position = typeCastPos(worldState.unitStates[i].position);
        }
        player.transform.position = GObjs[0].transform.position;
    }

    private static Vector3 typeCastPos(Vec3 p2)
    {
        Vector3 v;
        v.x = p2.x;
        v.y = p2.y;
        v.z = p2.z;
        return v;
    }


    private void StartCreateWorldTiles()
    {
        CreateWorldTileMesh(state.worldWidth, state.tiles.Length, 1f);
        
        
    }

    private void CreateWorldTileMesh(int worldWidth, int tileCount, float tileSize)
    {
        if (worldWidth <= 0 || tileCount <= 0)
        {
            return;
        }

        int worldHeight = (tileCount + worldWidth - 1) / worldWidth;
        int vertexCount = tileCount * 4;
        int indexCount = tileCount * 6;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[indexCount];

        // Tile index -> vertex index mapping:
        // tile i uses vertices [i*4 + 0..3] in order: BL, BR, TL, TR.
        for (int i = 0; i < tileCount; i++)
        {
            int x = i % worldWidth;
            int y = i / worldWidth;

            float x0 = x * tileSize;
            float z0 = y * tileSize;
            float x1 = x0 + tileSize;
            float z1 = z0 + tileSize;

            int v = i * 4;
            vertices[v + 0] = new Vector3(x0, 0f, z0);
            vertices[v + 1] = new Vector3(x1, 0f, z0);
            vertices[v + 2] = new Vector3(x0, 0f, z1);
            vertices[v + 3] = new Vector3(x1, 0f, z1);

            uvs[v + 0] = new Vector2(0f, 0f);
            uvs[v + 1] = new Vector2(1f, 0f);
            uvs[v + 2] = new Vector2(0f, 1f);
            uvs[v + 3] = new Vector2(1f, 1f);

            int t = i * 6;
            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + 2;
            triangles[t + 2] = v + 1;
            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + 2;
            triangles[t + 5] = v + 3;
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = vertexCount > 65535 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GameObject worldMeshObj = Instantiate(tileGrid);
        worldMeshObj.name = "WorldTileMesh";
        MeshFilter mf = worldMeshObj.GetComponent<MeshFilter>();
        if (mf == null)
        {
            mf = worldMeshObj.AddComponent<MeshFilter>();
        }
        mf.sharedMesh = mesh;

        MeshRenderer mr = worldMeshObj.GetComponent<MeshRenderer>();
        if (mr == null)
        {
            mr = worldMeshObj.AddComponent<MeshRenderer>();
        }
        Material tileGridMat = ResourceManager.LoadResSync<Material>("M_TileGrid@Art_Pawn_Grid");
        mr.sharedMaterial = tileGridMat;
        
        // Keep the material from the prefab if it has one.
        worldMeshObj.transform.position = Vector3.zero;
        worldMeshObj.transform.localScale = Vector3.one;
    }
}
