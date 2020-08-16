using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class MoveCamera : MonoBehaviour
{
    public float speedH = 1f;
    public float speedV = 1f;

    public Transform player;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private float rotX = 20;
    private float rotY = 180;
    private float rotZ = 0;

    private float posX = 0;
    private float posY = 1.1f;
    private float posZ = 0.2f;


    // Use this for initialization
    void Start()
    {
        transform.localPosition = new Vector3(posX, posY, posZ);
        transform.eulerAngles = new Vector3(rotX, rotY, rotZ);
    }

    // Update is called once per frame
    void Update()
    {

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, 100f, 260f);
        pitch = Mathf.Clamp(pitch, 15f, 25f);

        transform.eulerAngles = new Vector3(pitch, yaw+player.eulerAngles.y, 0.0f);

    }
}