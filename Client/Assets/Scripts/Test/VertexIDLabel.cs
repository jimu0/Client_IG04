//这个脚本能够查看单签模型的顶点id排序

using UnityEngine;

public class VertexIDLabel : MonoBehaviour
{
    public int id;
    void OnDrawGizmos()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (!mf || !mf.sharedMesh) return;
        Gizmos.color = Color.cyan;
        
        DrawPosId(mf.sharedMesh.vertices,id);
        // for (int i = 0; i < mf.sharedMesh.vertices.Length; i++)
        // {
        //     DrawPosId(mf.sharedMesh.vertices,i);
        // }
    }

    void DrawPosId(Vector3[] vertices,int i)
    {
        if (i < 0 || i >= vertices.Length) return;
        Vector3 worldPos = transform.TransformPoint(vertices[i]);
        Gizmos.DrawSphere(worldPos, 0.02f);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(worldPos, $"  {i}");
#endif
    }
}