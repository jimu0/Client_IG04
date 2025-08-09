using UnityEngine;

public class WindTrailController : MonoBehaviour
{
    public ParticleSystem windParticles;
    public float followSpeed = 10f;
    public float spawnDistance = 0.5f;
    public float maxParticles = 100;
    
    private Camera mainCamera;
    private Vector3 targetPosition;
    private ParticleSystem.EmitParams emitParams;

    void Start()
    {
        mainCamera = Camera.main;
        var emission = windParticles.emission;
        emission.rateOverTime = 0; // 完全由脚本控制发射
    }

    void Update()
    {
        // 获取鼠标世界坐标
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // 与相机的距离
        targetPosition = mainCamera.ScreenToWorldPoint(mousePos);
        
        // 平滑移动到鼠标位置
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // 限制粒子数量
        if (windParticles.particleCount < maxParticles)
        {
            emitParams.position = transform.position + Random.insideUnitSphere * spawnDistance;
            emitParams.velocity = Random.insideUnitSphere * 1f;
            windParticles.Emit(emitParams, 1);
        }
    }
}