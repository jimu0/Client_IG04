using System.Collections;
using System.Collections.Generic;
using System.IO;
using IGC.CardCore_IG04.cfg;
using IGC.RPGCore_IG04;
using Mycelia;
using UnityEngine;
using Input = Mycelia.Input;

public class RPGInstantiation : MonoBehaviour
{
    public SimBridge simBridg;

    public GameObject player;
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

        Vector3 pos = Vector3.zero;
        pos.x = MC.GetWorldState.pawnStates[0].tsf.postion.x;
        pos.z = MC.GetWorldState.pawnStates[0].tsf.postion.y;
        player.transform.SetPositionAndRotation(pos, Quaternion.identity);
        timer += Time.deltaTime;

        float distance = Vector3.Distance(oldPos, pos);
        //Debug.Log("控制测试:" +distance);
        speed = distance / timer;
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

        Debug.Log(MC.GetWorldState.pawnStates[0].tsf.rotation.x +" "+MC.GetWorldState.pawnStates[0].tsf.rotation.y);
}
    
    
}
