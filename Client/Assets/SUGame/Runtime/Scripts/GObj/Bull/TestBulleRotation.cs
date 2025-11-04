using System.Collections;
using System.Collections.Generic;
using cfg;
using UnityEngine;

public class TestBulleRotation : MonoBehaviour
{
    
    void DrawTrajectoryBullet(Vector3 posStart,Vector3 posEnd)
    {
        Vector3 dir = (posEnd - posStart).normalized;
        Vector3 right = Camera.main ? Vector3.Cross(dir, (Camera.main.transform.position - posStart)).normalized : Vector3.right;
        if (right.sqrMagnitude < 1e-4f) right = Vector3.Cross(dir, Vector3.up);
        transform.SetPositionAndRotation(posStart, Quaternion.LookRotation(dir, Vector3.Cross(right, dir)) * Quaternion.Euler(0, -90, 0));
        transform.localScale = new Vector3(Vector3.Distance(posStart, posEnd), 1, 1);
    }
    
    // public Transform pointA;
    // public Transform pointB;
    // public Camera cam;
    // public float width = 1f;
    // public float r;
    
    // void Update()
    // {
    //
    //     if (!pointA || !pointB || !cam) return;
    //
    //     Vector3 A = pointA.position, B = pointB.position;
    //     Vector3 dir = (B - A).normalized;
    //     Vector3 right = Vector3.Cross(dir, (cam.transform.position - A)).normalized;
    //     if (right.sqrMagnitude < 1e-4f) right = Vector3.Cross(dir, Vector3.up);
    //
    //     transform.SetPositionAndRotation(
    //         A,
    //         Quaternion.LookRotation(dir, Vector3.Cross(right, dir)) * Quaternion.Euler(0, -90, 0)
    //     );
    //
    //     transform.localScale = new Vector3(Vector3.Distance(A, B), 1, width);
    //     
    // }
}
