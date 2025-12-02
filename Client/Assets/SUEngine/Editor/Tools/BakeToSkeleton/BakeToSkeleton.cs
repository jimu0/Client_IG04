// BakeToSkeleton.cs
// Unity Animation Rigging - Official Sample Code
// Requires Unity Animation Rigging package installed

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Animations.Rigging;

public class BakeToSkeleton : EditorWindow
{
    GameObject targetGO;
    Animator animator;
    AnimationClip sourceClip;
    string newClipName = "BakedAnimation";

    [MenuItem("Animation Rigging/Bake To Skeleton")]
    public static void ShowWindow()
    {
        GetWindow<BakeToSkeleton>("Bake To Skeleton");
    }

    void OnGUI()
    {
        GUILayout.Label("Bake Animation With Animation Rigging Constraints", EditorStyles.boldLabel);

        targetGO = (GameObject)EditorGUILayout.ObjectField("Target GameObject", targetGO, typeof(GameObject), true);

        if (targetGO != null)
            animator = targetGO.GetComponent<Animator>();

        using (new EditorGUI.DisabledScope(animator == null))
        {
            sourceClip = (AnimationClip)EditorGUILayout.ObjectField("Source Clip", sourceClip, typeof(AnimationClip), false);
            newClipName = EditorGUILayout.TextField("Output Clip Name", newClipName);

            if (GUILayout.Button("Bake Animation"))
            {
                Bake();
            }
        }
    }

    void Bake()
    {
        if (targetGO == null || animator == null || sourceClip == null)
        {
            Debug.LogError("Bake failed: Missing target, animator, or source clip.");
            return;
        }

        // Duplicate the clip so we don’t edit original
        AnimationClip bakedClip = new AnimationClip();
        EditorUtility.CopySerialized(sourceClip, bakedClip);

        bakedClip.name = newClipName;

        // Set up AnimationMode
        AnimationMode.StartAnimationMode();

        float frameRate = sourceClip.frameRate;
        float clipLength = sourceClip.length;
        int totalFrames = Mathf.RoundToInt(frameRate * clipLength);

        AnimationMode.BeginSampling();
        for (int i = 0; i <= totalFrames; i++)
        {
            float time = (float)i / frameRate;
            AnimationMode.SampleAnimationClip(targetGO, sourceClip, time);

            // Force rig evaluation
            var rigBuilder = targetGO.GetComponent<RigBuilder>();
            if (rigBuilder != null)
            {
                rigBuilder.Build();
                rigBuilder.Evaluate(0);//逐帧采样与烘焙时填0
            }

            // Bake each transform in the skeleton
            BakeTransforms(animator, bakedClip, time);
        }
        AnimationMode.EndSampling();

        AnimationMode.StopAnimationMode();

        AssetDatabase.CreateAsset(bakedClip, $"Assets/{newClipName}.anim");
        AssetDatabase.SaveAssets();

        Debug.Log($"Baked clip saved as Assets/{newClipName}.anim");
    }

    void BakeTransforms(Animator animator, AnimationClip bakedClip, float time)
    {
        foreach (var bone in animator.GetComponentsInChildren<Transform>())
        {
            string path = AnimationUtility.CalculateTransformPath(bone, animator.transform);

            // Position
            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.x"),
                SetKeyframe(bakedClip, path, "m_LocalPosition.x", bone.localPosition.x, time)
            );

            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.y"),
                SetKeyframe(bakedClip, path, "m_LocalPosition.y", bone.localPosition.y, time)
            );

            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalPosition.z"),
                SetKeyframe(bakedClip, path, "m_LocalPosition.z", bone.localPosition.z, time)
            );

            // Rotation
            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.x"),
                SetKeyframe(bakedClip, path, "m_LocalRotation.x", bone.localRotation.x, time)
            );

            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.y"),
                SetKeyframe(bakedClip, path, "m_LocalRotation.y", bone.localRotation.y, time)
            );

            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.z"),
                SetKeyframe(bakedClip, path, "m_LocalRotation.z", bone.localRotation.z, time)
            );

            AnimationUtility.SetEditorCurve(
                bakedClip,
                EditorCurveBinding.FloatCurve(path, typeof(Transform), "m_LocalRotation.w"),
                SetKeyframe(bakedClip, path, "m_LocalRotation.w", bone.localRotation.w, time)
            );
        }
    }

    AnimationCurve SetKeyframe(AnimationClip clip, string path, string property, float value, float time)
    {
        var curve = AnimationUtility.GetEditorCurve(clip, new EditorCurveBinding
        {
            path = path,
            propertyName = property,
            type = typeof(Transform)
        });

        if (curve == null)
            curve = new AnimationCurve();

        curve.AddKey(new Keyframe(time, value));
        return curve;
    }
}
