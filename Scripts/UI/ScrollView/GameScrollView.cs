using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public class OnScrollMoveEvent : UnityEvent<float> { }
    [NonSerialized]
    public OnScrollMoveEvent OnScrollMove = new OnScrollMoveEvent();
    public RectTransform father;
    GridLayoutGroup gridLayoutGroup;
    float totalSizeY;
    [SerializeField]
    float curValue;
    public float CurValue
    {
        get { return father.anchoredPosition.y / totalSizeY; }
        set
        {
            SetScrollY(value);
            curValue = value;
        }
    }
    public float minValue;
    public float maxValue;

    [Header("滑动速度倍率")]
    public float times = 100f;
    [Header("摩擦系数")]
    public float reduceTimes = 0.01f;
    [Header("滑动系数")]
    public float addTimes = 1f;

    public void Init()
    {
        gridLayoutGroup = father.GetComponent<GridLayoutGroup>();

        totalSizeY = -(gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * (gridLayoutGroup.transform.childCount);

        minValue = totalSizeY;
        maxValue = 0;
    }
    Vector3 tempWordPos;
    Vector3 tempLocalPos;
    public void SetScrollY(float value)
    {

        StopCoroutine("SlideByInertia");
        father.anchoredPosition = new Vector2(father.anchoredPosition.x, totalSizeY * value);
        //tempLocalPos = new Vector3(father.localPosition.x, totalSizeY * value, father.localPosition.z);
        //if (RectTransformUtility.ScreenPointToWorldPointInRectangle(father, tempLocalPos, Camera.main, out tempWordPos))
        //{
        //    father.position =new Vector3(father.position.x, tempWordPos.y, father.position.z);
        //    OnScrollMove.Invoke(curValue);
        //}
        //father.position=new Vector3(father.position.x, totalSizeY * value - Screen.height, father.position.z);
    }

    private void LateUpdate()
    {
        if (father.anchoredPosition.y <= minValue)
        {
            father.anchoredPosition = new Vector2(father.anchoredPosition.x, minValue);

        }

        if (father.anchoredPosition.y >= maxValue)
        {
            father.anchoredPosition = new Vector2(father.anchoredPosition.x, maxValue);
        }

    }


    Vector3 startPoint;
    Vector3 endPoint;

    Vector3 globalMousePos;
    float offsetY;

    // begin dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        StopCoroutine("SlideByInertia");
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(father, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            startPoint = globalMousePos;

        }

    }

    // during dragging
    public void OnDrag(PointerEventData eventData)
    {

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(father, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            endPoint = globalMousePos;
            offsetY = endPoint.y - startPoint.y;
            father.anchoredPosition += new Vector2(0, offsetY * times);
            startPoint = globalMousePos;
            OnScrollMove.Invoke(CurValue);
        }


    }

    // end dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        startPoint = globalMousePos;
        endPoint = globalMousePos;
        StopCoroutine("SlideByInertia");
        StartCoroutine("SlideByInertia");
    }

    //是否大于0
    bool overZero;
    //惯性滑动
    IEnumerator SlideByInertia()
    {

        if (offsetY == 0)
        {
            StopCoroutine("SlideByInertia");
        }
        overZero = offsetY > 0;
        while (true)
        {

            if (father.anchoredPosition.y <= minValue)
            {
                father.anchoredPosition = new Vector2(father.anchoredPosition.x, minValue);
                break;
            }

            if (father.anchoredPosition.y >= maxValue)
            {
                father.anchoredPosition = new Vector2(father.anchoredPosition.x, maxValue);
                break;
            }
            offsetY -= overZero ? reduceTimes : -reduceTimes;
            offsetY = Mathf.Clamp(offsetY, -addTimes, addTimes);
            father.anchoredPosition += new Vector2(0, offsetY * times);
            OnScrollMove.Invoke(CurValue);
            if (overZero && offsetY <= 0)
            {
                break;
            }
            if (!overZero && offsetY >= 0)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        StopCoroutine("SlideByInertia");

    }

}