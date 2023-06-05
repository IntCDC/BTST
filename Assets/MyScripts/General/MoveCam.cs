using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        rotZ = 0.0f;
    }
    public float rotationSpeed = 40f;
    public float moveSpeed = 9f;
    float inputX, inputZ,ctrl,alt;
    private float rotZ;
    public float keyboardSensitivity=15f;
    private Quaternion localRotation;
    private bool rotateyactive=false;

    // Update is called once per frame
    void Update()
    {
        

        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
        ctrl = Input.GetAxis("Fire1");
        alt = Input.GetAxis("Fire2");

        if (inputX != 0 && ctrl ==0 &&alt==0) 
        {
            moveRight();
        }
        if (inputZ != 0 && ctrl == 0 && alt == 0) 
        {
            moveForward();
        }
        if (ctrl !=0 && inputZ !=0 && alt == 0)
        {
            moveUp();
        }
        if (alt != 0 && inputX != 0 &&ctrl ==0)
        {
            rotateX();
        }

        if (alt != 0 && inputZ != 0 && ctrl == 0)
        {
            rotateY();
        }
        /**
        if (Input.GetKey(KeyCode.Q))
        {
            rotZ += Time.deltaTime * keyboardSensitivity;
            localRotation = Quaternion.Euler(rotZ, 0.0f,0.0f );
            transform.rotation = localRotation;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotZ -= Time.deltaTime * keyboardSensitivity;
            localRotation = Quaternion.Euler(rotZ, 0.0f, 0.0f);
            transform.rotation = localRotation;
        }
        **/
    }

    private void rotateY()
    {
        //localRotation = Quaternion.Euler(inputZ * Time.deltaTime * rotationSpeed, 0.0f,0.0f);
        //transform.rotation = localRotation;
        // transform.Rotate(new Vector3(inputZ * Time.deltaTime * rotationSpeed,0f, 0f));
        transform.RotateAround(transform.position, transform.right, inputZ * Time.deltaTime * rotationSpeed);
    }

    private void moveUp()
    {
        
          //  Quaternion currentRotation = transform.rotation;
         //   transform.rotation = new Quaternion(0,0,0,0);
            transform.position += Vector3.up * inputZ * Time.deltaTime * moveSpeed;
           // transform.rotation *= currentRotation;       
    }

    private void moveRight()
    {
      //  Quaternion currentRotation = transform.rotation;
      //  transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.position += transform.right * inputX * Time.deltaTime * moveSpeed;
      //  transform.rotation *= currentRotation;
    }

    private void moveForward()
    {
        //  Quaternion currentRotation = transform.rotation;
        //  transform.rotation = new Quaternion(0, 0, 0, 0);
   
            transform.position += transform.forward * inputZ * Time.deltaTime * moveSpeed;
   
     
       // Debug.Log("Foread vec"+transform.forward);
       // transform.rotation *= currentRotation;
    }

    private void rotateX()
    {
        //transform.Rotate(new Vector3(0f, inputX*Time.deltaTime*rotationSpeed,0f));
        transform.RotateAround(transform.position, Vector3.up, inputX * Time.deltaTime * rotationSpeed);
        
      //  print("RotateX"+inputX);
    }


 
}
