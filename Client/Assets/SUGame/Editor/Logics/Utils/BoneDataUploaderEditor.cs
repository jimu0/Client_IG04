using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoneDataUploader))]
public class BoneDataUploaderEditor : UnityEditor.Editor
{
    private BoneDataUploader _target;
    private void OnEnable()
    {
        _target = target as BoneDataUploader;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("重新绑定", GUILayout.MinHeight(30)))
        {
            SetRebinding();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    private void SetRebinding()
    {
        BoneDataUploader.OnBindTransArray();
    }
}
