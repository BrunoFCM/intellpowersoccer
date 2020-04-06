﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfSideAreaRed : MonoBehaviour
{
    bool detected;
    public GameObject Agent;

    // Start is called before the first frame update
    void Start()
    {
        detected = false;
    }

    private void OnTriggerStay(Collider collision) {
        if (collision.name == Agent.name){
            detected = true;
        }
    }


    public bool getDetected(){
        return detected;
    }

    public void SetDetectedFalse(){
        detected = false;
    }
}