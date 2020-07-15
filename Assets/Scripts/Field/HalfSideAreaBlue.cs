using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfSideAreaBlue : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Ball Ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.name == Ball.name){
            //Debug.Log("ENJKDBKAKBAKJA FODASSE");
            gameEnvironment.setBallOutOfBoundsTimeOut(false);
            gameEnvironment.setOutOfBounds(false);
            Ball.setPositionInField(Ball.Areas.halfFieldBlue);
        }

        foreach(AgentCore agentCore in gameEnvironment.redTeamAgents){
            if (collision.name == agentCore.transform.GetChild(13).name){
                agentCore.setPlayersAtHalfSideAreaBlue();
            }
        }
        foreach(AgentCore agentCore in gameEnvironment.blueTeamAgents){
            if (collision.name == agentCore.transform.GetChild(13).name){
                agentCore.setPlayersAtHalfSideAreaBlue();
            }
        }
    }
}
