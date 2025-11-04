using System;
using System.Collections;
using System.Collections.Generic;
using SUGame.Runtime.Logics.Utils.ObjectUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BullePool : MonoBehaviour
{
     public static Camera cam;

    public class BullProperty
    {
        public GameObject obj = null;
        public MeshRenderer meshrender;
        public MaterialPropertyBlock propBlock;
        public int id;
        public float lifeCycle = 0.1f;
        public float speed = 10;
        public int timerID = 0;
        public Vector3 posStart = Vector3.back;
        public Vector3 posEnd = Vector3.back;
        public float distance = 1;
    }
    private static int _StretchValueID= Shader.PropertyToID("_StretchValue");
    private static int _ElapsedTimeID = Shader.PropertyToID("_ElapsedTime");
    private static int _EmissionID = Shader.PropertyToID("_Emission");
    
    public static ObjectPool<BullProperty> bullPool;
    public GameObject bullObja;
    private List<BullProperty> bullObjas = new List<BullProperty>();

    //public int timerAAA = 777;

    private void Awake()
    {
        TimerManager.Init();
    }

    void Start()
    {
        cam = Camera.main;
        bullPool = new ObjectPool<BullProperty>(OnCreate, OnGet, OnRelease, OnDestory,
            true, 10, 30);
    }

    public static void Set(BullProperty gObj, int id, Vector3 posStart, Vector3 posEnd)
    {
        gObj.id = id;
        gObj.posStart = posStart;
        gObj.posEnd = posEnd;
        gObj.distance = Vector3.Distance(posStart, posEnd);
        gObj.lifeCycle = (0.0667f * gObj.distance / gObj.speed) / Time.timeScale;//16纹理尺寸-1为了走完尾部
        gObj.timerID = TimerManager.Register(gObj.lifeCycle, () => { bullPool.Release(gObj);}, null, false, true);
        DrawTrajectoryBullet(gObj);
        SetBulleFlowEffect(gObj);
    }

    BullProperty OnCreate()
    {
        BullProperty bull = new();
        bull.obj = Instantiate(bullObja, transform);
        bull.meshrender = bull.obj.GetComponent<MeshRenderer>();
        bull.propBlock = new MaterialPropertyBlock();
        bull.meshrender.SetPropertyBlock(bull.propBlock);
        bullObjas.Add(bull);
        //bullObjas[^1].id = bullObjas.Count - 1;
        return bull;
    }
    void OnGet(BullProperty gObj)
    {
        Debug.Log("pool:获取");
        gObj.obj.SetActive(true);
    }
    void OnRelease(BullProperty gObj)
    {
        Debug.Log("pool:释放");
        gObj.obj.SetActive(false);
            
    }
    void OnDestory(BullProperty gObj)
    {
        Debug.Log("pool:销毁");
    }
    
    
    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            bullPool.Release(bullObjas[0]);

        }

        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            BullProperty gObj = bullPool.Get();
            Set(gObj, 1, Vector3.back, Vector3.right * 30);
        }
    }
    // private IEnumerator SpawnOnNextFrameCoroutine()
    // {
    //     // 等待一帧
    //     yield return null;
    //
    //     BullProperty gObj = bullPool.Get();
    //     Set(gObj, 1, Vector3.back, Vector3.right * 30);
    // }
    
    
    public static void DrawTrajectoryBullet(BullProperty gObj)
    {
        Vector3 dir = (gObj.posEnd - gObj.posStart).normalized;
        Vector3 right = cam ? Vector3.Cross(dir, (cam.transform.position - gObj.posStart)).normalized : Vector3.left;
        if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(dir, Vector3.up);
        gObj.obj.transform.SetPositionAndRotation(gObj.posStart, Quaternion.LookRotation(dir, Vector3.Cross(right, dir)) * Quaternion.Euler(0, -90, 0));
        gObj.obj.transform.localScale = new Vector3(gObj.distance, 1, 1);
    }

    public static void SetBulleFlowEffect(BullProperty gObj)
    {
        //gObj.propBlock ??= new MaterialPropertyBlock();
        gObj.meshrender.GetPropertyBlock(gObj.propBlock);  // 获取当前的 PropertyBlock
        // 设置实例化参数
        gObj.propBlock.SetFloat(_StretchValueID, gObj.distance);
        gObj.propBlock.SetFloat(_ElapsedTimeID, Time.time);
        gObj.propBlock.SetVector(_EmissionID, RandomColor());
        
        gObj.meshrender.SetPropertyBlock(gObj.propBlock);  // 应用到当前物体
    }
    
    public static Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }
}
