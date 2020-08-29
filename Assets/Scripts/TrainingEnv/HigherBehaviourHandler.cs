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

        if(gameEnvironmentInfo.getNearestPlayerToBall() != null)
            if(!pause)
                behaviorHandler();
        
    }

    void FixedUpdate() {

        

        /*if(playerWithBallCopy != gameEnvironmentInfo.getNearestPlayerToBall()){
            playerWithBallCopy.disableAllBehaviours();
            playerWithBallCopy = gameEnvironmentInfo.getNearestPlayerToBall();
        }*/

    }

    void LateUpdate() {
        
    }

    public void behaviorHandler(){

        //Debug.Log("ENTRA, FDS");

        AgentCore nearestPlayer = gameEnvironmentInfo.getNearestPlayerToBall();
        AgentCore nearestOpponent = gameEnvironmentInfo.getNearestOpponentWithBall(nearestPlayer);

        
        foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
            if(agentIsOutOfBounds(agent)){
                //Debug.Log("FORA");
                if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 20f)
                    agent.setMoveToPointBehaviour(newFracPoint(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y), 1f));
                else
                    agent.setMoveToPointBehaviour(agent.getOriginalPosition());
            }
            else if (agent.type == AgentCore.Type.GOALKEEPER && agent != nearestPlayer){
                if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) < 1f){    
                    if(Vector3.Distance(new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y), gameEnvironmentInfo.Ball.transform.position) < 3 && agent != nearestPlayer){
                        agent.setGoalKeepBehaviour(nearestOpponent);
                    }
                    else{
                        agent.disableAllBehaviours();
                    }
                }else{
                    agent.setMoveToPointBehaviour(agent.getOriginalPosition());
                }
            }
            else if(agent == nearestPlayer){
                if(agent.getDistanceToGoal() < 3.5f){
                    agent.setStrikeTheBallBehaviour();
                }
                else{
                    agent.setDribbleBallBehaviour(getVoronoiNewPoint(agent));
                }
            }
            else if(agent == nearestOpponent){
                if(Vector3.Distance(agent.transform.position, gameEnvironmentInfo.Ball.transform.position) < 4f)
                    setIntersectBallBehaviour(nearestPlayer, agent);
            }
            else{
                /*if(agent.getOriginalPosition() != null)
                    if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 2.5f){
                        if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 20f)
                            agent.setMoveToPointBehaviour(newFracPoint(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y), 1f));
                        else
                            agent.setMoveToPointBehaviour(agent.getOriginalPosition());
                    }
                    else
                        agent.disableAllBehaviours();*/
                /*if(teamIsAttacking(agent, nearestPlayer)){
                    agent.setMoveToPointBehaviour(
                        getPointAtXDistance(getVoronoiNewAttackingPoint(agent, nearestPlayer),
                        new Vector2(nearestPlayer.transform.position.x, nearestPlayer.transform.position.z),
                        3)
                    );
                }
                else{
                    agent.setMoveToPointBehaviour(
                        getPointAtXDistance(getVoronoiNewDefensivePoint(agent, nearestOpponent),
                        new Vector2(nearestOpponent.transform.position.x, nearestOpponent.transform.position.z),
                        3)
                    );
                }*/
                /*Vector2 pos = getVoronoiNewDefensivePoint(agent, nearestOpponent);
                if(Vector3.Distance(agent.transform.position, new Vector3(pos.x, 0, pos.y)) > 1f)
                    agent.setMoveToPointBehaviour(pos);
                else
                    agent.disableAllBehaviours();*/

                Vector2 pos = getOffensivePoint(agent);
                if(Vector3.Distance(agent.transform.position, new Vector3(pos.x, 0, pos.y)) > 1.0f)
                    agent.setMoveToPointBehaviour(pos);
                else
                    agent.disableAllBehaviours();
            }
        }

        foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
            if(agentIsOutOfBounds(agent)){
                Debug.Log("FORA");
                if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 20f)
                    agent.setMoveToPointBehaviour(newFracPoint(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y), 1f));
                else
                    agent.setMoveToPointBehaviour(agent.getOriginalPosition());
            }
            else if (agent.type == AgentCore.Type.GOALKEEPER && agent != nearestPlayer){
                if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) < 1f){    
                    if(Vector3.Distance(new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y), gameEnvironmentInfo.Ball.transform.position) < 3 && agent != nearestPlayer){
                        agent.setGoalKeepBehaviour(nearestOpponent);
                    }
                    else{
                        agent.disableAllBehaviours();
                    }
                }else{
                    agent.setMoveToPointBehaviour(agent.getOriginalPosition());
                }
            }
            else if(agent == nearestPlayer){
                //Debug.Log(agent.getDistanceToGoal());
                if(agent.getDistanceToGoal() < 3.5f){
                    agent.setStrikeTheBallBehaviour();
                }
                else{
                    agent.setDribbleBallBehaviour(getVoronoiNewPoint(agent));
                }
            }
            else if(agent == nearestOpponent){
                if(Vector3.Distance(agent.transform.position, gameEnvironmentInfo.Ball.transform.position) < 4.0f)
                    setIntersectBallBehaviour(nearestPlayer, agent);
            }
            else{
                /*if(agent.getOriginalPosition() != null)
                    if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 2.5f){
                        if(Vector3.Distance(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y)) > 20f)
                            agent.setMoveToPointBehaviour(newFracPoint(agent.transform.position, new Vector3(agent.getOriginalPosition().x, 0, agent.getOriginalPosition().y), 1f));
                        else
                            agent.setMoveToPointBehaviour(agent.getOriginalPosition());
                    }
                    else
                        agent.disableAllBehaviours();*/
                /*if(teamIsAttacking(agent, nearestPlayer)){
                    agent.setMoveToPointBehaviour(
                        getPointAtXDistance(getVoronoiNewAttackingPoint(agent, nearestPlayer),
                        new Vector2(nearestPlayer.transform.position.x, nearestPlayer.transform.position.z),
                        3)
                    );
                }
                else{
                    agent.setMoveToPointBehaviour(
                        getPointAtXDistance(getVoronoiNewDefensivePoint(agent, nearestOpponent),
                        new Vector2(nearestOpponent.transform.position.x, nearestOpponent.transform.position.z),
                        3)
                    );
                    agent.setMoveToPointBehaviour(getVoronoiNewDefensivePoint(agent, nearestOpponent));
                }*/
                /*Vector2 pos = getVoronoiNewDefensivePoint(agent, nearestOpponent);
                if(Vector3.Distance(agent.transform.position, new Vector3(pos.x, 0, pos.y)) > 2.5f)
                    agent.setMoveToPointBehaviour(pos);
                else
                    agent.disableAllBehaviours();*/

                Vector2 pos = getOffensivePoint(agent);
                if(Vector3.Distance(agent.transform.position, new Vector3(pos.x, 0, pos.y)) > 1.5f)
                    agent.setMoveToPointBehaviour(pos);
                else
                    agent.disableAllBehaviours();

            }
            
        }
    }

    public Vector2 getOffensivePoint(AgentCore agent){

        float distance;

        if(Vector3.Distance(agent.transform.position, gameEnvironmentInfo.Ball.transform.position) < 5)
            distance = 0.6f;
        else if(Vector3.Distance(agent.transform.position, gameEnvironmentInfo.Ball.transform.position) < 4)
            distance = 0.7f;
        else if(Vector3.Distance(agent.transform.position, gameEnvironmentInfo.Ball.transform.position) < 3)
            return agent.getOriginalPosition();
        else
            distance = 0.4f;

        /*if(agent.team == AgentCore.Team.RED){
            distance = -3;
        }
        else{
            distance = -3;
        }*/
        
        Vector2 newPoint;
        Vector2 originalPos = agent.getOriginalPosition();
        Vector2 currentPos = new Vector2(agent.transform.position.x, agent.transform.position.z);
        Vector2 ballPos = new Vector2(gameEnvironmentInfo.Ball.transform.position.x, gameEnvironmentInfo.Ball.transform.position.z);
        Vector2 intersectPoint = new Vector2(ballPos.x, originalPos.y);

        newPoint = Vector2.Lerp(originalPos, intersectPoint, distance);

        return newPoint;
    }

    public Vector2 getDefensivePoint(AgentCore agent){
        float d = 4f;
        /*if(agent.team == AgentCore.Team.RED){
            distance = -3;
        }
        else{
            distance = -3;
        }*/
        
        //Vector2 newPoint;
        Vector2 originalPos = agent.getOriginalPosition();
        Vector2 currentPos = new Vector2(agent.transform.position.x, agent.transform.position.z);
        Vector2 ballPos = new Vector2(gameEnvironmentInfo.Ball.transform.position.x, gameEnvironmentInfo.Ball.transform.position.z);
        Vector2 intersectPoint = new Vector2(ballPos.x, originalPos.y);

        //newPoint = Vector2.Lerp(originalPos, intersectPoint, distance);

        float t = d/Vector2.Distance(originalPos, intersectPoint);

        return new Vector2(((1-t)*intersectPoint.x+t*originalPos.x), ((1-t)*intersectPoint.y+t*originalPos.y));

        //return newPoint;
    }

    public Vector2 getPointAtXDistance(Vector2 p1, Vector2 p2, float d){

        if(Vector2.Distance(p1,p2) > d){
            return p1;
        }

        float t = d/Vector2.Distance(p1,p2);

        return new Vector2(((1-t)*p1.x+t*p2.x), ((1-t)*p1.y+t*p2.y));
    }

    public bool teamIsAttacking(AgentCore agent, AgentCore nearestAgent){
        if(agent.team == nearestAgent.team)
            return true;
        else
            return false;
    }

    public void setPause(bool p){
        pause = p;
        
        if(!p){
            foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
            agent.enableController();
        }

        foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
            agent.enableController();
        }
        }
    }

    public Vector2 newFracPoint(Vector3 p1, Vector3 p2, float frac){
        
        //Vector3 P = Vector3.Lerp(A, B, x / (A-B).Length());

        Vector3 newPoint = 2.5f * Vector3.Normalize(p2 - p1) + p1;

        //Vector3 newPoint = Vector3.Lerp(p1, p2, 3 / (p1-p2).);

        return new Vector2(newPoint.x, newPoint.z);
    }

    public bool agentIsOutOfBounds(AgentCore agent){
        if(agent.transform.position.x < -13 ||  agent.transform.position.x > 13){
            return true;
        }
        else if(agent.transform.position.z < -6.5f ||  agent.transform.position.z > 6.5f){
            return true;
        }

        return false;
    }

    public Vector2 getVoronoiNewPoint(AgentCore agent){
        Vector2 pointToGo = voronoiPort.getPointToGo((int)Char.GetNumericValue(agent.tag[5]));

        return new Vector2(pointToGo.x-14, pointToGo.y-7.5f);        
    }

    public Vector2 getVoronoiNewAttackingPoint(AgentCore agent, AgentCore playerWithBall){
        Vector2 pointToGo = voronoiPort.getPointToGoAttacking((int)Char.GetNumericValue(agent.tag[5]), playerWithBall);

        return new Vector2(pointToGo.x-14, pointToGo.y-7.5f);        
    }

    public Vector2 getVoronoiNewDefensivePoint(AgentCore agent, AgentCore playerWithBall){
        Vector2 pointToGo = voronoiPort.getPointToGoDefending((int)Char.GetNumericValue(agent.tag[5]), playerWithBall);

        return new Vector2(pointToGo.x-14, pointToGo.y-7.5f);        
    }

    public void setIntersectBallBehaviour(AgentCore agent, AgentCore nearestOpponent){
        if(gameEnvironmentInfo.getNearestPlayerToBall() != null){
            nearestOpponent.setIntersectBallBehaviour(agent, nearestOpponent);
        }
    }

    public void pauseHandler(AgentCore nearestPlayer){
        //Debug.Log("PAUSE");
        setPause(true);

        foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
            if(agent != nearestPlayer){
                agent.disableAllBehaviours();
                agent.disableController();
            }
        }

        foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
            if(agent != nearestPlayer){
                agent.disableAllBehaviours();
                agent.disableController();
            }
        }

        nearestPlayer.setPassTheBallBehaviour();
    }

    public void penaltyPauseHandler(AgentCore nearestPlayer, AgentCore defendingPlayer){
        Debug.Log("PAUSE PENALTY");
        setPause(true);

        //nearestPlayer.setStrikeTheBallBehaviour();

        foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
            if(agent != nearestPlayer && agent != defendingPlayer){
                agent.disableAllBehaviours();
                agent.disableController();
            }
        }

        foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
            if(agent != nearestPlayer && agent != defendingPlayer){
                agent.disableAllBehaviours();
                agent.disableController();
            }
        }

        //nearestPlayer.setPassTheBallBehaviour();
    }
}
