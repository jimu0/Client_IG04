using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;
using System.Collections.Generic;

public class BakeRuntimeAnimation : EditorWindow
{
    public GameObject rootObject;     // 场景角色
    public RuntimeAnimatorController controller; // 动态动画来源
    public float duration = 1f;
    public float frameRate = 30f;

    [MenuItem("Animation Rigging/Bake Runtime Animation")]
    static void Open() => GetWindow<BakeRuntimeAnimation>("动画运行采样器");

    void OnGUI()
    {
        rootObject = (GameObject)EditorGUILayout.ObjectField("根模型", rootObject, typeof(GameObject), true);
        controller = (RuntimeAnimatorController)EditorGUILayout.ObjectField("动画来源", controller, typeof(RuntimeAnimatorController), false);
        duration = EditorGUILayout.FloatField("时间(秒)", duration);
        frameRate = EditorGUILayout.FloatField("动画帧率", frameRate);
        if (GUILayout.Button("开始采样")) Bake();
    }

    void Bake()
    {
        // ---- 克隆一个可以模拟运行时的对象 ----
        GameObject clone = Instantiate(rootObject);
        clone.name = rootObject.name + "_BakeTemp";

        Animator animator = clone.GetComponentInChildren<Animator>();
        if (!animator) animator = clone.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        RigBuilder rig = clone.GetComponentInChildren<RigBuilder>();
        // 收集骨骼
        List<Transform> bones = new();
        Collect(clone.transform, bones);

        Dictionary<string, TransformCurves> curves = new();
        foreach (var b in bones)
        {
            string path = AnimationUtility.CalculateTransformPath(b, clone.transform);
            curves[path] = new TransformCurves();
        }

        // ---------- 开始 runtime 动画模拟 ----------
        float dt = 1f / frameRate;
        int totalFrames = Mathf.CeilToInt(duration / dt);

        for (int i = 0; i <= totalFrames; i++)
        {
            float t = i * dt;
            // 驱动 Animator（真正运行时动画）
            animator.Update(dt);
            // 让 Animation Rigging 求解
            if (rig != null)
            {
                #if UNITY_2023_1_OR_NEWER
                rig.Evaluate(dt);
                #else
                rig.Evaluate(0f);
                #endif
            }
            // 记录骨骼最终姿态
            foreach (var b in bones)
            {
                var path = AnimationUtility.CalculateTransformPath(b, clone.transform);
                curves[path].Record(t, b);
            }
        }

        // ---- 写入 AnimationClip ----
        AnimationClip newClip = new();
        newClip.frameRate = frameRate;
        foreach (var kv in curves) kv.Value.WriteToClip(newClip, kv.Key);

        string savePath = EditorUtility.SaveFilePanelInProject("保存采样片段", rootObject.name + "_RuntimeBake.anim", "anim", "");
        if (!string.IsNullOrEmpty(savePath))
        {
            AssetDatabase.CreateAsset(newClip, savePath);
            AssetDatabase.SaveAssets();
        }
        DestroyImmediate(clone);
        Debug.Log("动画片段采样完成：" + savePath);
    }

    void Collect(Transform t, List<Transform> list)
    {
        list.Add(t);
        foreach (Transform c in t) Collect(c, list);
    }

    class TransformCurves
    {
        public List<Keyframe> px = new(), py = new(), pz = new();
        public List<Keyframe> rx = new(), ry = new(), rz = new(), rw = new();
        public void Record(float t, Transform b)
        {
            Vector3 p = b.localPosition;
            Quaternion q = b.localRotation;
            px.Add(new Keyframe(t, p.x));
            py.Add(new Keyframe(t, p.y));
            pz.Add(new Keyframe(t, p.z));
            rx.Add(new Keyframe(t, q.x));
            ry.Add(new Keyframe(t, q.y));
            rz.Add(new Keyframe(t, q.z));
            rw.Add(new Keyframe(t, q.w));
        }

        public void WriteToClip(AnimationClip clip, string path)
        {
            clip.SetCurve(path, typeof(Transform), "m_LocalPosition.x", new AnimationCurve(px.ToArray()));
            clip.SetCurve(path, typeof(Transform), "m_LocalPosition.y", new AnimationCurve(py.ToArray()));
            clip.SetCurve(path, typeof(Transform), "m_LocalPosition.z", new AnimationCurve(pz.ToArray()));
            clip.SetCurve(path, typeof(Transform), "m_LocalRotation.x", new AnimationCurve(rx.ToArray()));
            clip.SetCurve(path, typeof(Transform), "m_LocalRotation.y", new AnimationCurve(ry.ToArray()));
            clip.SetCurve(path, typeof(Transform), "m_LocalRotation.z", new AnimationCurve(rz.ToArray()));
            clip.SetCurve(path, typeof(Transform), "m_LocalRotation.w", new AnimationCurve(rw.ToArray()));
        }
    }
}
