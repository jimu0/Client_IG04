using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Universse : MonoBehaviour
{
    public string universseDataPath = "Assets/Resources/universse.bytes"; 
    // Start is called before the first frame update
    void Start()
    {
        UniversseSys.Instance.SetWorldValues();
        //UniversseSys.Instance.SaveWorldArrayToBytes(universseDataPath);
        UniversseSys.Instance.LoadWorldBytesToArray(universseDataPath);
        // 测试打印前10个值
        for (int i = 0; i < Mathf.Min(10, UniversseSys.Instance.world.Length); i++)
        {
            Debug.Log($"World[{i}] = {UniversseSys.Instance.world[i]}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
