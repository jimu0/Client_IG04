using System;
using System.Collections;
using System.Collections.Generic;
using IGC.Engine;
using IGC.Game;
using IGCEngine;
using UnityEngine;

public class InstanceTransformation : MonoBehaviour
{
    private GameObject player;
    private static WorldState state = Igc.GetWorldState;
    private static int GObjsLength = 81;
    private GameObject[] GObjs = new GameObject[GObjsLength];
    private readonly List<GameObject> newGObjs;
    private GameObject TotemObjRoot;
    
    private Vector3 pos;
    void Start()
    {
        state = Igc.GetWorldState;
        StartGObjs();
        GameObject p = ResourceManager.LoadResSync<GameObject>("Player_P_Player@Art_Pawn_Player");
        player = Instantiate(p, new RectTransform(), false);

    }

    private void OnEnable()
    {

    }

    void Update()
    {
        state = Igc.GetWorldState;
        UpdateGObjsTsf(state);
        player.transform.position = GObjs[0].transform.position;
    }


    void StartGObjs()
    {
        TotemObjRoot = new GameObject($"TotemObjRoot");
        for (int i = 0; i < GObjs.Length; i++)
        {
            GObjs[i] = new GameObject($"obj{i}");
            GObjs[i].transform.SetParent(TotemObjRoot.transform);
            //GObjs[i] = Instantiate(null, TotemObjRoot.transform);
        }
        UpdateGObjsTsf(state);
    }

    void UpdateGObjsTsf(in WorldState worldState)
    {
        for (int i = 0; i < GObjs.Length; i++)
        {
            GObjs[i].transform.position = typeCastPos(worldState.totemsState[i].position);
        }

        GObjs[0].transform.position = typeCastPos(worldState.playerState.position);
    }

    private static Vector3 typeCastPos(Vec3 p2)
    {
        Vector3 v;
        v.x = p2.x;
        v.y = p2.y;
        v.z = p2.z;
        return v;
    }
}
