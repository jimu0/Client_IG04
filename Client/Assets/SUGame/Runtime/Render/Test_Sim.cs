using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using IGC.Engine;
using IGC.Game;
using UnityEngine;
using Observe;

public class Test_Sim : MonoBehaviour
{
    private string name;
    public int n;
    private double td;
    private Pulse pulse;
    private Totem[] totems;

    //private  worldSnapshot;
    
    private void Update()
    {
        Debug.Log($"血液：{Igc.GetWorldState.tick}");
        Debug.Log($"TestTxt:{Igc.GetWorldState.debugText}"); 
        //Debug.Log($"测试：时间({_state.GameMode.pulse.time}),待运行Totem数量:({_state.GameMode.pulse.queue.Count}),待运行内容：({idString})");
    }
    
    private void Start()
    {
        name = gameObject.name;
        //Simulate.RegisterListener
        //WorldSnapshot._Snapshot.TestTxt = "";
        
    }




    // public void OnSimUpdat(in IGC.Game.GameState _state)
    // {
    //     // if(n==IGC.Game.GameState.a)Debug.Log($"{name}:OnSimUpdat._state:{n}");
    //     // else if(n==IGC.Game.GameState.b)Debug.Log($"{name}:OnSimUpdat._state:{n}");
    //     // else if(n==IGC.Game.GameState.c)Debug.Log($"{name}:OnSimUpdat._state:{n}");
    //     // else Debug.Log($"{name}:OnSimUpdat._state:null");
    //     
    //     // if (_state.GameMode == null) return;
    //     // pulse = _state.GameMode.pulse;
    //     // totems = _state.GameMode.Totems;
    //     //
    //     //
    //     // int[] childrenIds = new int[8];
    //     // for (int i = 0; i < totems.Length; i++)
    //     // {
    //     //     int length = totems[i].children.Length;
    //     //     for (int j = 0; j < length; j++)
    //     //     {
    //     //         Totem a = totems[i].children[j] as Totem;
    //     //         //childrenIds[j] = a.id;
    //     //     }
    //     //     
    //     // }
    //     //Debug.Log($"测试：ID:{totems[i].id} 执行完成，儿子({string.Join(",", childrenIds)})准备");
    //     
    //     // Debug.Log($"g:{_state.GameMode?.exeContext.RemainingBudget}");
    //     // if (_state.GameMode != null)
    //     // {
    //     //     Totem totemAaa = _state.GameMode.Totems[8].targets[0] as Totem;
    //     //     if (totemAaa != null) Debug.Log($"AAAAAAAAAA:({totemAaa.id}");
    //     //     else Debug.Log($"AAAAAAAAAA:不存在");
    //     //     
    //     //     
    //     //     
    //     //     List<Totem> totems = new List<Totem>();
    //     //     foreach (ScheduledNode node in _state.GameMode.pulse.queue)
    //     //     {
    //     //         Totem totem = node.Node as Totem;
    //     //         totems.Add(totem);
    //     //     }
    //     //     
    //     //     string idString = string.Join(",", totems.Select(item => item?.id));
    //     //     
    //     //     Debug.Log($"测试：时间({_state.GameMode.pulse.time}),待运行Totem数量:({_state.GameMode.pulse.queue.Count}),待运行内容：({idString})");
    //
    //
    //       
    //     
    // }
}

