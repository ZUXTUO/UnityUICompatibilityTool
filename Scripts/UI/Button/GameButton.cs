using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine;
/// <summary>
/// 1、单击
/// 2、双击
/// 3、长按
/// </summary> 
public class GameButton : Selectable, IPointerClickHandler, ISubmitHandler, IEventSystemHandler
{
    [Serializable]
    /// <summary>
    /// 单击时触发的事件委托
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }

    // 单击时触发的事件委托
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
    protected GameButton()
    { }
    //单击
    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }
    private void Press()
    {
        if (!IsActive() || !IsInteractable())
            return;
        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke();
    }

    [Serializable]
    // 按钮单击事件的函数定义
    public class ButtonLongPressEvent : UnityEvent { }
    //长按
    public float SetLongPressTime = 0.6f;
    [FormerlySerializedAs("onLongPress")]
    [SerializeField]
    public ButtonLongPressEvent m_onLongPress = new ButtonLongPressEvent();
    public ButtonLongPressEvent onLongPress
    {
        get { return m_onLongPress; }
        set { m_onLongPress = value; }
    }
    //双击
    [FormerlySerializedAs("OnDoubleClick")]
    public ButtonClickedEvent m_onDoubleClick = new ButtonClickedEvent();
    public ButtonClickedEvent OnDoubleClick
    {
        get { return m_onDoubleClick; }
        set { m_onDoubleClick = value; }
    }
    //结束
    [FormerlySerializedAs("EndClick")]
    public ButtonClickedEvent m_EndClick = new ButtonClickedEvent();
    public ButtonClickedEvent EndClick
    {
        get { return m_EndClick; }
        set { m_EndClick = value; }
    }

    private bool SetIsStartPress = false;
    private float SetCurPointDownTime = 0f;
    private bool SetLongPressTrigger = false;

    void Update()
    {
        CheckIsLongPress();
    }
    //长按
    void CheckIsLongPress()
    {
        if (SetIsStartPress && !SetLongPressTrigger)
        {
            if (Time.time > SetCurPointDownTime + SetLongPressTime)
            {
                SetLongPressTrigger = true;
                SetIsStartPress = false;
                if (m_onLongPress != null)
                {
                    m_onLongPress.Invoke();
                }
            }
        }
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        //(避免已经点击进入长按后，抬起的情况)
        if (!SetLongPressTrigger)
        {
            // 正常单击
            if (eventData.clickCount == 2)
            {
                eventData.clickCount = 1;//回到第一
                if (m_onDoubleClick != null)
                {
                    m_onDoubleClick.Invoke();
                }

            }// 双击
            else if (eventData.clickCount == 1)
            {
                onClick.Invoke();
            }
        }
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        Press();
        // 如果"按下"被设置为禁用
        // 不要运行协程
        if (!IsActive() || !IsInteractable())
            return;
        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        DoStateTransition(currentSelectionState, false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // 按下刷新当前时间
        base.OnPointerDown(eventData);
        SetCurPointDownTime = Time.time;
        SetIsStartPress = true;
        SetLongPressTrigger = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // 指针抬起，结束开始长按
        base.OnPointerUp(eventData);
        SetIsStartPress = false;
        m_EndClick.Invoke();//抬起
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        // 指针移出，结束开始长按，计时长按标志
        base.OnPointerExit(eventData);
        SetIsStartPress = false;
    }
}