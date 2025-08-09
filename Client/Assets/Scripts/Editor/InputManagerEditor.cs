using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("打印所有绑定"))
        {
            ((InputManager)target).LogAllBinds();
        }
    }
}
#endif
