using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StackedHeatMap : MonoBehaviour
{
    /**
     * Creates a stacked Heatmap on the floor based on the sampled Hitpoints
     * 
     * 
     * **/
    GameObject stackBox;
    ParticipantData participant;
    VisualisationInfo vis;
    List<Vector3> hitPoints;
    Vector3[] sampledHitPoints;
    Toggle[]  toggles;
    bool clicked=false;
    Trajectory current_trajectory;
    Dictionary<int, GameObject> activeParts;
    bool activated = false;
    GameObject heatmapParent;
    GameObject currentHeatmapParent;
    GameObject parentParticipant;
    GameObject visual;
    float floor = -1.3f;
    int current_id;
    Dictionary<int, GameObject> activeStacks;
    bool[] actvieStack;
    Vector3 originPosition;
    Quaternion originRotation;
    Vector3 participantPos;
    Quaternion participantRot;
    Vector3 difference;
    Quaternion newRot;
    bool initialize = false;

    GameObject root;
    GameObject root2;
    Dictionary<Vector3, int> fixationCounts;
    Dictionary<Vector3, List<GameObject>> fixationCubes;
    Dictionary<int, bool> activeStates;
    Dictionary<Transform, bool> gridSwitcher;

    DateTime[] timestamps;
    int amount_s = 20;
    ActiveStateManager stateManager;
 
    Vector3 newPOs;
    Dictionary<Transform, int> countHitsinCell;
    Dictionary<Transform, int> currentHitsinCell;
    Dictionary<Transform, List<Transform>> stackHeat;
    Dictionary<Transform, Transform> currentStackHeat;
    Dictionary<Transform, List<int>> lines;
    Dictionary<Transform, List<int>> particpantsHit;
    Dictionary<Transform, List<float>> probsOfCells;
    Dictionary<Transform, float> multipliedProbsinCell;
    Dictionary<Transform, float> multipliedProbsinCell2;

    Transform[] cells;
    Transform[] cells2;
    private TimeLine time;
    Slider mySlider;
    float sigma = 1.0f;

    public Material grid_m;
    Ray ray;
    RaycastHit hit;
    #region Viridis Color Scale
    static Vector4 color_1 = new Vector4(0.9529994532916154f, 0.9125452328290099f, 0.11085876909361342f, 1f);
    static Vector4 color_2 = new Vector4(0.5758187113512072f, 0.8600490306624121f, 0.2073384027250591f, 1f);
    static Vector4 color_3 = new Vector4(0.24090172204161298f, 0.7633448774061599f, 0.42216355577803744f, 1f);
    static Vector4 color_4 = new Vector4(0.13182686795279983f, 0.6340509511445657f, 0.5291665063764508f, 1f);
    static Vector4 color_5 = new Vector4(0.20303365450234195f, 0.49729398741766556f, 0.5578110963268489f, 1f);
    static Vector4 color_6 = new Vector4(0.2633950954372358f, 0.3549021741820773f, 0.5528267289276363f, 1f);
    static Vector4 color_7 = new Vector4(0.31093764455559003f, 0.1867198329507134f, 0.499013125962462f, 1f);
    static Vector4 color_8 = new Vector4(0.2823645529290169f, 0.0f, 0.3310101940118055f, 1f);
    Vector4[] color_v = { color_8, color_7,color_6,color_5,color_4,color_3,color_2,color_1};
    #endregion
    int amountOfHits = 0;
    private void Start()
    {
        actvieStack = new bool[100];
        activeParts = new Dictionary<int, GameObject>();
        activeStacks = new Dictionary<int, GameObject>();
        heatmapParent = GameObject.Find("HeatMap");
        currentHeatmapParent = GameObject.Find("CurrentHeatMap");
        fixationCounts = new Dictionary<Vector3, int>();
        fixationCubes = new Dictionary<Vector3, List<GameObject>>();
        activeStates = new Dictionary<int, bool>();
        countHitsinCell = new Dictionary<Transform, int>();
        currentHitsinCell = new Dictionary<Transform, int>();
        stackHeat = new Dictionary<Transform, List<Transform>>();
        currentStackHeat = new Dictionary<Transform, Transform>();
        gridSwitcher = new Dictionary<Transform, bool>();
        lines = new Dictionary<Transform, List<int>>();
        particpantsHit = new Dictionary<Transform, List<int>>();
        probsOfCells = new Dictionary<Transform, List<float>>();
        multipliedProbsinCell = new Dictionary<Transform, float>();
        multipliedProbsinCell2 = new Dictionary<Transform, float>();

    }
    public void placeStacks(GameObject participantData, Trajectory trajectory,Toggle[] t ,ActiveStateManager stateManager,Slider slider)
    {
        if(!initialize)
            Initialize(participantData, t, trajectory, stateManager,slider);
        calculateTransformationData();
        clicked = !clicked;
        if (clicked)
        {
            participantActivated();
        }
        else
        {
            ResetCountList();
            ResetCurrentLists();
        }
           
        
    }

    private void calculateTransformationData()
    {
        originPosition = participant.originalOrigin;
        originRotation = participant.originalRotation;
     /*   root = GameObject.Find("Cube1");
        root2 = GameObject.Find("Cube2");
        root.transform.rotation = originRotation;
        root.transform.position = originPosition + visual.transform.position;*/

    }

    float multipliedProb=1.0f;

    private void Initialize(GameObject p, Toggle[] t, Trajectory trajectory, ActiveStateManager state,Slider slider)
    {
        participant = p.GetComponent<ParticipantData>();
        vis = p.GetComponent<VisualisationInfo>();
        stackBox = vis.heatMapStack;
        toggles = new Toggle[t.Length];
        current_trajectory = trajectory;
        visual = vis.visualization;
        parentParticipant = vis.participant;
        stateManager = state;
        cells = new Transform[vis.grid.GetComponentsInChildren<Transform>().Length];
        cells =  vis.grid.GetComponentsInChildren<Transform>();
        cells2 = new Transform[vis.grid2.GetComponentsInChildren<Transform>().Length];
        cells2 = vis.grid2.GetComponentsInChildren<Transform>();
        grid_m = vis.grid.GetComponent<MeshRenderer>().material;
        time = p.GetComponent<TimeLine>();
        mySlider = slider;
        mySlider.minValue = 0;
        mySlider.maxValue = 1;
        foreach (Transform cell in cells)
        {
            countHitsinCell.Add(cell,0);
            gridSwitcher.Add(cell,false);
            multipliedProbsinCell.Add(cell,0.0f);
           
        }

        foreach (Transform cell in cells2)
        {
            currentHitsinCell.Add(cell,0);
            multipliedProbsinCell2.Add(cell, 0.0f);
            // getDistanceToOtherCells(cell);
        }

       


        initialize = true;

    }

    public void currentTimeHeatMap()
    {

        //Check if Heatmap slider is on
      if (clicked)
        { 
           
            ResetCurrentLists();
           
             foreach (KeyValuePair<int,bool> state_p in stateManager.activeParticipants)
              {
                
                  switch (state_p.Value)
                  {
                     case true:
                        
                         getHitPointsData(state_p.Key);
                         calculateOrientationPosition(state_p.Key);
                      //  Debug.Log("CuurentTime"+time.currentMinute);
                         int index = findTimeLine(time.currentMinute,state_p.Key);
                      //  currentHitsOnWall(index);
                        //counts how many hit were in each cell upto the specific timeline
                        currentHitAmountinCell(index);        
                        break;
                    
                }
            }
         //   colorGrid();
            //generate cubes onto the cells 
            placeHeatCubes();
            colorCurrentCubes();
        }    
              
    }

    private void currentHitsOnWall(int line)
    {
        int length = 0 == line ? participant.hitPoints_sampled[current_id].Length : line;
        //   Debug.Log("Line"+line+"FixationLine"+fixationLine.Count);
        // int length = (fixationLine.Count <= line) ? fixationLine.Count : line;
        for (int i = 0; i < length; i++)
        {


            //These stacks will be projectedontothefloor of the mesh
            // Vector3 temp = newRot * sampledHitPoints[i] + difference;
            Vector3 temp = newRot * participant.hitPoints_sampled[current_id][i] + difference;
            int layerMask = LayerMask.GetMask("ScanPath");
            ray = new Ray(newRot * participant.gazeOrigins_sampled[current_id][i]+difference , temp);
            if (participant.event_Class_sampled[current_id][i] == "fixation")
            {
                if (Physics.Raycast(ray, out hit, 100, ~layerMask))
                {
                    Debug.Log("Hit name"+hit.transform.name);        
                    currentHitsinCell[hit.transform] += 1;
                }
            }

        }
    }

    private void colorGrid()
    {
        foreach (KeyValuePair<Transform, int> cell in currentHitsinCell)
        {
            if (cell.Value > amountOfHits)
            {
                foreach (KeyValuePair<Transform, int> innerCell in currentHitsinCell)
                {
                    if (innerCell.Value > amountOfHits)
                    {
                        float distance = getDistanceToOtherCells(cell.Key, innerCell.Key);
                       
                        prob = kde(distance, sigma);
                        //prob = epacheninkov(distance);
                        float currentProb = prob * innerCell.Value;
                        multipliedProbsinCell2[innerCell.Key] += currentProb;
                    }
                }

            }
        }

        float min = multipliedProbsinCell2.Values.Min();
        float max = multipliedProbsinCell2.Values.Max();
        foreach (KeyValuePair<Transform, int> kvp in currentHitsinCell)
        {

            float avg = mySlider.value * (max - min);

            if (kvp.Value > 0)
            {
                float prob = kde((float)kvp.Value, 1.0f);
                //     Debug.Log("prob of value" + prob);
                float clampWeight = Mathf.Clamp(multipliedProbsinCell2[kvp.Key], min, avg);
                Vector4 newColor = new Vector4(0, 0, 0, 0);
                float normalizedWeight = (clampWeight - min) / (avg - min);
                for (int i = 1; i <= 8; i++)
                {
                    float f_min = 1.0f / 7.0f * ((float)i - 1.0f);
                    float f_max = 1.0f / 7.0f * ((float)i);

                    if (normalizedWeight <= f_max)
                    {
                        float a1 = (f_max - normalizedWeight) / (1.0f / 7.0f);
                        float a2 = (normalizedWeight - f_min) / (1.0f / 7.0f);
                        newColor = a1 * color_v[i - 1] + a2 * color_v[i];
                        break;
                    }

                }
                kvp.Key.GetComponent<MeshRenderer>().material.color = newColor;
            }
          
        }
    }

    private void colorCurrentCubes()
    {
       
        foreach (KeyValuePair<Transform, int> cell in currentHitsinCell)
        {
            if (cell.Value> amountOfHits)
            {
                foreach (KeyValuePair<Transform, int> innerCell in currentHitsinCell)
                {
                    if (innerCell.Value> amountOfHits)
                    {
                        float distance = getDistanceToOtherCells(cell.Key,innerCell.Key);
                        prob = kde(distance, 1);
                        //prob = epacheninkov(distance);
                        float currentProb = prob * innerCell.Value;
                        multipliedProbsinCell2[innerCell.Key] += currentProb;
                    }
                }
            }
          
          
            // prob = epacheninkov(cell.Value);

           

          
        }
        float min = multipliedProbsinCell2.Values.Min();
        float max = multipliedProbsinCell2.Values.Max();
        foreach (KeyValuePair<Transform, int> kvp in currentHitsinCell)
        {
       
            float avg = mySlider.value * (max-min);
       
            if (kvp.Value > 0)
            {
                float prob = kde((float)kvp.Value, 1.0f);
           //     Debug.Log("prob of value" + prob);
                float clampWeight = Mathf.Clamp(multipliedProbsinCell2[kvp.Key], min, avg);
                Vector4 newColor = new Vector4(0, 0, 0, 0);
                float normalizedWeight = (clampWeight - min) / (avg - min);
                for (int i = 1; i <= 8; i++)
                {
                    float f_min = 1.0f / 7.0f * ((float)i - 1.0f);
                    float f_max = 1.0f / 7.0f * ((float)i);

                    if (normalizedWeight <= f_max)
                    {
                        float a1 = (f_max - normalizedWeight) / (1.0f / 7.0f);
                        float a2 = (normalizedWeight - f_min) / (1.0f / 7.0f);
                        newColor = a1 * color_v[i - 1] + a2 * color_v[i];
                        break;
                    }

                }
                currentStackHeat[kvp.Key].GetComponent<MeshRenderer>().material.color = newColor;
            }
        }
    }
    public void setSigmaValue(Slider slider)
    {
        sigma = slider.value / 10f;
    }
    float distance;
    List<float> probsOfCell;
    private float getDistanceToOtherCells(Transform start,Transform dest)
    {
        float dist = Vector3.Distance(start.position,dest.position);
        return dist;
    }
    public void fixationCount(Slider slider)
    {
        amountOfHits = (int)slider.value;
    }
    private void ResetCurrentLists()
    {
        foreach(KeyValuePair<Transform, int> cell in currentHitsinCell.ToList())
        {
            currentHitsinCell[cell.Key] = 0;
            cell.Key.GetComponent<MeshRenderer>().material.color= new Vector4(0f,0f,0f,0.001f);
            multipliedProbsinCell2[cell.Key] = 0.0f;
            
        }
        foreach (KeyValuePair<Transform, Transform> cube in currentStackHeat.ToList())
        {
            currentStackHeat[cube.Key].parent = null;
            DestroyImmediate(currentStackHeat[cube.Key].gameObject);
        }
        currentStackHeat.Clear();
        
    }

    private void placeHeatCubes()
    {
        foreach (KeyValuePair<Transform, int> cell in currentHitsinCell)
        {
            if (cell.Value > amountOfHits)
            {
              
                GameObject currentStack = Instantiate(stackBox,currentHeatmapParent.transform);
                currentStack.transform.position = cell.Key.position;
                currentStackHeat.Add(cell.Key,currentStack.transform);
            }
        }
    }
    bool newFix = true;
    private void currentHitAmountinCell(int line)
    {
        List<int> fixationLine;
        List<int> saccadeLine;
        participant.sortOutFixationandSaccades(current_id, out fixationLine, out saccadeLine);

        int length = participant.fixationPoints_sampled[current_id].Length > line ? line:participant.fixationPoints_sampled[current_id].Length;
        //   Debug.Log("Line"+line+"FixationLine"+fixationLine.Count);
        // int length = (fixationLine.Count <= line) ? fixationLine.Count : line;
      
        for (int i = 0; i < length; i++)
        {

            if (participant.event_Class_sampled[current_id][i]=="fixation"||newFix)
            {
                newFix = false;
                //These stacks will be projectedontothefloor of the mesh
                // Vector3 temp = newRot * sampledHitPoints[i] + difference;
                Vector3 temp = newRot * participant.fixationPoints_sampled[current_id][i] + difference;
                int layerMask = LayerMask.GetMask("HeatMapGrid");
                ray = new Ray(temp, -Vector3.up);
              
                    if (Physics.Raycast(ray, out hit, 100, layerMask))
                    {
                        //    Debug.Log("Hit name"+hit.transform.name);        
                        currentHitsinCell[hit.transform] += 1;
                    }
               
            }
            else if(participant.event_Class_sampled[current_id][i] == "gap"|| participant.event_Class_sampled[current_id][i] == "saccade")
            {
                newFix = true;
            }
      
          
        }
    }

    private int findTimeLine(float currentMinute, int participantID)
    {

        List<DateTime> timeSTempsUnsampled = participant.getEyeData(participantID);
        timestamps = participant.eyeDataTimes_sampled[participantID];
        float passedTime = 0.0f;

        for (int i = 1; i < timestamps.Length - 1; i++)
        {
            double interval = time.totalDuration(timestamps[i - 1], timestamps[i]);
            if (passedTime < currentMinute)
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

    public void participantActivated()
    {
        if (clicked)
        {
            ResetCountList();
            foreach (KeyValuePair<int, bool> state_p in stateManager.activeParticipants)
            {

                current_id = state_p.Key;
                // height = -1.3f;
                getHitPointsData(state_p.Key);
                calculateOrientationPosition(state_p.Key);
                switch (state_p.Value)
                {
                    case true:

                        List<int> fixationLine;
                        List<int> saccadeLine;
                        participant.sortOutFixationandSaccades(current_id, out fixationLine, out saccadeLine);

                        for (int i = 0; i < sampledHitPoints.Length; i++)
                        {
                            countGridHit(i, true);
                            
                        }
                        break;
                    case false:
                        break;
                }

            }
            stackCube();
            MapToColor();
        }
           
    }
    List<Transform> cellsSurrounding;
    float prob;
    private void MapToColor()
    {
        bool inSurround;
        cellsSurrounding = new List<Transform>();
       /** int min = countHitsinCell.Values.Min();
        int max = countHitsinCell.Values.Max();*/
        float avg=0;

        float min;
        float max;

        //min = max;
        foreach (KeyValuePair<Transform, int> cell in countHitsinCell)
        {
         
            if (cell.Value> amountOfHits)
            {
                //Debug.Log("Cell Name" + cell.Key.name + "Cell Value" + cell.Value);
                foreach (KeyValuePair<Transform, int> innerCell in countHitsinCell)
                {
                    if (innerCell.Value > amountOfHits)
                    {
                        float distance = getDistanceToOtherCells(cell.Key, innerCell.Key);
                        prob = kde(distance, 1);

                        //prob = epacheninkov(distance);
                        float currentProportion = prob * innerCell.Value;
                        multipliedProbsinCell[innerCell.Key] += currentProportion;
                    }

                }



            
            }
          
        }
        min = multipliedProbsinCell.Values.Min();
        max = multipliedProbsinCell.Values.Max();
        //  Debug.Log("minimum"+min);
        if (stackHeat.Count != 0)
        {
            avg = mySlider.value * (max-min);
        }
        foreach (KeyValuePair<Transform, int> cell in countHitsinCell)
        {
 
            if (cell.Value> amountOfHits)
            {
               // Debug.Log("Probability"+multipliedProbsinCell[cell.Key]);
                float clampWeight = Mathf.Clamp(multipliedProbsinCell[cell.Key] , min, avg);
                Vector4 newColor = new Vector4(0, 0, 0, 0);
                float normalizedWeight = (clampWeight - min) / (avg - min);
                for (int i = 1; i <= 8; i++)
                {
                    float f_min = (1.0f / 7.0f) * ((float)i - 1.0f);
                    float f_max = (1.0f / 7.0f) * ((float)i);

                    if (normalizedWeight <= f_max)
                    {
                        float a1 = (f_max - normalizedWeight) / (1.0f / 7.0f);
                        float a2 = (normalizedWeight - f_min) / (1.0f / 7.0f);
                        newColor = a1 * color_v[i - 1] + a2 * color_v[i];
                        break;
                    }

                }

              
                Transform[] stack = stackHeat[cell.Key].ToArray();
                for (int i=0;i<stack.Length;i++)
                {
                    stack[i].GetComponent<MeshRenderer>().material.color = newColor;
                }
           
            }
        }
    }
    private float kde(float d, float sigma)
    {
        float prob = (1.0f / (sigma * Mathf.Sqrt(2.0f * Mathf.PI))) * (Mathf.Exp(-(Mathf.Pow(d, 2.0f)) / 2.0f * Mathf.Pow(sigma, 2.0f)));
        return prob;
    }

    private float epacheninkov(float d)
    {
        float prob = (3.0f/4.0f)*(1-Mathf.Pow(d,2));
        return prob;
    }

    private void ResetCountList()
    {
        foreach (KeyValuePair<Transform, int> kvp in countHitsinCell.ToList())
        {
            countHitsinCell[kvp.Key] = 0;
            multipliedProbsinCell[kvp.Key] = 0.0f;

        }
        if (stackHeat.Count!=0)
        {
            foreach (KeyValuePair<Transform,List<Transform>> kvp in stackHeat.ToList() )
            {
                foreach (Transform t in kvp.Value)
                {
                    t.parent = null;
                    Destroy(t.gameObject);
                }
            }
            stackHeat.Clear();
        }
        
        lines.Clear();
        particpantsHit.Clear();
    }



    private void stackCube()
    {
        foreach (KeyValuePair<Transform, int> hits in countHitsinCell)
        {
            if (hits.Value>0)
            {
                List<Transform> stacks = new List<Transform>();
                float stackIntervalHeight = time.intervalHeight;
                float stackIntervalLength = time.intervalLength;
                

                for (int i = 0;i<hits.Value;i++)
                {
                      int current_i = lines[hits.Key][i];
                      int current_p = particpantsHit[hits.Key][i];
                      double current_time = time.totalDuration(participant.eyeDataTimes_sampled[current_p][0],participant.eyeDataTimes_sampled[current_p][current_i]);
                   
                      float current_height =((float)current_time * (stackIntervalHeight/stackIntervalLength));
                   // Debug.Log("Heihgt stack" + current_height);
                   // Debug.Log("MInute stack" + current_time);
                      GameObject newStack = Instantiate(stackBox, heatmapParent.transform);
                      newStack.transform.position = hits.Key.position + new Vector3(0f, current_height, 0f);
                      stacks.Add(newStack.transform);
                }
                stackHeat.Add(hits.Key, stacks);
            }
               
        }
    
       
    }

    private void calculateOrientationPosition(int p_id)
    {
        participantPos = participant.worldOrigins[p_id];
        participantRot = participant.worldRotations[p_id];

        newRot = originRotation * Quaternion.Inverse(participantRot);
        participantPos = newRot * participantPos;
        difference = originPosition - participantPos;

    }
    bool newFixation = true;
    private void countGridHit(int current_line,bool state_p)
    {
        if (participant.event_Class_sampled[current_id][current_line]=="fixation"&&newFixation)
        {
            newFixation = false;
           // ray = new Ray(newRot * sampledHitPoints[current_line] + difference + visual.transform.position, -Vector3.up);
             ray = new Ray(newRot * participant.fixationPoints_sampled[current_id][current_line] + difference + visual.transform.position, -Vector3.up);
            // Bit shift the index of the layer (8) to get a bit mask
            int layer_mask = LayerMask.GetMask("HeatMapGrid");


            if (Physics.Raycast(ray, out hit, 100f, layer_mask))
            {

                countHitsinCell[hit.transform] += 1;

                switch (!lines.ContainsKey(hit.transform))
                {
                    case true:
                        List<int> index = new List<int>();
                        List<int> id_p = new List<int>();
                        index.Add(current_line);
                        id_p.Add(current_id);
                        lines.Add(hit.transform, index);
                        particpantsHit.Add(hit.transform, id_p);
                        break;
                    case false:
                        lines[hit.transform].Add(current_line);
                        particpantsHit[hit.transform].Add(current_id);
                        break;
                }


            }

        }
        else if (participant.event_Class_sampled[current_id][current_line] == "gap"|| participant.event_Class_sampled[current_id][current_line] == "saccade")
        {
            newFixation = true;
        }
       
    }

    //Saves the hitPoints and time of CUrrent Participant into Array
    private void getHitPointsData(int p_id)
    {
        sampledHitPoints = participant.hitPoints_sampled[p_id];
        timestamps = participant.eyeDataTimes_sampled[p_id];
    }

   


}
