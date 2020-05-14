using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAreaBlue : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Ball Ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerStay(Collider collision) {
        if (collision.name == Ball.name){
            gameEnvironment.setBallOutOfBoundsTimeOut(false);
            gameEnvironment.setOutOfBounds(false);
            Ball.setPositionInField(Ball.Areas.smallBlueArea);

        }

        foreach(AgentCore agentCore in gameEnvironment.redTeamAgents){
            if (collision.name == agentCore.transform.GetChild(13).name){
                agentCore.setPlayersAtSmallAreaBlue();
            }
        }
        foreach(AgentCore agentCore in gameEnvironment.blueTeamAgents){
            if (collision.name == agentCore.transform.GetChild(13).name){
                agentCore.setPlayersAtSmallAreaBlue();
            }
        }
    }
}
