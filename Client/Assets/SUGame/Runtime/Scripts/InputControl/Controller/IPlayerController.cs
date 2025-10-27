using UnityEngine;

public interface IPlayerController
{
    public void SetMoveValue(Vector2 v);
    public void SetAimValue(Vector2 v);
    //public void UpdateMoveFingerStatus(int? moveFinger);
    //public void UpdateCrosshairFingerStatus(int? crosshairFinger);
    public void OnMove(bool v);
    public void OnAim(bool v);
    public void OnFire();
    public void SetFireTriggerState(bool v);
    public void OnSkill();
}