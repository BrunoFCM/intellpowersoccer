using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollisions : MonoBehaviour
{
    public AgentCore agentCore;
    public GameObject Ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.name == Ball.name)
            agentCore.touchedBall();
    }
}
