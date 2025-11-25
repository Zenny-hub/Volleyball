using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    //Sensitivity
    public float sensX;
    public float sensY;

    public Transform orientation;
    

    float xRotation;
    float yRotation;


    

    private void Start()
    {
        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void Update()
    {


        //StateHandler();


        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Camera rotation via mouse
        yRotation += mouseX;
        xRotation -= mouseY;

        // Stops camera when looking up/down
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        
       
        
    }


}
