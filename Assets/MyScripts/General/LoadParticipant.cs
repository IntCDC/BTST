using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadParticipant : MonoBehaviour
{
    /**
    * loads the data of each participant into the dicitonaries in Particpant Data class
     * 
    * 
    * **/

    public Toggle toggle_p;
    public GameObject parent_Panel;
    int count = 9;
    Text toggle_label;
    List<string> paths = new List<string>();
    ReadData readGaze = new ReadData();
    ParticipantData p;
   
    public GameObject particpantGameObject;
 
    public void loadOldGazeData(string path,int participantNo,GameObject participant)
    {
        string fileName = path;
        paths.Add(fileName);
        readGaze.readGazeData(fileName);
        p = participant.GetComponent<ParticipantData>();
         
        p.setDictionaries(participantNo, readGaze.gazeOrigin, readGaze.gazeDir, readGaze.hitPoint, readGaze.eyeDataTime);
        p.setFixationDictionaries(participantNo,readGaze.fixationPoint,readGaze.eventName,readGaze.eventNo,readGaze.eventDuration,readGaze.angularV);
    }

    public void loadOldWorldData(string path, int participantNo, GameObject participant)
    {
        readGaze.readWorldData(path);
        p = participant.GetComponent<ParticipantData>();
        p.setWorldDicitonary(participantNo,readGaze.position,readGaze.rotation);
    }


}

