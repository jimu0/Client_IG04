using System;
using SUGame.Runtime.Scripts.InputControl;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerUIPanelBtn : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
{
    private IPlayerController iPlayerController;

    //private IPointerClickHandler pointerClickHandlerImplementation;
    public delegate void MyAction();

    public event MyAction Ddd;
    private void Awake()
    {

    }

    private void Start()
    {
        iPlayerController = TouchInputManager.Instance.iPlayerController;
        //Ddd += iPlayerController.OnFire;
    }

    // 当按钮被按下（鼠标/触摸按下）时调用
    public void OnPointerDown(PointerEventData eventData)
    {
        iPlayerController.SetFireTriggerState(true);
        Ddd?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        iPlayerController.SetFireTriggerState(false);
    }
}
