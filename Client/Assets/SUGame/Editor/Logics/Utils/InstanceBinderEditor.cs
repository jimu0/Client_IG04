using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstanceBinder))]
public class InstanceBinderEditor : UnityEditor.Editor
{
    private InstanceBinder _target;
    
    private void OnEnable()
    {
        _target = target as InstanceBinder;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("重新绑定", GUILayout.MinHeight(30)))
        {
            SetRebinding();
        }
    }
    
    private void SetRebinding()
    {
        _target.OnBindTransArray();
        
        EditorUtility.SetDirty(_target);
        serializedObject.ApplyModifiedProperties();
        PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(_target.gameObject);
        if (status == PrefabInstanceStatus.Connected)
        {
            GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(_target.gameObject);
            if (root != null)
            {
                PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(root);
                if (assetType is PrefabAssetType.Regular or PrefabAssetType.Variant)
                {
                    PrefabUtility.ApplyPrefabInstance(root, InteractionMode.AutomatedAction);
                }
                else
                {
                    Debug.LogWarning("无法 Apply，因为这是 Model Prefab (FBX/OBJ)： " + root.name);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
