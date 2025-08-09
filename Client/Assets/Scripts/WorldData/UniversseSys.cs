using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UniversseSys
{
    private static UniversseSys instance;
    public static UniversseSys Instance => instance ??= new UniversseSys();
    
    private int entity => posId * SpatialPointer + typeId;
    public int typeId;//单位类型，上限是999个类型(不包括0)
    public int posId;//空间位置，上限是2073600个位置
    private const int SpaceMax = 2073600;//空间最大尺寸
    public int[] world = new int[SpaceMax];//数据世界

    private const int Size = 1440;//将空间2维化的最大尺寸
    private const int SpatialPointer = 1000;//从千位数截断以定义类型与空间

    
    public void SaveWorldArrayToBytes(string filePath)
    {
        // 确保目录存在
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        using (BinaryWriter writer = new(File.Open(filePath, FileMode.Create)))
        {
            //writer.Write(mainCs.Length); //写入数组长度
            foreach (int value in world) writer.Write(value);
        }
        Debug.Log("世界数据已保存: " + filePath);
    }
    
    public void LoadWorldBytesToArray(string filePath)
    {
        try
        {
            // 读取字节数据
            byte[] byteData = File.ReadAllBytes(filePath);
            // 检查字节长度是否能正确转换为int数组
            if (byteData.Length % sizeof(int) != 0)
            {
                Debug.LogError("字节数据长度不是4的倍数，无法正确转换为int数组");
                return;
            }
            // 将字节数组转换为int数组
            Buffer.BlockCopy(byteData, 0, world, 0, byteData.Length);
            Debug.Log($"成功加载数据，数组长度: {world.Length}");
        }
        catch (Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
        }
    }
    
    // 示例用法
    public void SetWorldValues()
    {
        // 初始化数组（示例）
        for (int i = 0; i < world.Length; i++)
        {
            world[i] = i % 100; // 随便填些数据
        }
    }
}

