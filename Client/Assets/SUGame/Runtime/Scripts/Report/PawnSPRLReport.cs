using UnityEngine;

public struct PawnSPRLReport
{
    public int pawnId;     // 角色唯一标识，比如 0~59
    public int pawnState;  //角色状态
    public Vector3 pawnPos; // 角色当前上报的位置
    public Quaternion pawnRotation; // 角色当前上报的朝向
    public Vector3 pawnAimWorldPos; // 角色当前上报的准星点位置
    public PawnSPRLReport(int pawnId, int pawnState,Vector3 pawnPos,Quaternion pawnRotation, Vector3 pawnAimWorldPos)
    {
        this.pawnId = pawnId;
        this.pawnState = pawnState;
        this.pawnPos = pawnPos;
        this.pawnRotation = pawnRotation;
        this.pawnAimWorldPos = pawnAimWorldPos;
    }
}