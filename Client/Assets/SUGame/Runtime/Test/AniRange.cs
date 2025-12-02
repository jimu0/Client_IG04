using UnityEngine;

public class AniRange : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        
        if (!animator) return;
        // 随机动画开始位置（0~1）
        animator.Play("run", 0, Random.Range(0f, 1f));
        // 每个NPC动画播放速度随机（细微差异）
        animator.speed = Random.Range(0.8f, 1.2f);
    }
}
