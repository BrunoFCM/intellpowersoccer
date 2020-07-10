using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollisions : MonoBehaviour
{
    public AgentCore agentCore;
    public GameObject Ball;
    //public DribbleBallTrainer dribbleBallTrainer;
    //public PassTheBallTrainer passTheBallTrainer;
    //public StrikeTheBallTrainer strikeTheBallTrainer;
    //public GoalKeepTrainer goalKeepTrainer;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == Ball.name){
            //Ball.GetComponent<Rigidbody>().velocity = new Vector3 (collision.relativeVelocity.x, collision.relativeVelocity.y, collision.relativeVelocity.z);
            agentCore.touchedBall();
            //dribbleBallTrainer.touchedBall();
            //passTheBallTrainer.touchedBall();

            /*  -- ONLY FOR TRAINING --
        
            if(agentCore.name == "Agent1")
                strikeTheBallTrainer.touchedBall();
            else if(agentCore.name == "Agent2")
                goalKeepTrainer.touchedBall();

            */
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.name == Ball.name){
            //Ball.GetComponent<Rigidbody>().velocity = new Vector3 (collision.relativeVelocity.x, collision.relativeVelocity.y, collision.relativeVelocity.z);
            agentCore.touchedBall();
            
            /*  -- ONLY FOR TRAINING --
        
            if(agentCore.name == "Agent1")
                strikeTheBallTrainer.touchedBall();
            else if(agentCore.name == "Agent2")
                goalKeepTrainer.touchedBall();

            */
        }
    }
}
