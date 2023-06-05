using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticipantData:MonoBehaviour
{
    /**
    * GEts accessed by other classes in order to get the participant datas
    * 
    * 
    * **/

    #region Required Variables
    public Dictionary<int, Vector3> worldOrigins;
    public Dictionary<int, Quaternion> worldRotations;

    public  Vector3 originalOrigin;
    public Quaternion originalRotation; 

    public Dictionary<int, List<Vector3>> gazeOrigins;
    public  Dictionary<int, List<Vector3>> gazeDirections;
    public  Dictionary<int, List<Vector3>> hitPoints;
    public  Dictionary<int, List<DateTime>> eyeDataTimes ;

    public Dictionary<int, List<Vector3>> fixationPoints;
    public Dictionary<int, List<string>> event_Class;
    public Dictionary<int, List<int>> event_id;
    public Dictionary<int, List<float>> angular_velocity;
    public Dictionary<int, List<float>> event_duration;

    public Dictionary<int, Vector3[]> gazeOrigins_sampled;
    public Dictionary<int, Vector3[]> gazeDirections_sampled;
    public Dictionary<int, Vector3[]> hitPoints_sampled;
    public Dictionary<int, DateTime[]> eyeDataTimes_sampled;


    public Dictionary<int, Vector3[]> fixationPoints_sampled;
    public Dictionary<int, string[]> event_Class_sampled;
    public Dictionary<int, int[]> event_id_sampled;
    public Dictionary<int, float[]> angular_velocity_sampled;
    public Dictionary<int, float[]> event_duration_sampled;

    public Dictionary<int,Dictionary<string,List<Vector2>>> uvCoords;
    public Dictionary<int, Dictionary<string, List<DateTime>>> uvTimeStamps;
    Dictionary<string, List<Vector2>> uvsCoord;
    Dictionary<string, List<DateTime>> uvsTime;
    public float currentMinute;

    #endregion
    private void Start()
    {
        currentMinute = 0.0f;
        worldOrigins = new Dictionary<int, Vector3>();
        worldRotations = new Dictionary<int, Quaternion>();
        gazeOrigins = new Dictionary<int, List<Vector3>>();
        gazeDirections = new Dictionary<int, List<Vector3>>();
        hitPoints = new Dictionary<int, List<Vector3>>();
        eyeDataTimes = new Dictionary<int, List<DateTime>>();

        fixationPoints = new Dictionary<int, List<Vector3>>();
        event_Class = new Dictionary<int, List<string>>();
        event_duration = new Dictionary<int, List<float>>();
        event_id = new Dictionary<int, List<int>>();
        angular_velocity = new Dictionary<int, List<float>>();


        gazeOrigins_sampled = new Dictionary<int, Vector3[]>();
        gazeDirections_sampled = new Dictionary<int, Vector3[]>();
        hitPoints_sampled = new Dictionary<int, Vector3[]>();
        eyeDataTimes_sampled = new Dictionary<int, DateTime[]>();

        fixationPoints_sampled = new Dictionary<int, Vector3[]>();
        event_Class_sampled = new Dictionary<int, string[]>();
        event_duration_sampled = new Dictionary<int, float[]>();
        event_id_sampled = new Dictionary<int, int[]>();
        angular_velocity_sampled = new Dictionary<int, float[]>();

        uvCoords = new Dictionary<int, Dictionary<string, List<Vector2>>>();
        uvTimeStamps = new Dictionary<int, Dictionary<string, List<DateTime>>>();
        uvsCoord = new Dictionary<string, List<Vector2>>();
        uvsTime = new Dictionary<string, List<DateTime>>();

        originalOrigin = new Vector3(6.04925f, -1.350565f, 2.123948f);
        originalRotation = new Quaternion(0.0002687277f, 0.9991931f, -0.0033791f, 0.04002222f);
    }

    //Gets set every time a partcipant gets checked in the list
    public  void  setDictionaries(int participantNo, List<Vector3> gazeOrigin, List<Vector3> gazeDirection, List<Vector3> hitPoint, List<DateTime> eyeDataTime) 
    {
        lock (gazeOrigins)
        {
            gazeOrigins.Add(participantNo, gazeOrigin);
           
        }
        lock (gazeDirections)
        {
            gazeDirections.Add(participantNo, gazeDirection);
            
        }
        lock (hitPoints)
        {
            hitPoints.Add(participantNo, hitPoint);
          
        }
        lock (eyeDataTimes)
        {
            eyeDataTimes.Add(participantNo, eyeDataTime);
         
        }
   
 
    }

    public void setFixationDictionaries(int participantNo, List<Vector3> fixationPoint, List<string> eventName, List<int> eventNo, List<float> eventDuration, List<float> angularV)
    {
        fixationPoints.Add(participantNo,fixationPoint);
        event_Class.Add(participantNo,eventName);
        event_id.Add(participantNo,eventNo);
        event_duration.Add(participantNo,eventDuration);
        angular_velocity.Add(participantNo,angularV);
    }

    public void setWorldDicitonary(int participantNo, Vector3 worldPos, Quaternion worldRot)
    {
        worldOrigins.Add(participantNo,worldPos);
        worldRotations.Add(participantNo,worldRot);
    }
 

    public void setUVCoords(int participantNo, List<DateTime> timeStamps, List<Vector2> uvs, string key)
    {
        if (uvsCoord.ContainsKey(key))
        {
            uvsCoord.Clear();
        }
        else
        {
            uvsCoord.Add(key, uvs);
            uvCoords.Add(participantNo, uvsCoord);
        }
      
    }
    public List<Vector3> getGazeOrigin(int i)
    {
        return gazeOrigins[i];
    }

    public List<Vector3> getGazeDirection(int i)
    {
        return gazeDirections[i];
    }
    public List<Vector3> getHit(int i)
    {
        return hitPoints[i];
    }

    public List<DateTime> getEyeData(int i)
    {
        return eyeDataTimes[i];
    }


    public List<Vector3> getFixations(int i)
    {
        return fixationPoints[i];
    }

    public List<float> getEventDur(int i)
    {
        return event_duration[i];
    }

    public List<float> getAngular_v(int i)
    {
        return angular_velocity[i];
    }

    public List<string> getEventName(int i) 
    {
        return event_Class[i];
    }

    public List<int> getEventId(int i) 
    {
        return event_id[i];
    }

    
    public Quaternion calculateOrientation(Quaternion differentRotation)
    {
        Quaternion rotation = originalRotation * Quaternion.Inverse(differentRotation);
        return rotation;
    }

    public Vector3 calculateDifference(Vector3 orignalPosition,Vector3 differentPosition)
    {
        Vector3 position = differentPosition - orignalPosition;
        
        return position;
    }
    public void calculateOrientationPosition(int p_id, out Vector3 difference, out Quaternion newRot)
    {
       Vector3 participantPos = worldOrigins[p_id];
       Quaternion participantRot = worldRotations[p_id];
        
        newRot = originalRotation * Quaternion.Inverse(participantRot);
        participantPos = newRot * participantPos;
        difference = originalOrigin - participantPos;
       // Debug.Log("Differemce"+difference);
    }

    public void setCurrentTIme(float current)
    {
        currentMinute = current;
        
    }

    public float getCurrentTime()
    {
        return currentMinute;
    }
    public int findCurrentTime_index(float value, int id) 
    {
        DateTime[] timestamps = eyeDataTimes_sampled[id];
        float passedTime = 0.0f;

        for (int i = 1; i < timestamps.Length - 1; i++)
        {
            double interval = totalDuration(timestamps[i - 1], timestamps[i]);
            if (passedTime < /*currentMinute*/480000f * value)
            {
                passedTime += (float)interval;

            }
            else
            {

                return i;

            }

        }
        return 0;
    }

    public double totalDuration(DateTime s, DateTime e)
    {

        TimeSpan difference = e - s;
        double totalMs = difference.TotalMilliseconds;
        return totalMs;
    }
    int event_id_l = 0;
    public void sortOutFixationandSaccades(int id,out List<int> fixationLine, out List<int> saccadeLine)
    {
        fixationLine = new List<int>();

        saccadeLine = new List<int>();
        for (int i = 0; i < eyeDataTimes_sampled[id].Length; i++)
        {

            if (event_Class_sampled[id][i] != null)
            {
                if (event_Class_sampled[id][i].Contains("fixation"))
                {
                    if (event_id_sampled[id][i] != event_id_l)
                    {
                        fixationLine.Add(i);
                        event_id_l = event_id_sampled[id][i];

                    }

                }
                else if (event_Class_sampled[id][i].Contains("saccade"))
                {
                    saccadeLine.Add(i);
                }
            }

        }
    }
}
