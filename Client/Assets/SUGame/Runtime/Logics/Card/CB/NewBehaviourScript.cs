using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    private Rigidbody rigidbody1;
    public GameObject gameObject1;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody1 = gameObject1.GetComponent<Rigidbody>();
        // 设置容量为1000活跃Tween + 200缓冲Tween（根据项目需求调整）
        DOTween.SetTweensCapacity(300, 100);
        
    }

    // Update is called once per frame
    void Update()
    {
        
        gameObject1.transform.DOMove(new Vector3(0, 0, 0), 0.2f).SetAutoKill(true);

        //transform.DOShakePosition(1,new Vector3(0.1f,0.1f,0.1f),3,1);
        //rigidbody.DOMove(new Vector3(2, 3, 4), 1);
        //transform.DOPunchScale(new Vector3(0.1f,0.1f,0.1f), 0.3f, 1, 0.1f);

        gameObject1.transform.DORestart();
        
        DOTween.Play(gameObject1.transform);
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            gameObject1.transform.DOPunchPosition(new Vector3(0.5f, 0.8f, 1f), 0.8f, 1, 0.5f).SetAutoKill(true);
        }
    }
    
}
