using System;
using UnityEngine;

public class TestTouch : MonoBehaviour
{

    private TouchInputManager touchInputManager;
    private Camera cameraMain;
    
    private void Awake()
    {
        touchInputManager = TouchInputManager.Instance;
        cameraMain = Camera.main;
    }

    private void OnEnable()
    {
        touchInputManager.OnStartTouch += Move;
        
    }
    private void OnDisable()
    {
        touchInputManager.OnEndTouch -= Move;
    }

    public void Move(Vector2 screenPosition,float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, cameraMain.nearClipPlane);
        Vector3 worldCoordinated = cameraMain.ScreenToWorldPoint(screenCoordinates);
        screenCoordinates.z = 0;
        transform.position = worldCoordinated;
    }
}
