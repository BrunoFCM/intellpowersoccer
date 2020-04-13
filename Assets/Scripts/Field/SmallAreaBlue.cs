using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAreaBlue : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Collider Ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.name == Ball.name){
            gameEnvironment.setBallOutOfBoundsTimeOut(false);
            gameEnvironment.setOutOfBounds(false);
            Debug.Log("Ball in SmallSide Blue");
        }

        foreach(AgentCore agentCore in gameEnvironment.redTeamAgents){
            if (collision.name == agentCore.name){
                agentCore.setPlayersAtSmallAreaBlue();
            }
        }
    }
}
