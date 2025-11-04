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
        public int targetPawnId;
        
        public void ReportPawnSPRL(int id,int state,Vector3 pos,Quaternion rot,Vector3 aimPos)
        {
            // 上报当前位置到事件系统
            var report = new PawnSPRLReport(id,state,pos,rot,aimPos);
            EventSystem.Instance.Trigger(GameEvent.PAWN_POSITION_REPORT, report);
        }
        public void ReportPawnDamage(int id,int targetId,float damage,int source)
        {
            // 上报伤害到事件系统
            var report = new PawnDamageReport(id,targetId,damage,source);
            EventSystem.Instance.Trigger(GameEvent.PAWN_POSITION_REPORT, report);
        }
    }
}

