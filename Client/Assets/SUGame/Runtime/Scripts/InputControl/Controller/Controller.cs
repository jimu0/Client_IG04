using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SUGame.Runtime.Scripts.InputControl
{
    public class Controller : MonoBehaviour
    {
        public int pawnId; //角色唯一ID
        public int pawnState;
        public Vector3 pawnPos; // 位置
        public Quaternion pawnRotation; // 朝向
        public Vector3 pawnAimWorldPos; // 准星的世界位置
        
        public void ReportPawnPRL()
        {
            // 上报当前位置到事件系统
            var report = new PawnSPRLReport(pawnId,pawnState,pawnPos,pawnRotation,pawnAimWorldPos);
            EventSystem.Instance.Trigger(GameEvent.PAWN_POSITION_REPORT, report);
        }
    }
}

