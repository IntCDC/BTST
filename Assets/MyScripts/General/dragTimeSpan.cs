using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class dragTimeSpan : MonoBehaviour,IEndDragHandler,IDragHandler
{

    #region Required Variables
    private RectTransform timeSpanSlider;
    float originalXStart =/*-684f*/-310;
    float originalXEnd = /*746f*/333f;
    Vector3 prev_pos;
    public TextMeshProUGUI text_time;
    public TimeLine timeLineSlider;
    float totalMinutes;
    float differenceTimeSpan;
    public float currentMinute;
    public UnityEvent spanDragged;
    #endregion
    private void Awake()
    {
        
        timeSpanSlider = GetComponent<RectTransform>();
        prev_pos = timeSpanSlider.transform.localPosition;
        totalMinutes = 480f;
        differenceTimeSpan = originalXEnd - originalXStart;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!(timeSpanSlider.offsetMin.x >= originalXStart && timeSpanSlider.offsetMin.x <= originalXEnd))
        {
            timeSpanSlider.transform.localPosition = prev_pos;
        }


    }

    public void OnDrag(PointerEventData eventData)
    {
      
        
        if (timeSpanSlider.transform.localPosition.x>=originalXStart && timeSpanSlider.transform.localPosition.x <=originalXEnd)
        {
            timeSpanSlider.position = new Vector3(eventData.position.x, timeSpanSlider.position.y, timeSpanSlider.position.z);                      
        }
        
        currentMinute = ((totalMinutes * 1000f) / (differenceTimeSpan)) * Math.Abs(timeSpanSlider.transform.localPosition.x - originalXStart) ;
        text_time.text = changeIntoTimeFormat(currentMinute);
        spanDragged.Invoke();

    }

    private string changeIntoTimeFormat(float currentMinute)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(currentMinute);
        string answer = string.Format("{0:D2}m:{1:D2}s:{2:D2}ms",
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds);
        return answer;
    }
}
