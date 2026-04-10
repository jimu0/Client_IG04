using System.Collections;
using System.Collections.Generic;
using System.IO;
using IGC.CardCore_IG04.cfg;
using IGC.RPGCore_IG04;
using Mycelia;
using UnityEngine;
using Input = Mycelia.Input;

public class RPGInstantiation : MonoBehaviour, IRender
{
    //public SimBridge simBridg;

    
    public GameObject player;
    public GameObject playerCameraFollowPos;
        
    private PawnController player_PawnController;
    
    //static BinaryReader reader = new (new FileStream("data.bin", FileMode.Open));
    //private ZoneState cardGameZoneState = new BinaryStateSerializer().ReadZoneList(reader);

    private Vector3 oldPos;
    float timer = 0f;
    float speed = 0.1f;
    
    void Start()
    {
        //Debug.Log($"time:{MC.GetWorldState.debugText}");
        //Debug.Log($"位置测试:{MC.GetWorldState.pawnStates[0].tsf.postion.ToString()}");
        player_PawnController = player.GetComponent<PawnController>();
        
        
    }

    void Update()
    {


    }


    public void OnRender(in WorldState state)
    {
        var roleStates = state.roleStates[0];
        Vector3 pos = new (pos.x = roleStates.tsf.postion.x,0,pos.x = roleStates.tsf.postion.y);
        Vector3 rot = new (roleStates.tsf.direction.x, 0, roleStates.tsf.direction.y);
        Quaternion rotQ = Quaternion.LookRotation(rot);
        player.transform.SetPositionAndRotation(pos,rotQ);
        playerCameraFollowPos.transform.SetPositionAndRotation(pos, playerCameraFollowPos.transform.rotation);
        
        
        
        float distance = Vector3.Distance(oldPos, pos);
        Debug.Log("控制测试:" + state.debugText);
        speed = distance / Time.deltaTime;
        oldPos = pos;

        // if (timer >= 0.0334f)
        // {
        //     float distance = Vector3.Distance(oldPos,pos);
        //     //Debug.Log("控制测试:" +distance);
        //     speed = distance / timer;
        //     oldPos = pos;
        //     timer -= 0.0334f;   // 防止漂移
        // }

        player_PawnController.speed = speed;
    }
}
