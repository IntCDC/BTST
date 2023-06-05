using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/**
[System.Serializable]
public class participantDeactivated : UnityEvent<int>
{
}
*/
public class Trajectory : MonoBehaviour
{
    /**
    *Set the trajectory of each participant
    * 
    * 
    * **/

    #region Required variables
    static List<DateTime> eyeGazeTime;
    static List<Vector3> gazeOrigin;
    static List<Vector3> gazeDirection;
    static List<Vector3> hitPoint;
    static Dictionary<int, GameObject> activeParticipants;
    Vector3[] sampledGazeOrigin;
    DateTime[] sampledDateTime;

    ParticipantData participant;
    VisualisationInfo vis;
    TimeLine time;
    GameObject pathTile;
    GameObject visualisation;
    GameObject parentParticipant;
    GameObject root_participants;
    const float floorHeight = -1.3f;
    float currentHeight;
    float interval = 0.025f;
    double currentDur;
    double intervalDuration = 1000;
    int current_id;
 
 
    Vector3 wordPosition;
    Quaternion worldRotation;
    Vector3 origin;
    Quaternion newRotation;
    Vector3 differencePos;
    ActiveStateManager stateManager;
    bool trajectoryState = false;
 
    GameObject p;
    Material[] p_material;

    LineRenderer l1;
    #endregion

    private void Start()
    {
      
        activeParticipants = new Dictionary<int, GameObject>();
        p_material = new Material[10];
    }

    public void stateParticpantChanged()
    {

        if (trajectoryState)
        {
            initialize(p);
            ResetAll();
            foreach (KeyValuePair<int, bool> kvp in stateManager.activeParticipants)
            {

                if (kvp.Value)
                {
                    current_id = kvp.Key;
                    getParticpantData(kvp.Key);
                    placeCube();
                }
 
            }
        }

    }

    private void ResetAll()
    {
        if (root_participants.transform.childCount > 0)
        {
            Transform[] children = root_participants.GetComponentsInChildren<Transform>();
            foreach (Transform child in children.Skip(1))
            {
                child.transform.parent = null;
                Destroy(child.gameObject);
            }
        }

    }

    public void LoadPath(int particpantID, GameObject p, ActiveStateManager state)
    {
        stateManager = state;
        this.p = p;
        trajectoryState = !trajectoryState;
        stateParticpantChanged();
        if (!trajectoryState)
            ResetAll();
    }



    private void initialize(GameObject p)
    {

        eyeGazeTime = new List<DateTime>();
        gazeOrigin = new List<Vector3>();
        gazeDirection = new List<Vector3>();
        hitPoint = new List<Vector3>();

        participant = p.GetComponent<ParticipantData>();
        vis = p.GetComponent<VisualisationInfo>();
   
        time = p.GetComponent<TimeLine>();
        pathTile = vis.pathTile;
        visualisation = vis.visualization;
        parentParticipant = vis.participant;

        root_participants = GameObject.Find("Participant");
        p_material[0] = vis.p1_m;
        p_material[1] = vis.p2_m;
        p_material[2] = vis.p3_m;
        p_material[3] = vis.p4_m;
        p_material[4] = vis.p5_m;
        p_material[5] = vis.p6_m;
        p_material[6] = vis.p7_m;
        p_material[7] = vis.p8_m;
        p_material[8] = vis.p9_m;
        p_material[9] = vis.p10_m;


    }

    private void placeCube()
    {
        currentHeight = floorHeight;
        GameObject parent = Instantiate(parentParticipant, root_participants.transform);
        parent.name = "Participant" + current_id;
        l1 = new LineRenderer();
        parent.AddComponent(l1.GetType());

        interval = time.intervalHeight;
        intervalDuration = time.intervalLength;
        parent.GetComponent<LineRenderer>().positionCount = sampledDateTime.Length - 1;
        for (int i = 0; i < sampledDateTime.Length - 1; i++)
        {

            GameObject newPath = Instantiate(pathTile, root_participants.transform);
            newPath.transform.rotation = newRotation * newPath.transform.rotation;
            Vector3 worldPosition = newRotation * sampledGazeOrigin[i] + visualisation.transform.position - differencePos;
            Vector3 projectFloor = new Vector3(worldPosition.x, currentHeight, worldPosition.z);
            currentDur = totalDuration(sampledDateTime[i], sampledDateTime[i + 1]);
            currentHeight += (float)calculateHeigt(interval, intervalDuration, currentDur);
            newPath.transform.position = projectFloor;
            parent.GetComponent<LineRenderer>().SetPosition(i, projectFloor);
            newPath.transform.GetComponent<MeshRenderer>().material = p_material[current_id - 1];
            parent.GetComponent<LineRenderer>().material = p_material[current_id - 1];
            parent.GetComponent<LineRenderer>().SetWidth(0.004f, 0.062f);
        }

    }
    double calculateHeigt(float interval, double interval_d, double last_d)
    {
        double difference = (interval / interval_d) * last_d;
        return difference;
    }

    void getParticpantData(int particpantID)
    {

        eyeGazeTime = participant.getEyeData(particpantID);
        gazeOrigin = participant.getGazeOrigin(particpantID);
        gazeDirection = participant.getGazeDirection(particpantID);
        hitPoint = participant.getHit(particpantID);
        origin = participant.originalOrigin;      
        wordPosition = participant.worldOrigins[particpantID];
        worldRotation = participant.worldRotations[particpantID];
        newRotation = participant.calculateOrientation(worldRotation);
        differencePos = participant.calculateDifference(origin, newRotation * wordPosition);
        sampledDateTime = participant.eyeDataTimes_sampled[particpantID];
        sampledGazeOrigin = participant.gazeOrigins_sampled[particpantID];
       
    }

    private void ClearAll()
    {
        eyeGazeTime.Clear();
        gazeOrigin.Clear();
        gazeDirection.Clear();
        hitPoint.Clear();

    }

    double totalDuration(DateTime s, DateTime e)
    {
        TimeSpan difference = e - s;
        double totalMs = difference.TotalMilliseconds;
        return totalMs;
    }

    public Dictionary<int, GameObject> getActivePartcipants()
    {
        return activeParticipants;
    }
}
