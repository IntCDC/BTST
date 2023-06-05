using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeatmapWall : MonoBehaviour
{
    #region Variables for Heatmap
    bool wall_heat_on = false;
    int amountOfHits = 0;
    ParticipantData p_data;
    VisualisationInfo visual_objects;
    TimeLine timeLine;
    ActiveStateManager active_states;
    RaycastHit hit;
    Ray ray;
    string heat_metric = "fixationcount";
    bool isNewFixation;
    public Transform wall1;
    public Transform wall2;
    public Transform wall3;
    Dictionary<Transform, int> hits_cell;
    Dictionary<Transform, float> probability_cell;
    float sigma = 0.1f;
    public Slider heatSlider;
    float minLeft;
    float maxRight;
    gridMesh grid2;
    gridMesh grid1;
    #endregion

    #region Viridis Color Scale
    static Vector4 color_1 = new Vector4(0.9529994532916154f, 0.9125452328290099f, 0.11085876909361342f, 0.4f);
    static Vector4 color_2 = new Vector4(0.5758187113512072f, 0.8600490306624121f, 0.2073384027250591f, 0.4f);
    static Vector4 color_3 = new Vector4(0.24090172204161298f, 0.7633448774061599f, 0.42216355577803744f, 0.4f);
    static Vector4 color_4 = new Vector4(0.13182686795279983f, 0.6340509511445657f, 0.5291665063764508f, 0.4f);
    static Vector4 color_5 = new Vector4(0.20303365450234195f, 0.49729398741766556f, 0.5578110963268489f, 0.4f);
    static Vector4 color_6 = new Vector4(0.2633950954372358f, 0.3549021741820773f, 0.5528267289276363f, 0.4f);
    static Vector4 color_7 = new Vector4(0.31093764455559003f, 0.1867198329507134f, 0.499013125962462f, 0.4f);
    static Vector4 color_8 = new Vector4(0.2823645529290169f, 0.0f, 0.3310101940118055f, 0.2f);
    Vector4[] color_v = { color_8, color_7, color_6, color_5, color_4, color_3, color_2, color_1 };
    #endregion
    void Start()
    {
        hits_cell = new Dictionary<Transform, int>();
        probability_cell = new Dictionary<Transform, float>();
        grid1 = wall1.transform.GetComponent<gridMesh>();
        grid2 = wall2.transform.GetComponent<gridMesh>();
    }

    #region initialize needed Variables,Helper Functions to set TimeSlider, Sigma Value
    internal void initialize(GameObject particpantGameObject)
    {
        wall_heat_on = !wall_heat_on;
        if (grid1.Gridloaded)
        {
            Transform[] cells1 = wall1.GetComponentsInChildren<Transform>();

            foreach (Transform cell in cells1.Skip(1))
            {
                hits_cell.Add(cell, 0);
                probability_cell.Add(cell, 0.0f);
            }

        }
        else if (grid2.Gridloaded)
        {
            Transform[] cells2 = wall2.GetComponentsInChildren<Transform>();
            Transform[] cells3 = wall3.GetComponentsInChildren<Transform>();
            foreach (Transform cell in cells2.Skip(1))
            {
                hits_cell.Add(cell, 0);
                probability_cell.Add(cell, 0.0f);
            }
            foreach (Transform cell in cells3.Skip(1))
            {

                hits_cell.Add(cell, 0);
                probability_cell.Add(cell, 0.0f);
            }
        }

        p_data = particpantGameObject.GetComponent<ParticipantData>();
        visual_objects = particpantGameObject.GetComponent<VisualisationInfo>();
        timeLine = particpantGameObject.GetComponent<TimeLine>();
        active_states = particpantGameObject.GetComponent<ActiveStateManager>();
        minLeft = 0;
        maxRight = timeLine.totalMinutes * 1000f;
        getHeatOnWall(0.0f);
    }
    public void setSigmaValue(Slider slider)
    {
        sigma = slider.value / 10f;
    }
    public void setLeftTime(GameObject leftSpan)
    {
        minLeft = leftSpan.GetComponent<dragTimeSpan>().currentMinute;

    }

    public void setRightTime(GameObject rightSpan)
    {
        maxRight = rightSpan.GetComponent<dragTimeSpan>().currentMinute;
    }

    #endregion

    #region Heatmap Calculation
    internal void getHeatOnWall(float value)
    {
        ResetValues();
        if (wall_heat_on)
        {
            foreach (KeyValuePair<int, bool> state in active_states.activeParticipants)
            {
                if (state.Value)
                {
                    getHitPointOnWall(state.Key, value);
                }
            }
            if (heat_metric == "fixationcount")
            {
                mapColorToGrid();
            }

        }
        else
        {

            ResetValues();

            mapColorToGrid();
        }
    }


    private void getHitPointOnWall(int current_p, float slider_value)
    {
        int index = p_data.findCurrentTime_index(slider_value, current_p);

        float current_value = (p_data.fixationPoints_sampled[current_p].Length > index) ? index : p_data.fixationPoints_sampled[current_p].Length;

        Quaternion p_orientation;
        Vector3 p_position;
        p_data.calculateOrientationPosition(current_p, out p_position, out p_orientation);
        int leftIndex = p_data.findCurrentTime_index((1 / (timeLine.totalMinutes * 1000f)) * minLeft, current_p);

        int layerMask = LayerMask.GetMask("HeatMapWall");
        for (int i = leftIndex; i < current_value; i++)
        {

            if (p_data.event_Class_sampled[current_p][i] == "fixation" && isNewFixation)
            {
                isNewFixation = false;
                Vector3 fixToOrigin = ((p_orientation * p_data.fixationPoints_sampled[current_p][i] + p_position) - (p_orientation * p_data.gazeOrigins_sampled[current_p][i] + p_position));
                ray = new Ray(p_orientation * p_data.gazeOrigins_sampled[current_p][i] + p_position, fixToOrigin);
                if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
                {

                    hits_cell[hit.transform] += 1;

                }
            }
            else if (p_data.event_Class_sampled[current_p][i] == "gap" || p_data.event_Class_sampled[current_p][i] == "saccade")
            {
                isNewFixation = true;
            }
        }
    }
    int neigbourhood_size = 10;

    public void fixationCount(Slider slider)
    {
        amountOfHits = (int)slider.value;
    }
    private void mapColorToGrid()
    {
        int count = 0;
        foreach (KeyValuePair<Transform, int> cell in hits_cell)
        {
            if (cell.Value > amountOfHits)
            {
                float weight = cell.Value;
                count++;

                foreach (KeyValuePair<Transform, int> other_cell in hits_cell)
                {
                    float distance = Vector3.Distance(cell.Key.position, other_cell.Key.position);



                    float probability = kde(distance, sigma);
                    probability_cell[other_cell.Key] += probability * weight;

                }
            }
        }

        foreach (KeyValuePair<Transform, int> cell in hits_cell)
        {
            probability_cell[cell.Key] = (1.0f / hits_cell.Keys.Count) * probability_cell[cell.Key];

        }
        float min = probability_cell.Values.Min();
        float max = probability_cell.Values.Max();

        float avg = heatSlider.value * ((max - min));
        foreach (KeyValuePair<Transform, float> cell_prob in probability_cell)
        {
            float clampWeight = Mathf.Clamp(probability_cell[cell_prob.Key], min, avg);
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
            cell_prob.Key.GetComponent<MeshRenderer>().material.color = newColor;
        }
    }

    #endregion

    #region Helper Functions
    private float kde(float d, float sigma)
    {

        float prob = (1.0f / (sigma * Mathf.Sqrt(2.0f * Mathf.PI))) * (Mathf.Exp(-0.5f * (Mathf.Pow((d / sigma), 2.0f))));
        return prob;
    }

    private float epacheninkov(float d, float sigma)
    {
        float prob;
        if (Mathf.Abs(d) <= 0.5f)
        {
            prob = (3.0f / 4.0f) * (1 - (d * d));
        }
        else
        {
            prob = 0;
        }

        return prob;
    }



    private void ResetValues()
    {
        hits_cell.Clear();
        probability_cell.Clear();
        Transform[] cells1 = wall1.GetComponentsInChildren<Transform>();
        Transform[] cells2 = wall2.GetComponentsInChildren<Transform>();
        Transform[] cells3 = wall3.GetComponentsInChildren<Transform>();

        foreach (Transform cell in cells1.Skip(1))
        {

            hits_cell.Add(cell, 0);
            probability_cell.Add(cell, 0.0f);
        }
        foreach (Transform cell in cells2.Skip(1))
        {

            hits_cell.Add(cell, 0);
            probability_cell.Add(cell, 0.0f);
        }
        foreach (Transform cell in cells3.Skip(1))
        {

            hits_cell.Add(cell, 0);
            probability_cell.Add(cell, 0.0f);
        }

    }
    #endregion
}