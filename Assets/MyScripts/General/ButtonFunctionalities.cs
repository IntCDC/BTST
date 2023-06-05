using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonFunctionalities : MonoBehaviour
{
    /**
    * All functionalities regarding the visualization in the GUI are controlled by this script.
    * 
    * 
    * 
    * 
    * **/

    #region Required variables
    TimeLine time;
    LoadParticipant load_p;
    StackedHeatMap heatMapStack;
    public GameObject particpantGameObject;
    public string[] paths;
    public string[] pathsWorldData;
 
    public GameObject parentPanel;
    public Toggle toggle_p;
    public Toggle[] toggles;
    Toggle currentToggle;

    int current_id;
    int[] ids=new int[10];
    ParticipantData participant;
    Trajectory trajectory;
    ActiveStateManager stateManager;
    SampleData sample_d;
    ReplayManager_General manager_replay;
    public int amount_sample = 20;
    public Slider sliderHeatMap;
    public Slider sliderSigma;
    public Slider sliderTime;

    GameObject replayManager;
    private bool loaded=false;

    HeatmapWall wall_heat;

    bool playPause = false;

    public Sprite play_s;
    public Sprite pause_s;
    public Button playPause_b;
    public Button increase;
    public Button decrease;

    #endregion
    private void Start()
    {
        //The gaze replay scene is loaded at runtime
         SceneManager.LoadScene("GazeReplay", LoadSceneMode.Additive);
        
        //The script componets for visualizing the gaze and movement data are initialized in here
        load_p = particpantGameObject.GetComponent<LoadParticipant>();
        participant = particpantGameObject.GetComponent<ParticipantData>();
        heatMapStack = particpantGameObject.GetComponent<StackedHeatMap>();        
        trajectory = particpantGameObject.GetComponent<Trajectory>();
        stateManager = particpantGameObject.GetComponent<ActiveStateManager>();
        sample_d = particpantGameObject.GetComponent<SampleData>();
        time = particpantGameObject.GetComponent<TimeLine>();
       
        wall_heat = particpantGameObject.GetComponent<HeatmapWall>();

        //used to change intensity of heatmap projected on the wall
        sliderHeatMap.onValueChanged.AddListener(delegate { wall_heat.getHeatOnWall(sliderTime.value); });
        //used to change intensity of heatmap in STC
        sliderHeatMap.onValueChanged.AddListener(delegate { heatMapStack.participantActivated(); });
        sliderSigma.onValueChanged.AddListener(delegate { wall_heat.setSigmaValue(sliderSigma); });
        sliderSigma.onValueChanged.AddListener(delegate { heatMapStack.setSigmaValue(sliderSigma); });

        increase.onClick.AddListener(delegate { trajectory.stateParticpantChanged(); });
        decrease.onClick.AddListener(delegate { trajectory.stateParticpantChanged(); });

        increase.onClick.AddListener(delegate { heatMapStack.participantActivated(); });
        decrease.onClick.AddListener(delegate { heatMapStack.participantActivated(); });

    }

    #region Managing data
    public void sampling_amount(Slider slider)
    {
        amount_sample = (int)slider.value;
       
    }

    public void getCurrentToggle(Toggle t)
    {
        currentToggle = t;
    }

    //loads the data of the participant into the visualization
    public void loadPath(int ID)
    {

        current_id = ID;
        //Checks if already loaded, if loaded ids[currentid] is not 0, else 0
        if (ids[current_id-1]==0)
        {
            load_p.loadOldGazeData(paths[current_id-1], current_id, particpantGameObject);
            load_p.loadOldWorldData(pathsWorldData[current_id-1],current_id,particpantGameObject);
        

            ids[current_id-1] = current_id;
            sample_d.sampleData(amount_sample, current_id,participant);
        
        }
    }

    public void onStateChanged()
    {
        stateManager.informClasses(trajectory,heatMapStack);
        stateManager.ParticipantStateChanged(current_id,currentToggle.isOn);
        if (loaded)
        {
            manager_replay.instantiate_p(stateManager, particpantGameObject);
        }  
    }
    #endregion 
    #region Visualization related
    public void showTrajectory()
    {
        trajectory.LoadPath(current_id, particpantGameObject,stateManager);
    }
    public void stackHeatMap(Slider value)
    {       
        heatMapStack.placeStacks(particpantGameObject, trajectory, toggles,stateManager,value);
        wall_heat.initialize(particpantGameObject);
    }
    public void heatmap_Wall()
    {
        wall_heat.initialize(particpantGameObject);
    }
    #endregion

    #region timeline related
    public void getCurrentTime(Slider timeSlider)
    {
        time.getParticipantData(particpantGameObject);
        time.getCurrentTime(timeSlider);
        manager_replay.positionPlayer(timeSlider.value);
        wall_heat.getHeatOnWall(timeSlider.value); 
    }

    bool play;
    bool pause;
    public void playPause_function()
    {
        playPause = !playPause;
        if (playPause)
        {
            play = true;
            pause = false;
            playPause_b.image.sprite = pause_s;
        }
        else
        {
            pause = true;
            play = false;
            playPause_b.image.sprite = play_s;
        }
    }
    float dragLeftMinute = 0f;
    float dragRightMinute = 480000f;
    public void leftDrag(GameObject currentDragSlider)
    {
        dragLeftMinute = currentDragSlider.GetComponent<dragTimeSpan>().currentMinute;
        sliderTime.value = (1 / (time.totalMinutes * 1000f)) * dragLeftMinute;
        time.setLeftDragTime(dragLeftMinute);
    }

    public void rightDrag(GameObject currentDragSlider)
    {
        dragRightMinute = currentDragSlider.GetComponent<dragTimeSpan>().currentMinute;
        time.setRightDragTime(dragRightMinute);
    }
    public void timeSpanScale()
    {
        time.ScaleTimeSpan();
    }

    #endregion
    private void Update()
    {
        //Since the ReplayManager script is in a different Scene, the corresponding functionalities in this script need to be called during runtime.
        // Update checks, wheter the scene is loaded and sets the manager_replay variable acocrdingly.
        try
        {
            if (!loaded)
            {
                if (SceneManager.GetSceneAt(0).isLoaded)
                {
                    
                    replayManager = GameObject.Find("ReplayManager");

                    manager_replay = replayManager.GetComponent<ReplayManager_General>();

                }
                loaded = true;
            }
        }
        catch (NullReferenceException e)
        {

        }

        if (play)
        {
            if (sliderTime.value < (1 / (time.totalMinutes * 1000f)) * dragRightMinute)
            {
                sliderTime.value += 0.0007f;
            }

        }



    }

}
