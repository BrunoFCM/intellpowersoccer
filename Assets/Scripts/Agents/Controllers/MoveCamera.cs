using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCamera : MonoBehaviour
{
    public float speedH = 0.5f;
    public float speedV = 0.5f;

    public Transform player;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float rotX = 20;
    private float rotY = 180;
    private float rotZ = 0;

    private float posX = 0;
    private float posY = 1.1f;
    private float posZ = 0.2f;

    public Toggle toggle;
    private bool tog;


    // Use this for initialization
    void Start()
    {
        tog = false;
        //transform.localPosition = new Vector3(posX, posY, posZ);
        //transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
    }

    // Update is called once per frame
    void Update()
    {
        if(toggle.IsActive()){
            if(toggle.isOn){
                tog = true;
            }
            else{
                tog = false;
                transform.localEulerAngles =  new Vector3(12.888f, 180, 0);
            }
        }
        

        if(tog){
            if(!(Input.GetAxis("Mouse X") >= -0.1f && Input.GetAxis("Mouse X") <= 0.1f) ||
                !(Input.GetAxis("Mouse Y") >= -0.1f && Input.GetAxis("Mouse Y") <= 0.1f)){

                yaw += speedH * Input.GetAxis("Mouse X");
                pitch -= speedV * Input.GetAxis("Mouse Y");

                yaw = Mathf.Clamp(yaw, 120f, 240f);
                pitch = Mathf.Clamp(pitch, 0f, 25f);

                transform.eulerAngles = new Vector3(pitch, yaw+player.localEulerAngles.y, 0.0f);
            }
            else if(!(Input.GetAxis("Right X Joystick") >= -0.2f && Input.GetAxis("Right X Joystick") <= 0.2f) ||
                !(Input.GetAxis("Right Y Joystick") >= -0.2f && Input.GetAxis("Right Y Joystick") <= 0.2f)){

                float newSpeedH = speedH + 1.5f;
                float newSpeedV = speedV + 1.5f;

                yaw += newSpeedH * Input.GetAxis("Right X Joystick");
                pitch -= newSpeedV * Input.GetAxis("Right Y Joystick");

                yaw = Mathf.Clamp(yaw, 120f, 240f);
                pitch = Mathf.Clamp(pitch, 0f, 25f);

                transform.eulerAngles = new Vector3(pitch, yaw+player.localEulerAngles.y, 0.0f);
            }
            else{
                //transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
            }
        }
    }
}