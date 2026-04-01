using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : MonoBehaviour
{
    public Animator animator;
    public Transform meshTsf;
    public float speed;
    
    void Awake()
    {
        meshTsf = transform.Find("PawnMesh");
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        animator.SetFloat("speed", speed);
    }
}
