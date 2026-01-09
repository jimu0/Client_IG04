using System;
using System.Collections;
using System.Collections.Generic;
using SUGame.Runtime.Scripts.InputControl;
using SUGame.Simulation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel; ///
using Random = UnityEngine.Random;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using IGC.Engine;

//[DefaultExecutionOrder(-1)]
public class TouchInputManager : Singleton<TouchInputManager>
{
    [Header("UI References (可选，用于显示准心)")]
    public RectTransform crosshair; // 可拖拽的准心UI，需在Canvas下
    
    
    private float screenSplitX = 0.5f; // 0~1，左边为移动控制区，右边为准心控制区
    //public float screenSplitY = 0.5f; // 0~1，预留上下

    public Vec2 moveInput = Vec2.Zero;        // 用于角色移动的输入，范围 [-1, 1]
    private Vec2 crosshairPos; // 准心在屏幕上的位置（标准化 0~1）
    private int? moveFingerId = null; // 当前负责移动的手指ID
    private int? crosshairFingerId = null; // 当前负责准心的手指ID
    
    // 按钮区域（示例，可扩展为实际UI按钮检测）
    private bool isFiring = false;
    private bool isUsingSkill = false;

    public IPlayerController iPlayerController;
    private GraphicRaycaster raycaster;

    public GameInput gameInput = new GameInput();
    public RawInputSample s = new RawInputSample();
    
    // 注册一个可交互对象
    // public static void RegisterInteractable(IPlayerController interactable)
    // {
    //     iPlayerController = interactable;
    // }
    //
    // // 取消注册
    // public static void UnregisterInteractable(IPlayerController interactable)
    // {
    //     iPlayerController = null;
    // }

    // // 触发所有已注册的交互
    // public static void TriggerAllInteractions()
    // {
    //     foreach (var controller in iPlayerController)
    //     {
    //         //interactable.SetMoveValue();
    //     }
    // }
    
