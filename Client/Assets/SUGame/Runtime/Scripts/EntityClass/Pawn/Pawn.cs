using System.Collections;
using System.Collections.Generic;
using IGC.CardCore_IG04.cfg;
using IGC.CardCore_IG04.Luban;
using SUGame.Runtime.Scripts.InputControl;
using UnityEngine;
using File = System.IO.File;
using Mycelia;

public class Pawn : Controller, IPlayerController
{
    //临时
    private const string GameConfDir = "Assets/Scripts/GameConfig/Bin";


    public void SetMoveValue(Vec2 v)
    {

    }

    public void SetAimValue(Vec2 v)
    {

    }

    public void OnMove(bool v)
    {

    }

    public void OnAim(bool v)
    {

    }

    public void OnFire()
    {

    }

    public void SetFireTriggerState(bool v)
    {

    }

    public void OnSkill()
    {

    }


    public struct PawnSprl
    {
        public int pawnId; // 角色唯一标识，比如 0~59
        public int pawnState; // 角色状态
        public Vector3 pawnPos; // 角色当前上报的位置
        public Quaternion pawnRotation; // 角色当前上报的朝向
        public Vector3 pawnAimWorldPos; // 角色当前上报的准星点位置

        public PawnSprl(int pawnId, int pawnState, Vector3 pawnPos, Quaternion pawnRotation, Vector3 pawnAimWorldPos)
        {
            this.pawnId = pawnId;
            this.pawnState = pawnState;
            this.pawnPos = pawnPos;
            this.pawnRotation = pawnRotation;
            this.pawnAimWorldPos = pawnAimWorldPos;
        }
    }

    public PawnSprl[] pawnsSprl = new PawnSprl[60];





    public void Awake()
    {
        TimerSystem.Init();
        //worldLifeID = TimerManager.Register(gF, Pulse, null, true, true, this);

        //gfTimer = new Timer(gF*1000);
        //gfTimer.Elapsed += OnHeartbeat;
        //gfTimer.AutoReset = true;


    }

    void OnEnable()
    {
        // 订阅角色位置上报事件
        EventSystem.Instance.Subscribe(GameEvent.PAWN_POSITION_REPORT, OnPawnSPRLReported);
    }

    void OnDisable()
    {
        // 取消订阅，防止内存泄漏
        if (EventSystem.HasInstance)
            EventSystem.Instance.Unsubscribe(GameEvent.PAWN_POSITION_REPORT, OnPawnSPRLReported);
    }

    private void Pulse()
    {
        Debug.Log("脉搏");

    }
    // private void PulsePause() { TimerSystem.Pause(worldLifeID);}
    // private void PulseResume() { TimerSystem.Resume(worldLifeID);}
    // private void PulseCancel() { TimerSystem.Cancel(worldLifeID);}

    public void Start()
    {
        CfgTablesReadAllBytes();
    }

    public void Update()
    {

    }

    public void CfgTablesReadAllBytes()
    {
        //ByteBuf Func(string file) => new ByteBuf(File.ReadAllBytes($"{GameConfDir}/{file}.bytes"));
        Tables tables = new Tables(file => new ByteBuf(File.ReadAllBytes($"{GameConfDir}/{file}.bytes")));
        //var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
        //var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes($"{gameConfDir}/A.bytes")));
        //Instantiate(tables);
        foreach (GunCfg tableData in tables.GunTable.DataList)
        {
            Debug.Log(tableData.Name);
        }


    }


    private void OnPawnSPRLReported(object eventData)
    {
        // 试图将上报数据转换为 PawnSPRLReport
        if (eventData is PawnSPRLReport report)
        {
            int pawnId = report.pawnId;
            int pawnState = report.pawnState;
            Vector3 pawnPos = report.pawnPos;
            Quaternion pawnRotation = report.pawnRotation;
            Vector3 pawnAimWorldPos = report.pawnAimWorldPos;

            pawnsSprl[pawnId].pawnId = pawnId;
            // 检查索引是否有效
            if (pawnId < 0 || pawnId >= pawnsSprl.Length)
            {
                Debug.LogWarning($"[PawnSPRL] 收到无效的 pawnId: {pawnId}");
                return;
            }

            if (pawnId == 0)
            {
                Debug.Log($"[PawnSPRL] 收到无效的 pawnId: {pawnId}");
                return;
            }

            if (pawnState == 0)
            {
                Debug.Log($"[PawnSPRL] 收到pawn pawnId:{pawnId}的pawnState状态为0，系统将初始化他的id！");
                pawnId = 0;
                pawnsSprl[pawnId].pawnId = 0;
                return;
            }

            pawnsSprl[pawnId].pawnState = pawnState;
            pawnsSprl[pawnId].pawnPos = pawnPos;
            pawnsSprl[pawnId].pawnRotation = pawnRotation;
            pawnsSprl[pawnId].pawnAimWorldPos = pawnAimWorldPos;
        }
    }
}
