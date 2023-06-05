using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeLine : MonoBehaviour
{

    /**
     * This class is responsible for the timeline within the Gaze Replay and the STC
     * The timeline value is set to a fixed value of 8 minutes
     * The data of each participant lays within this time interval
     * **/

    #region Required variables
    public float intervalHeight=0.025f;
    public float intervalLength=1000;
    public float currentHeight=0.0f;
    public float currentMinute;
    ParticipantData p_data;
    GameObject timeSLider;
    GameObject timeSpan;
    GameObject timeSpanUp;
    GameObject images;
    TextMeshProUGUI timeText;

    public float totalMinutes = 480f;
    float maxHeightRight;
    float minHeightLeft = -1.3f;
    float rightDragTime;
    float leftDragTime;
    float minHeight = -1.3f;

    #endregion
    void Start()
    {
        intervalHeight = 0.025f;
        currentHeight = 0.0f;
        rightDragTime = totalMinutes;
        maxHeightRight = intervalHeight * totalMinutes;
    }

    public void setRightDragTime(float dragTime)
    {
        rightDragTime = dragTime;
        maxHeightRight = minHeight + (intervalHeight * (rightDragTime / 1000f));
    }

    public void setLeftDragTime(float dragTime)
    {
        leftDragTime = dragTime;
        minHeightLeft = intervalHeight * (leftDragTime / 1000f);
        minHeightLeft = minHeight + minHeightLeft;
    }

    public double getCurrentTime(Slider slider)
    {     
        //8 minutes in seconds as a fixed timeline scale value
        totalMinutes = 480f;
        float currentValue = slider.value;
        float maxHeight = intervalHeight * totalMinutes;
        float minHeight = -1.3f;
        currentHeight = minHeight+(maxHeight * currentValue);
        currentMinute = totalMinutes * 1000 * currentValue;
        string time = changeIntoTimeFormat(currentMinute);
       
        p_data.setCurrentTIme(currentMinute);
        timeText = p_data.GetComponent<VisualisationInfo>().textTime;
        timeText.text = time;
        timeSLider.transform.position = new Vector3(timeSLider.transform.position.x,currentHeight,timeSLider.transform.position.z);
        images.transform.position = new Vector3(images.transform.position.x, currentHeight+1.3f, images.transform.position.z);
        return currentMinute;
    }

    internal void ScaleTimeSpan()
    {
        float yScaleTimeSpan = maxHeightRight - (minHeightLeft);
     
        timeSpanUp.transform.position = new Vector3(timeSpanUp.transform.position.x, maxHeightRight, timeSpanUp.transform.position.z);
      
        timeSpan.transform.position = new Vector3(timeSpan.transform.position.x, minHeightLeft + (timeSpan.transform.localScale.y / 2.0f), timeSpan.transform.position.z);
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

    public void getParticipantData(GameObject participantInfo)
    {
        p_data = participantInfo.GetComponent<ParticipantData>();
        timeSLider = participantInfo.GetComponent<VisualisationInfo>().timeSlider;
        timeSpan = participantInfo.GetComponent<VisualisationInfo>().timeSpan;
        timeSpanUp = participantInfo.GetComponent<VisualisationInfo>().timeSpanUp;
        images = participantInfo.GetComponent<VisualisationInfo>().scanPathImg;
    }

    public double totalDuration(DateTime s, DateTime e)
    {

        TimeSpan difference = e - s;
        double totalMs = difference.TotalMilliseconds;
        return totalMs;
    }
    float height;
    public void setIntervalHeight(TMP_InputField heightText)
    {
        height=float.Parse(heightText.text);
        this.intervalHeight = height;
    }

    public void increaseIntervalHeight(TextMeshProUGUI heightext)
    {
        
        this.intervalHeight = intervalHeight+0.005f;
        heightext.text = this.intervalHeight.ToString();
    }
    public void decreaseIntervalHeight(TextMeshProUGUI heightext)
    {

        this.intervalHeight = intervalHeight - 0.005f;
        heightext.text = this.intervalHeight.ToString();
    }
}