    /// <summary>
    /// 单例初始化完成后的自定义初始化
    /// </summary>
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        Debug.Log("[TouchInputManager] 触摸输入系统初始化完成");
    }

    void Awake()
    {
        raycaster = FindObjectOfType<GraphicRaycaster>();
        // 启用 EnhancedTouch（推荐，更精准的多点触控API）
        EnhancedTouchSupport.Enable();
        
        touchControls = new TouchControls();
        
        //PlayerController = gameObject.AddComponent<PlayerController>();
    }
    
    void Update()
    {
        HandleTouches(); // 检测触摸
        UpdateMovement(); // 更新位移量
        UpdateCrosshair(); // 更新准心坐标
        UpdateActions(); // 检测功能按钮
    }

    public GameInput Sample()
    {
        return gameInput;
    }


    void HandleTouches()
    {
        
        // TouchState fingerTouchState = touchControls.Touch.TouchInput.ReadValue<TouchState>();
        // Vector2 fingerPos = fingerTouchState.position;
        // float touchXNormalized = fingerPos.x / Screen.width; // 0~1
        // if (touchXNormalized < screenSplitX)
        // {
        //     moveFingerId = fingerTouchState.touchId;
        // }        
        // else
        // {
        //     crosshairFingerId = fingerTouchState.touchId;
        // }
        // Debug.LogWarning($"moveFingerId:{moveFingerId} , crosshairFingerId:{crosshairFingerId}");


        foreach (Touch touch in Touch.activeTouches)
        {
            // 构造 PointerEventData（需传入 EventSystem）
            PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            eventData.position = touch.startScreenPosition;
            bool isHitButton = IsTouchOnButton(eventData);
            // if (touch.phase == TouchPhase.Ended)
            // {
            //     
            // }

            if (isHitButton) continue;//触摸到按钮的指头跳过
            
            //Vector2 startScreenPos = touch.startScreenPosition;
            //float touchXNormalized = touch.startScreenPosition.x / Screen.width; // 0~1
            if (touch.startScreenPosition.x / Screen.width < 0.5f)
            {
                //if (moveFingerId != null) continue;
                moveFingerId ??= touch.finger.index;
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) HandleMoveInput(touch);
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    moveFingerId = null;
                    moveInput = Vec2.Zero;
                }
                //iPlayerController.UpdateMoveFingerStatus(moveFingerId);
            }
            else
            {
                //if (crosshairFingerId != null) continue;
                crosshairFingerId ??= touch.finger.index;
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) HandleCrosshairInput(touch.screenPosition);
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    crosshairFingerId = null;
                }
                //iPlayerController.UpdateCrosshairFingerStatus(crosshairFingerId);
            }
            //Debug.LogWarning($"moveFingerId:{moveFingerId} , crosshairFingerId:{crosshairFingerId}");
            
            
            
            
        }
        // foreach (Touch touch in Touch.activeTouches)
        // {
        //     
        //     Vector2 screenPos = touch.screenPosition;
        //     float touchXNormalized = screenPos.x / Screen.width; // 0~1
        //     if (moveFingerId.HasValue && moveFingerId.Value == touch.finger.index)
        //     {
        //         //Debug.LogWarning($"{touch.screenPosition}????????");
        //         // 如果这个手指是当前负责移动的，继续处理移动
        //         if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        //         {
        //             HandleMoveInput(touch.delta);
        //         }
        //         else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        //         {
        //             //moveInput = Vector2.zero;
        //             moveFingerId = null; // 移动手指抬起，取消跟踪
        //         }
        //     }
        //     else if (crosshairFingerId.HasValue && crosshairFingerId.Value == touch.finger.index)
        //     {
        //         // 如果这个手指是当前负责准心的
        //         if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        //         {
        //             HandleCrosshairInput(screenPos);
        //         }
        //         else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        //         {
        //             crosshairFingerId = null;
        //         }
        //     }
        //     else
        //     {
        //         // 新触摸点，判断它属于哪个功能区
        //         if (touch.phase == TouchPhase.Began)
        //         {
        //             if (touchXNormalized < screenSplitX)
        //             {
        //                 // 在左侧
        //             }
        //             else
        //             {
        //                 // 在右侧
        //             }
        //         }
        //     }
        // }
    }

    void HandleMoveInput(Touch touch)
    {
        float maxRadius = 10f; // 摇杆最大距离
        Vector2 touchOffset = touch.screenPosition - touch.startScreenPosition;
        Vec2 offset;
        offset.x = touchOffset.x;
        offset.y = touchOffset.y;
        float distance = offset.Length(); // 计算偏移距离
        if (distance > 0)
        {
            if (distance > maxRadius)
            {
                // 如果超出最大半径，则“拉回”到边界，保证方向不变，但长度合法
                offset = offset.Normalized() * maxRadius;
            }
            moveInput = offset.Normalized();
        }
        else
        {
            moveInput = Vec2.Zero;
        }

        //Debug.Log($"moveInput:{moveInput.x},{moveInput.y}");
    }
    
    void HandleCrosshairInput(Vector2 screenPos)
    {
        crosshairPos.x = screenPos.x;
        crosshairPos.y = screenPos.y;
        //crosshairPos = screenPos;
    }
    
    void UpdateMovement()
    {
        // if (iPlayerController != null)
        // {
        //     iPlayerController.SetMoveValue(moveInput);
        //     iPlayerController.OnMove(crosshairFingerId != null);
        // }
        // else
        // {
        //     //Debug.LogWarning($"iPlayerController为空！无法执行移动控制");
        // }
        if (gameInput == null) return;
        gameInput.moveValue = moveInput;
        gameInput.moving = crosshairFingerId != null;
        
    }

    void UpdateCrosshair()
    {
        //Android准心控制
        const float ax = 0.7f;
        const float ay = 0.1f;
        const float bx = 3f;
        const float by = 3f;
        
        Vec2 aimPos = Vec2.Zero;
        aimPos.x = (crosshairPos.x - Screen.width * ax) * (1 + bx);
        aimPos.y = (crosshairPos.y - Screen.height * ay) * (1 + by);
        if (aimPos.x < 0)aimPos.x = 0;
        else if(aimPos.x > Screen.width)aimPos.x = Screen.width;
        if (aimPos.y < 0)aimPos.y = 0;
        else if(aimPos.y > Screen.height)aimPos.y = Screen.height;

        // if (iPlayerController != null)
        // {
        //     iPlayerController.SetAimValue(aimPos);
        //     iPlayerController.OnAim(crosshairFingerId != null);
        // }
        // else
        // {
        //     //Debug.LogWarning($"iPlayerController为空！无法执行准心控制");
        // }
        
        if (gameInput == null) return;
        gameInput.aimValue = aimPos;
        gameInput.aiming = crosshairFingerId != null;
    }

    void UpdateActions()
    {
        
    }





    public delegate void StartTouchEvent(Vector2 position,float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position,float time);
    public event EndTouchEvent OnEndTouch;
    
    private TouchControls touchControls;
    
    
    // public void Awake()
    // {
    //     touchControls = new TouchControls();
    // }
    
    public void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        touchControls.Enable();
        TouchSimulation.Enable();
    
        //UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;///
    }
    
    public void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        touchControls.Disable();
        TouchSimulation.Disable();
    
        //UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;///
    }
    private void OnDestroy()
    {
        // 销毁时取消注册，避免空引用或错误调用
        //Destroy(gameObject);
        
    }
    
    /// <summary>
    /// 单例销毁时的清理工作
    /// </summary>
    protected override void OnSingletonDestroy()
    {
        base.OnSingletonDestroy();
        Destroy(gameObject);
        Debug.Log("[TouchInputManager] 触摸输入系统已清理");
    }

    //
    // private void Start()
    // {
    //     touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
    //     touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    //     
    //     
    // }
    //
    // private void FingerDown(Finger finger)
    // {
    //     if (OnStartTouch != null) OnStartTouch(finger.screenPosition, Time.time);///
    // }
    //
    // public void Update()
    // {
    //
    // }
    //
    private void StartTouch(InputAction.CallbackContext context)
    {
        Debug.Log($"触摸开始：{touchControls.Touch.TouchPosition.ReadValue<Vector2>()}");
        if (OnStartTouch != null)
            OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
    }
    private void EndTouch(InputAction.CallbackContext context)
    {
        Debug.Log($"触摸结束：{touchControls.Touch.TouchPosition.ReadValue<Vector2>()}");
        if (OnEndTouch != null)
            OnEndTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }


    
    
    
    
    
    // private void Look(Vector3 pos)
    // {
    //     Vector3 direction = pos - transform.position;
    //     direction.Normalize();
    //     Vector3 worldDir = new (direction.x, 0, direction.z);
    //     Quaternion toRotation = Quaternion.LookRotation(worldDir, Vector3.up);
    //     playerTsf.rotation = Quaternion.Lerp(playerTsf.rotation, toRotation, 100 * Time.deltaTime);
    // }
    // private void Move(Vector2 move)
    // {
    //     if (move.sqrMagnitude < 0.01)return;
    //     transform.position += new Vector3(move.x, 0, move.y) * (moveSpeed * Time.deltaTime);
    // }
    // private void LookAndMove(Vector3 lookPos,Vector2 move)
    // {
    //     Look(lookPos);
    //     Move(move);
    // }

    


    // private Vector3 GetWorldPositionAtY(Vector2 screenPos,Vector3 repairPos)
    // {
    //     if (mainCamera == null)return repairPos;
    //     Ray ray = mainCamera.ScreenPointToRay(screenPos);
    //     return yPlane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : repairPos;
    // }


    // 检测触摸点是否命中按钮（或指定UI元素）
    private bool IsTouchOnButton(PointerEventData eventData)
    {
        // 存储所有被射线命中的UI元素
        var results = new List<RaycastResult>();
        // 执行射线检测（仅检测UI层）
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);

        // 检查命中的元素是否包含按钮（可根据需求扩展为其他类型）
        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true; // 命中按钮
            }
        }
        return false; // 未命中按钮（空白区域）
    }
    
}
