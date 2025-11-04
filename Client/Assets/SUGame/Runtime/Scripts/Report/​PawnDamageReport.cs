using UnityEngine;

public class PawnDamageReport
{
    public readonly int pawnId; // 角色唯一标识
    public readonly int targetPawnId; // 目标角色唯一标识
    public readonly float damageValue; // 伤害量
    public readonly int damageSourceId; // 伤害来源
    public PawnDamageReport(int pawnId, int targetPawnId,float damageValue,int damageSourceId)
    {
        this.pawnId = pawnId;
        this.targetPawnId = targetPawnId;
        this.damageValue = damageValue;
        this.damageSourceId = damageSourceId;
    }
}
