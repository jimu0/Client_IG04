using System.Collections;
using System.Collections.Generic;
using SUGame.Runtime.Logics.Utils.ObjectUtils;
using UnityEngine;

public class BullePool : MonoBehaviour
{
    private ObjectPool<GameObject> bullPool;
    public GameObject bullObja;
    private List<GameObject> bullObjas = new List<GameObject>();
    
    void Start()
    {
        bullPool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestory,
            true, 10, 30);
    }
    GameObject OnCreate()
    {
        //return new GameObject("gObj");
        GameObject bull = Instantiate(bullObja, transform);
        bullObjas.Add(bull);
        return bull;
    }
    void OnGet(GameObject gObj)
    {
        Debug.Log("pool:获取");
        gObj.SetActive(true);
    }
    void OnRelease(GameObject gObj)
    {
        Debug.Log("pool:释放");
        gObj.SetActive(false);
            
    }
    void OnDestory(GameObject gObj)
    {
        Debug.Log("pool:销毁");
    }
}
