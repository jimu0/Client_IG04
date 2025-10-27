using UnityEngine;

public readonly struct PawnSPRLReport
{
    public readonly int pawnId;     // 角色唯一标识，比如 0~59
    public readonly int pawnState;  //角色状态
    public readonly Vector3 pawnPos; // 角色当前上报的位置
    public readonly Quaternion pawnRotation; // 角色当前上报的朝向
    public readonly Vector3 pawnAimWorldPos; // 角色当前上报的准星点位置
    public PawnSPRLReport(int pawnId, int pawnState,Vector3 pawnPos,Quaternion pawnRotation, Vector3 pawnAimWorldPos)
    {
        this.pawnId = pawnId;
        this.pawnState = pawnState;
        this.pawnPos = pawnPos;
        this.pawnRotation = pawnRotation;
        this.pawnAimWorldPos = pawnAimWorldPos;
    }
}