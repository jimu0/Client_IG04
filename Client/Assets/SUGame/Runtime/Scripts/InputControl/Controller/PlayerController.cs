using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using IGC.Engine;
//using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace SUGame.Runtime.Scripts.InputControl
{
    public class PlayerController : Controller, IPlayerController
    {

        //private TouchInputManager touchInputManager;
        //private Vector3 movePosition = Vector2.zero;
        //private Vector2 mousePosition = Vector2.zero;
        
        [SerializeField] private RectTransform pointerRectTransform; // 指针的RectTransform（如果是UI Image）
        [SerializeField] private Camera mainCamera;
        private GameObject camRoot;//相机，玩家控制器依赖镜头！
        private Plane yPlane = new(Vector3.up, new Vector3(0, 1, 0));
        private TextMeshProUGUI testPosTextMeshPro;
        
        //private Vector2 touchPos;
        //private Vector2 _screenCenter; // 屏幕中心点（动态更新）

        //public IPlayerController iPlayerController;
        private Vector2 moveValue; // 位移量
        private Vector2 aimValue; // 准星点
        public float moveSpeed; // 位移速度
        private float moveMobility=1; //移动性系数，单指因瞄准投掷加速等角色主动状态时的移动衰减/补偿/加乘系数
        ///private Vector3 movePos; // 玩家位置
        public Transform actorTsf; // 玩家模型Transform
        ///public Vector3 aimWorldPos; // 准星的世界位置
        private bool aimlock = false; //用于判定射击键未松开时取消精确瞄准而不缺换准心模式的情况
        
        private int fireRateTimerID; // 表示射击间隔的计时器ID
        
        private LineRenderer line;
        public Color color = Color.cyan; // 轨迹颜色
        public bool show = false; // 是否显示

        public Vector3 playerPos; // 玩家位置（发射点）
        public Vector3? targetPos; // 目标点（如目标敌人的位置，如果有）
        public Vector3? poi; // 子弹击中点(Point of impact的简称)

        public int firearmsType = 0;
        private bool fireTriggerState = false;
        private int fireLineRateTimerID;//测试用模拟子弹飞行生命周期表现的计时器ID

        public Material bulletFlowMaterial;//临时看弹道shader效果

        private CinemachineBrain cinemachineBrain;
        CinemachineVirtualCameraBase activeVcam;
        private void Awake()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            GameObject playerUIPanel = ResourceManager.LoadResSync<GameObject>("ArtUI_PlayerUIPanel");
            Instantiate(playerUIPanel, canvas.transform, false);
            
            
            // 注册交互接口
            TouchInputManager.Instance.iPlayerController = this;
            TimerSystem.Init();
            //fireRateTimerID = TimerManager.Register(0.1f, Fire, null, true, true, null);


            // // 初始化屏幕中心点（动态适配分辨率变化）
            // UpdateScreenCenter();
            // StartCoroutine(UpdateScreenCenterRoutine()); // 每帧检查分辨率变化

            pawnState = 1;
            
        }
        
        


        void OnEnable()
        {
            //touchInputManager.OnStartTouch += OnMove();
        }
        void OnDisable()
        {
            //touchInputManager.OnEndTouch -= OnMove();
        }
        private void OnDestroy()
        {
            // 销毁时取消注册，避免空引用或错误调用
            if(TouchInputManager.HasInstance) TouchInputManager.Instance.iPlayerController = null;
            
            StopAllCoroutines();
        }





        private Vector3 GetWorldPositionAtY(Vector2 screenPos)
        {
            Vector3 positionInFront = actorTsf.position + actorTsf.forward * 1; // 当鼠标空间转换射线无法与主平面相交时提供一个容错位置
            if (mainCamera == null) return positionInFront;
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            return yPlane.Raycast(ray, out float enter) ? ray.GetPoint(enter) : positionInFront;
        }






        public void SetMoveValue(Vec2 v)
        {
            moveValue.x = v.x;
            moveValue.y = v.y;
            //moveValue = v;
        }

        public void SetAimValue(Vec2 v)
        {
            aimValue.x = v.x;
            aimValue.y = v.y;
            //aimValue = v;
        }

        public void OnMove(bool v)
        {
            if (moveValue.sqrMagnitude < 0.01f) return;
            Vector3 direction = new Vector3(moveValue.x, 0, moveValue.y).normalized;
            pawnPos += direction * (moveSpeed * Time.deltaTime * moveMobility);
            //movePos.z += moveValue.y * (moveSpeed * Time.deltaTime) * moveMobility;
            
            //if(!v && !fireTriggerState) LookAtTarget(pawnPos); // 移动改变朝向的前提是玩家不在瞄准或射击状态以及判断是否有准星指在操作
            if(!v) LookAtTarget(pawnPos);//移动时候不再因为是否按下开火而固定方向，除非开火期间进行了精确瞄准
            
            transform.position = pawnPos;
        }

        public void OnAim(bool v)
        {
            if (v)
            {
                pointerRectTransform.anchoredPosition = aimValue; // 准星图标设置到鼠标位置
                //pointerRectTransform.anchoredPosition = new Vector2(Screen.width/2, Screen.height/2);
                pawnAimWorldPos = GetWorldPositionAtY(aimValue); // 获取鼠标的世界空间位置作为瞄准点
                //pawnAimWorldPos.x = playerPos.x + (aimValue.x-1920/2)/50;
                //pawnAimWorldPos.z = playerPos.z + (aimValue.y-1080/2)/50;
                aimlock = true;
                
                LookAtTarget(pawnAimWorldPos);
                moveMobility = 0.5f;
                
            }
            else
            {
                if (!aimlock)
                {
                    // 获取角色当前正前方数米的位置
                    Vector3 positionInFront = transform.position + actorTsf.forward * 50f;
                    Vector3 screenPoint = mainCamera.WorldToScreenPoint(positionInFront);
                    //screenPoint.y = 1;
                    pointerRectTransform.anchoredPosition = screenPoint;// 准星图标设置到角色前方位置
                    pawnAimWorldPos = positionInFront; // 获取角色前方这个距离的位置作为瞄准点
                }
                else
                {
                    //去掉这个else内的方法，只设置pointerRectTransform.anchoredPosition = aimValue;将成为基于旧位置的锁定射击
                    
                    pawnAimWorldPos = GetWorldPositionAtY(pointerRectTransform.anchoredPosition); // 获取鼠标的世界空间位置作为瞄准点
                    LookAtTarget(pawnAimWorldPos);
                }

                moveMobility = 1f;
            }
            if (fireTriggerState)
            {
                
            }
            else
            {
                aimlock = false;
            }

            pointerRectTransform.gameObject.SetActive(v);
            testPosTextMeshPro.text =$"{aimValue.x},{aimValue.y}";


        }

        public void OnFire()
        {
            if (firearmsType == 0)
            {
                Fire();
                if (fireTriggerState)
                {
                    TimerSystem.Cancel(fireRateTimerID);
                    fireRateTimerID = TimerSystem.Register(0.1f, Fire, null, true, true, null);
                }

                
            }
        }
        
        public void OnSkill()
        { 
            
        }

        private void Fire()
        {
            //ReportPawnDamage(pawnId,1,14,0);
            //Debug.Log("DaDaDaDaDa!");
            //LookAtTarget(aimWorldPos); 
            //DrawTrajectoryBullet(pawnState,pawnAimWorldPos);
            BullePool.BullProperty gObj = BullePool.bullPool.Get();
            BullePool.Set(gObj, 1, pawnPos, transform.position + actorTsf.forward * 50f);
            //DrawTrajectory();
            fireLineRateTimerID = TimerSystem.Register(0.1f, BulletFly, null, false, true, null);
            
        }

        private void fireUpdate(float obj)
        {
            OnAim(true);
        }

        private void fireUpdate()
        {
        }


        public void SetFireTriggerState(bool v)
        {
            fireTriggerState = v;
            if (!fireTriggerState) TimerSystem.Cancel(fireRateTimerID);
        }


        void Start()
        {

            line = gameObject.AddComponent<LineRenderer>();
            if (bulletFlowMaterial != null) line.sharedMaterial = bulletFlowMaterial;//"BulletFlowEffect_mat"
            
            if (mainCamera == null)
            {
                camRoot = GameStateManager.Instance.cameraRoot;
                //if(!mainCamera)Debug.Log(GameObject.Find("SplashController").name);
                mainCamera = camRoot.GetComponentsInChildren<Camera>()[0];
                pointerRectTransform = FindObjectOfType<Canvas>().GetComponentsInChildren<RectTransform>()[1];
            }
            //临时测试
            if (testPosTextMeshPro == null)
            {
                testPosTextMeshPro = GameObject.Find("Canvas").transform.Find("test_posText").GetComponent<TextMeshProUGUI>();
            }
            //testPosTextMeshPro.SetText($"{mousePosition.x},{mousePosition.y}");
            
            cinemachineBrain=mainCamera.GetComponent<CinemachineBrain>();
            
        }
        
        void Update()
        {
            
            
            //camRoot.transform.SetPositionAndRotation(pawnAimWorldPos,Quaternion.identity);
            camRoot.transform.SetPositionAndRotation(actorTsf.position,Quaternion.identity);
            
            playerPos = transform.position;
            poi = pawnAimWorldPos;//临时吧任何瞄准的位置都当做目标
            
            // float dis = Vector3.Distance(playerPos, targetPos)*0.02f;
            // cinemachineBrain.ActiveVirtualCamera.Follow.SetPositionAndRotation(cinemachineBrain.ActiveVirtualCamera.LookAt.position+new Vector3(0,22,-8)+new Vector3(0,22,-8)*dis,Quaternion.identity);
            //
            // if (cinemachineBrain.ActiveVirtualCamera.LookAt.position.y+22+22*dis>0)
            // {
            // }

            
            ReportPawnSPRL(pawnId,pawnState,pawnPos,pawnRotation,pawnAimWorldPos);//发送位置

            //Debug.Log(fireTriggerState ? "A" : "B");
            // if (show)
            // {
            //     DrawTrajectory();
            // }

        }
        
        private void LookAtTarget(Vector3 lookAtPos)
        {
            Vector3 direction = lookAtPos - transform.position;
            direction.Normalize();
            Vector3 worldDir = new (direction.x, 0, direction.z);
            Quaternion toRotation = Quaternion.LookRotation(worldDir, Vector3.up);
            pawnRotation = Quaternion.Lerp(actorTsf.rotation, toRotation, 100 * Time.deltaTime);
            actorTsf.rotation = pawnRotation;
        }


        void BulletFly()
        {
            //show = false;
            //line.enabled = show;
            TimerSystem.Cancel(fireLineRateTimerID);
        
        }
        
    }
}


