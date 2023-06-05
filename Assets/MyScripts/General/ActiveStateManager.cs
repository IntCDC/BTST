using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActiveStateManager : MonoBehaviour
{
    /**
     * saves the currentState of the ParticipantList
     * **/
    public Dictionary<int, bool> activeParticipants;
    Trajectory t;
    StackedHeatMap heatMap;
    UnityEvent stateChange;
    bool listenerCalled = false;
    private void Start()
    {
        activeParticipants = new Dictionary<int, bool>();
        stateChange = new UnityEvent();
     
 
    }

    
    public void ParticipantStateChanged(int id,bool state) 
    {
        
        if (!(activeParticipants.ContainsKey(id)))
        {
            activeParticipants.Add(id,state);
        }
        else
        {
           
            activeParticipants[id] = state;
          
        }
        stateChange.Invoke();
    }

    public void informClasses(Trajectory trajectory,StackedHeatMap stack)
    {
        this.t = trajectory;
        heatMap = stack;
        if (!listenerCalled)
        {
          
            stateChange.AddListener(t.stateParticpantChanged);
            stateChange.AddListener(heatMap.participantActivated);
            listenerCalled = true;
        }

    }
}
