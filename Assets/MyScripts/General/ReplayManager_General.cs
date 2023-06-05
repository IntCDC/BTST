using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayManager_General : MonoBehaviour
{

    /**
     * This Class Visualizes the participants within the Gaze Replay.
     * Each Participant gets instantiated and positioned according the timeline
     * */

    #region Required Variables
    public GameObject p_1;
    public GameObject p_2;
    public GameObject p_3;
    public GameObject p_4;
    public GameObject p_5;
    public GameObject p_6;
    public GameObject p_7;
    public GameObject p_8;
    public GameObject p_9;
    public GameObject p_10;

    GameObject[] participants;
    LineRenderer[] participant_line;
    LineRenderer l1;
    LineRenderer l2;
    LineRenderer l3;
    LineRenderer l4;
    LineRenderer l5;
    LineRenderer l6;
    LineRenderer l7;
    LineRenderer l8;
    LineRenderer l9;
    LineRenderer l10;
    ParticipantData p_data;

    Ray[] ray_p =new Ray[10];


    Dictionary<int, List<Vector3>> gazeOrigin;
    Dictionary<int, List<Vector3>> gazeDirection;
    Dictionary<int, List<DateTime>> time;

    Dictionary<int, Vector3[]> gazeOrigin_sampled;
    Dictionary<int, Vector3[]> gazeDirection_sampled;
    Dictionary<int, Vector3[]> hitPoint_sampled;
    Dictionary<int, DateTime[]> time_sampled;
    bool initialized = false;

    ActiveStateManager activeState;

    #endregion 
    // Start is called before the first frame update
    void Start()
    {
        participants = new GameObject[] { p_1 ,p_2,p_3,p_4,p_5,p_6,p_7,p_8,p_9,p_10};
       
        participant_line = new LineRenderer[] { l1, l2, l3, l4, l5, l6, l7, l8,l9,l10 };
        for (int i = 0; i < participant_line.Length; i++)
        {
            
            participant_line[i] = participants[i].GetComponent<LineRenderer>();
        } 
        gazeOrigin = new Dictionary<int, List<Vector3>>();
        gazeDirection = new Dictionary<int, List<Vector3>>();
        time = new Dictionary<int, List<DateTime>>();
        gazeDirection_sampled = new Dictionary<int, Vector3[]>();
        gazeOrigin_sampled = new Dictionary<int, Vector3[]>();
        time_sampled = new Dictionary<int, DateTime[]>();
        hitPoint_sampled = new Dictionary<int, Vector3[]>();
    }

    public void instantiate_p(ActiveStateManager state,GameObject p_info)
    {
        p_data = p_info.GetComponent<ParticipantData>();
        activeState = state;
        foreach (KeyValuePair<int , bool> s in state.activeParticipants)
        {
            switch (s.Key)
            {
                case 1:
                    p_1.SetActive(s.Value);
                    break;
                case 2:
                    p_2.SetActive(s.Value);
                    break;
                case 3:
                    p_3.SetActive(s.Value);
                    break;
                case 4:
                    p_4.SetActive(s.Value);
                    break;
                case 5:
                    p_5.SetActive(s.Value);
                    break;
                case 6:
                    p_6.SetActive(s.Value);
                    break;
                case 7:
                    p_7.SetActive(s.Value);
                    break;
                case 8:
                    p_8.SetActive(s.Value);
                    break;
                case 9:
                    p_9.SetActive(s.Value);
                    break;
                case 10:
                    p_10.SetActive(s.Value);
                    break;
            }

            if (s.Value)
            {
                if (!gazeOrigin.ContainsKey(s.Key))
                {
                    gazeOrigin.Add(s.Key, p_data.getGazeOrigin(s.Key));
                    gazeDirection.Add(s.Key, p_data.getGazeDirection(s.Key));
                    time.Add(s.Key, p_data.getEyeData(s.Key));
                    gazeDirection_sampled.Add(s.Key, p_data.gazeDirections_sampled[s.Key]);
                    gazeOrigin_sampled.Add(s.Key, p_data.gazeOrigins_sampled[s.Key]);
                    time_sampled.Add(s.Key, p_data.eyeDataTimes_sampled[s.Key]);
                    hitPoint_sampled.Add(s.Key,p_data.hitPoints_sampled[s.Key]);
                }               
            }          
        }
    }

    public void positionPlayer(float value)
    {
        if (activeState!=null)
        {
            foreach (KeyValuePair<int, bool> active in activeState.activeParticipants)
            {
                int index = p_data.findCurrentTime_index(value,active.Key);
                p_data.calculateOrientationPosition(active.Key, out Vector3 diff, out Quaternion newRot);

                ray_p[active.Key - 1] = new Ray(newRot*gazeOrigin_sampled[active.Key][index]-diff,newRot*gazeDirection_sampled[active.Key][index]);
              
                participants[active.Key-1].transform.position =newRot*gazeOrigin_sampled[active.Key][index]+diff;
                participants[active.Key - 1].transform.localPosition = new Vector3(participants[active.Key - 1].transform.localPosition.x,-2.49f, participants[active.Key - 1].transform.localPosition.z);
              
                participant_line[active.Key-1].SetPosition(0, new Vector3(participants[active.Key - 1].transform.position.x, participants[active.Key - 1].transform.position.y + 0.75f, participants[active.Key - 1].transform.position.z));
                participant_line[active.Key-1].SetPosition(1, newRot*hitPoint_sampled[active.Key][index]+diff);
            }
        }    
    }
}
