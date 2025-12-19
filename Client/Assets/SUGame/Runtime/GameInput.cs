using SUGame.Simulation;

public class GameInput
{
    public TouchInputManager touchInputManager;


    public Vec2 moveValue = Vec2.Zero;
    public Vec2 aimValue = Vec2.Zero;
    public bool moving;
    public bool aiming;
    public bool firing;
    //public bool hdSkillKey;
    
    //
    // public void OnMove(bool v)
    // {
    //     if (moveValue.sqrMagnitude < 0.01f) return;
    //     Vector3 direction = new Vector3(moveValue.x, 0, moveValue.y).normalized;
    //     pawnPos += direction * (moveSpeed * Time.deltaTime * moveMobility);
    //     //movePos.z += moveValue.y * (moveSpeed * Time.deltaTime) * moveMobility;
    //
    //     //if(!v && !fireTriggerState) LookAtTarget(pawnPos); // 移动改变朝向的前提是玩家不在瞄准或射击状态以及判断是否有准星指在操作
    //     if (!v) LookAtTarget(pawnPos); //移动时候不再因为是否按下开火而固定方向，除非开火期间进行了精确瞄准
    //
    //     transform.position = pawnPos;
    // }
    //
    // public void OnAim(bool v)
    // {
    //     if (v)
    //     {
    //         pointerRectTransform.anchoredPosition = aimValue; // 准星图标设置到鼠标位置
    //         //pointerRectTransform.anchoredPosition = new Vector2(Screen.width/2, Screen.height/2);
    //         pawnAimWorldPos = GetWorldPositionAtY(aimValue); // 获取鼠标的世界空间位置作为瞄准点
    //         //pawnAimWorldPos.x = playerPos.x + (aimValue.x-1920/2)/50;
    //         //pawnAimWorldPos.z = playerPos.z + (aimValue.y-1080/2)/50;
    //         aimlock = true;
    //
    //         LookAtTarget(pawnAimWorldPos);
    //         moveMobility = 0.5f;
    //
    //     }
    //     else
    //     {
    //         if (!aimlock)
    //         {
    //             // 获取角色当前正前方数米的位置
    //             Vector3 positionInFront = transform.position + actorTsf.forward * 50f;
    //             Vector3 screenPoint = mainCamera.WorldToScreenPoint(positionInFront);
    //             //screenPoint.y = 1;
    //             pointerRectTransform.anchoredPosition = screenPoint; // 准星图标设置到角色前方位置
    //             pawnAimWorldPos = positionInFront; // 获取角色前方这个距离的位置作为瞄准点
    //         }
    //         else
    //         {
    //             //去掉这个else内的方法，只设置pointerRectTransform.anchoredPosition = aimValue;将成为基于旧位置的锁定射击
    //
    //             pawnAimWorldPos = GetWorldPositionAtY(pointerRectTransform.anchoredPosition); // 获取鼠标的世界空间位置作为瞄准点
    //             LookAtTarget(pawnAimWorldPos);
    //         }
    //
    //         moveMobility = 1f;
    //     }
    //
    //     if (fireTriggerState)
    //     {
    //
    //     }
    //     else
    //     {
    //         aimlock = false;
    //     }
    //
    //     pointerRectTransform.gameObject.SetActive(v);
    //     testPosTextMeshPro.text = $"{aimValue.x},{aimValue.y}";
    //
    //
    // }
    //
    // public void OnFire()
    // {
    //     if (firearmsType == 0)
    //     {
    //         Fire();
    //         if (fireTriggerState)
    //         {
    //             TimerSystem.Cancel(fireRateTimerID);
    //             fireRateTimerID = TimerSystem.Register(0.1f, Fire, null, true, true, null);
    //         }
    //
    //
    //     }
    // }
    //
    // public void OnSkill()
    // {
    //
    // }
}
