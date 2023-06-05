using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class switchOrthoPersp : MonoBehaviour
{
    public Material projectionPersp;
    public Material projectionOrtho;
    bool ortho = true;
    GameObject camOrtho;
    GameObject camPersp;
    bool loaded = false;
    public GameObject projectionPlane;
    public TextMeshProUGUI text;
    public void switchCamera()
    {
      
        ortho = !ortho;
        camOrtho.SetActive(ortho);
        camPersp.SetActive(!ortho);
        switch (ortho)
        {
            case true:               
                projectionPlane.GetComponent<MeshRenderer>().material = projectionOrtho;
                text.text = "Orthographic";
                break;
            case false:
                projectionPlane.GetComponent<MeshRenderer>().material = projectionPersp;
                text.text = "Perspective";
                break;
        }

    }
    private void Update()
    {
        try
        {
            if (!loaded)
            {
                if (SceneManager.GetSceneAt(0).isLoaded)
                {


                    camOrtho = GameObject.Find("TopVIewOrtho");
                    camPersp = GameObject.Find("TopVIewPersp");

                    camPersp.SetActive(false);


                }
                loaded = true;
            }
        }
        catch (NullReferenceException e)
        {

        }



    }
}
