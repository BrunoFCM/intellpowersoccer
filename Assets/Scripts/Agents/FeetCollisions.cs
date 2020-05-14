using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollisions : MonoBehaviour
{
    public AgentCore agentCore;
    public GameObject Ball;
    public PassTheBallTrainer passTheBallTrainer;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == Ball.name){
            //Ball.GetComponent<Rigidbody>().velocity = new Vector3 (collision.relativeVelocity.x, collision.relativeVelocity.y, collision.relativeVelocity.z);
            agentCore.touchedBall();
            passTheBallTrainer.touchedBall();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.name == Ball.name){
            //Ball.GetComponent<Rigidbody>().velocity = new Vector3 (collision.relativeVelocity.x, collision.relativeVelocity.y, collision.relativeVelocity.z);
            agentCore.touchedBall();
            passTheBallTrainer.touchedBall();
        }
    }
}
