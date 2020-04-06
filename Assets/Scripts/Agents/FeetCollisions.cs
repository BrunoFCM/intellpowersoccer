using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollisions : MonoBehaviour
{
    bool detected;
    public GameObject Ball;

    // Start is called before the first frame update
    void Start()
    {
        detected = false;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.name == Ball.name)
            detected = true;
    }


    public bool getDetected(){
        return detected;
    }

    public void SetDetectedFalse(){
        detected = false;
    }
}
