using System.Collections.Generic;
using IGC.CardCore_IG04;
using IGC.RPGCore_IG04;
using UnityEngine;
using Mycelia;
using SUEngine;
using UnityEngine.InputSystem;

public class SimBridge : MonoBehaviour
{
    [SerializeField, SerializeReference] public List<GameObject> instanceSys;
    private readonly List<ISim> SimSys = new();
    private readonly List<IRender> renderSys = new();
    
    //BinaryStateSerializer  serializer = new BinaryStateSerializer();//触摸相关
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        //MC.InitTables(new LubanConfigService());//初始化LuBan配置
        
        MC.listSimSys.Add(new RPGMode());
        //收集Mycelia支持引擎ISim、IRender接口的实例
        foreach (var item in instanceSys)
        {
            if(HasInterface<ISim>(item)) SimSys.Add(item.GetComponent<ISim>());
            if(HasInterface<IRender>(item)) renderSys.Add(item.GetComponent<IRender>());
        }
        MC.listSimSys.AddRange(SimSys);
        MC.listRenderSys.AddRange(renderSys);
        MC.Simulate_Awake();
    }
    void Start()
    {
        MC.Simulate_Start();
    }
    void Update()
    {
        PlayerInput();
        MC.Simulate_Update();
    }


    /// <summary>
    /// Mycelia和游戏Core没有提供输入方法，这里由Unity的TouchControls提供输入，由InputManager管理
    /// </summary>
    void PlayerInput()
    {
        var moveValue = InputManager.Instance.moveValue;
        MC.Input.SetMove(moveValue.x,moveValue.y);
    }
    
    
    bool HasInterface<T>(GameObject go) where T : class
    {
        return go.GetComponent(typeof(T)) != null;
    }
}
