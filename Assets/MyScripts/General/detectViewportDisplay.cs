using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class detectViewportDisplay : MonoBehaviour
{

    /**
     * This Class sitches between the Cameras within the Gaze Replay and the STC based on the Cursor position    
     */

    GameObject cams;
    Camera[] cameras;
    Camera activeCam;
    bool loaded = false;
    public Camera main_c;
   
     void Update()
    {
        try
        {
            if (!loaded)
            {
                if (SceneManager.GetSceneAt(0).isLoaded)
                {


                    cams = GameObject.Find("Cams");

                    cameras = cams.GetComponentsInChildren<Camera>();

                    activeCam = cameras[0];
                    
                }
                loaded = true;
            }


            if (main_c.ScreenToViewportPoint(Input.mousePosition).x > 0)
            {
                activeCam.GetComponent<MoveCam>().enabled = false;
                main_c.GetComponent<MoveCam>().enabled = true;
            }
            else
            {
                activeCam.GetComponent<MoveCam>().enabled = true;
                main_c.GetComponent<MoveCam>().enabled = false;
            }
        }
        catch (NullReferenceException e)
        {

        }




    }
}
