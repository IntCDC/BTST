using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleData : MonoBehaviour
{
    private int amount;
    private Vector3[] sampledGazeOrigin;
    Vector3[] sampledGazeDirection;
    Vector3[] sampledHitPoint;
    DateTime[] sampledDateTime;

    Vector3[] sampledFixationPoint;
    int[] sampled_event_id;
    string[] sampled_event_class;
    float[] sampled_event_duration;
    float[] sampled_angular_v;


    private double longestDuration;
    int current_id;
    public void sampleData(int amountOfSampling,int id,ParticipantData participant)
    {
        current_id = id;
        if (amountOfSampling == 1)
        {
            int lengthOfArray = participant.getEyeData(id).Count;
            sampledGazeOrigin = new Vector3[lengthOfArray + 1];
            sampledGazeDirection = new Vector3[lengthOfArray + 1];
            sampledHitPoint = new Vector3[lengthOfArray + 1];
            sampledDateTime = new DateTime[lengthOfArray + 1];

            sampledFixationPoint = new Vector3[lengthOfArray+1];
            sampled_event_id = new int[lengthOfArray+1];
            sampled_event_class = new string[lengthOfArray+1];
            sampled_event_duration = new float[lengthOfArray+1];
            sampled_angular_v = new float[lengthOfArray+1];


            for (int i = 0; i < lengthOfArray; i++)
            {
                sampledGazeOrigin[i] = participant.getGazeOrigin(id)[i];
                sampledGazeDirection[i] = participant.getGazeDirection(id)[i];
                sampledDateTime[i] = participant.getEyeData(id)[i];
                sampledHitPoint[i] = participant.getHit(id)[i];

                sampledFixationPoint[i] = participant.getFixations(id)[i];
                sampled_angular_v[i] = participant.getAngular_v(id)[i];
                sampled_event_class[i] = participant.getEventName(id)[i];
                sampled_event_duration[i] = participant.getEventDur(id)[i];
                sampled_event_id[i] = participant.getEventId(id)[i];
            }

        }
        else
        {
            int lengthOfArray = (int)Mathf.Floor(participant.getEyeData(id).Count / amountOfSampling);

            sampledGazeOrigin = new Vector3[lengthOfArray + 1];
            sampledGazeDirection = new Vector3[lengthOfArray + 1];
            sampledHitPoint = new Vector3[lengthOfArray + 1];
            sampledDateTime = new DateTime[lengthOfArray + 1];


            sampledFixationPoint = new Vector3[lengthOfArray + 1];
            sampled_event_id = new int[lengthOfArray + 1];
            sampled_event_class = new string[lengthOfArray + 1];
            sampled_event_duration = new float[lengthOfArray + 1];
            sampled_angular_v = new float[lengthOfArray + 1];
            int j = 0;
            for (int i = 1; i <= participant.getEyeData(id).Count - 1; i = i + amountOfSampling)
            {

                if (i <= participant.getEyeData(id).Count - 1)
                {
                    sampledGazeOrigin[j] = participant.getGazeOrigin(id)[i];
                    sampledGazeDirection[j] = participant.getGazeDirection(id)[i];
                    sampledDateTime[j] = participant.getEyeData(id)[i];
                    sampledHitPoint[j] = participant.getHit(id)[i];

                    sampledFixationPoint[j] = participant.getFixations(id)[i];
                    sampled_angular_v[j] = participant.getAngular_v(id)[i];
                    sampled_event_class[j] = participant.getEventName(id)[i];
                    sampled_event_duration[j] = participant.getEventDur(id)[i];
                    sampled_event_id[j] = participant.getEventId(id)[i];

                }
                else
                {
                    sampledGazeOrigin[j] = participant.getGazeOrigin(id)[i];
                    sampledGazeDirection[j] = participant.getGazeDirection(id)[i];
                    sampledDateTime[j] = participant.getEyeData(id)[i];
                    sampledHitPoint[j] = participant.getHit(id)[i];
   
                    sampledFixationPoint[j] = participant.getFixations(id)[i];
                    sampled_angular_v[j] = participant.getAngular_v(id)[i];
                    sampled_event_class[j] = participant.getEventName(id)[i];
                    sampled_event_duration[j] = participant.getEventDur(id)[i];
                    sampled_event_id[j] = participant.getEventId(id)[i];

                }
                j++;

            }

        }

        setSamples(participant);
        double currentDuration = totalDuration(sampledDateTime[0], sampledDateTime[sampledDateTime.Length - 1]);
        if (longestDuration < currentDuration)
        {
            longestDuration = currentDuration;
        }
    }

    double totalDuration(DateTime s, DateTime e)
    {

        TimeSpan difference = e - s;
        double totalMs = difference.TotalMilliseconds;
        return totalMs;
    }

    private void setSamples(ParticipantData participant)
    {
        participant.eyeDataTimes_sampled.Add(current_id, sampledDateTime);
        participant.gazeOrigins_sampled.Add(current_id, sampledGazeOrigin);
        participant.gazeDirections_sampled.Add(current_id, sampledGazeDirection);
        participant.hitPoints_sampled.Add(current_id, sampledHitPoint);

        participant.fixationPoints_sampled.Add(current_id,sampledFixationPoint);
        participant.event_Class_sampled.Add(current_id,sampled_event_class);
        participant.event_duration_sampled.Add(current_id,sampled_event_duration);
        participant.event_id_sampled.Add(current_id,sampled_event_id);
        participant.angular_velocity_sampled.Add(current_id,sampled_angular_v);
    }

}
