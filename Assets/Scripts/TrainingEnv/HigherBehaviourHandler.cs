using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HigherBehaviourHandler : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironmentInfo;
    public VoronoiPort voronoiPort;
    private AgentCore playerWithBallCopy;
    float time;

    bool pause;

    // Start is called before the first frame update
    void Start()
    {
        pause=false;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*time -= Time.deltaTime;
		if(time < 1){
			if(gameEnvironmentInfo.getNearestPlayerToBall() != null)
                behaviorHandler();
			time = 1;
		}*/
        
    }

    void FixedUpdate() {

        

        /*if(playerWithBallCopy != gameEnvironmentInfo.getNearestPlayerToBall()){
            playerWithBallCopy.disableAllBehaviours();
            playerWithBallCopy = gameEnvironmentInfo.getNearestPlayerToBall();
        }*/

    }

    void LateUpdate() {
        if(gameEnvironmentInfo.getNearestPlayerToBall() != null)
            behaviorHandler();
    }

    public void behaviorHandler(){

        AgentCore nearestPlayer = gameEnvironmentInfo.getNearestPlayerToBall();
        AgentCore nearestOpponent = gameEnvironmentInfo.getNearestOpponentWithBall(nearestPlayer);

        if(gameEnvironmentInfo.checkGamePause()){
            Debug.Log("PAUSE");
            if(gameEnvironmentInfo.checkPenalty()){

            }
            else{
                foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
                    if(!agent == nearestPlayer){
                        agent.disableAllBehaviours();
                    }
                }

                foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
                    if(!agent == nearestPlayer){
                        agent.disableAllBehaviours();
                    }
                }

                nearestPlayer.setPassTheBallBehaviour();
            }
        }
        else{
            foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
                if(agent == nearestPlayer){
                    if(agent.getDistanceToGoal() < 7){
                        agent.setStrikeTheBallBehaviour();
                    }
                    else{
                        agent.setDribbleBallBehaviour(getVoronoiNewPoint(agent));
                    }
                }
                else if(agent == nearestOpponent){
                    setIntersectBallBehaviour(nearestPlayer, agent);
                }
                else{
                    if(agent.getOriginalPosition() != null)
                        if(Vector2.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 2f){
                            if(Vector2.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 4f)
                                agent.setMoveToPointBehaviour(Fraction((1/3), agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)));
                            else
                                agent.setMoveToPointBehaviour(agent.getOriginalPosition());
                        }
                        else
                            agent.disableAllBehaviours();
                }
            }

            foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
                if(agent == nearestPlayer){
                    if(agent.getDistanceToGoal() < 7){
                        agent.setStrikeTheBallBehaviour();
                    }
                    else{
                        agent.setDribbleBallBehaviour(getVoronoiNewPoint(agent));
                    }
                }
                else if(agent == nearestOpponent){
                    setIntersectBallBehaviour(nearestPlayer, agent);
                }
                else{
                    if(agent.getOriginalPosition() != null)
                        if(Vector2.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 2f){
                            if(Vector2.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 4f)
                                agent.setMoveToPointBehaviour(Fraction((1/3), agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)));
                            else
                                agent.setMoveToPointBehaviour(agent.getOriginalPosition());
                        }
                        else
                            agent.disableAllBehaviours();
                }
            }
        }
    }

    public Vector2 Fraction(float frac, Vector2 p1, Vector2 p2) {
        return new Vector2(p1.x + frac*(p2.x-p1.x), p1.y + frac*(p2.y-p1.y));
   }

    public Vector2 getVoronoiNewPoint(AgentCore agent){
        Vector2 pointToGo = voronoiPort.getPointToGo((int)Char.GetNumericValue(agent.tag[5]));

        return new Vector2(pointToGo.x-50, pointToGo.y-50);        
    }

    public void setIntersectBallBehaviour(AgentCore agent, AgentCore nearestOpponent){
        if(gameEnvironmentInfo.getNearestPlayerToBall() != null){
            nearestOpponent.setIntersectBallBehaviour(agent, nearestOpponent);
        }
    }
}
