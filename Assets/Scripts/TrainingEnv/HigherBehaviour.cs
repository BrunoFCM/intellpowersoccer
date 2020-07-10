using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;

public class HigherBehaviour : Agent
{
    Rigidbody agentRBody;
    public BehaviourHandler behaviourHandler;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    public StrikeTheBallTrainer strikeTheBallTrainer;
    public PassTheBallTrainer passTheBallTrainer;
    public GoalKeepTrainer goalKeepTrainer;
    public Ball Ball;

    //1 means that a teammate has the ball, and 0 a opponent
    int teamBool;
    float elapsed = 0f;
   
     

    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
        teamBool = 0;        
    }

    void Update()
    {
        setTeamBool();

        elapsed += Time.deltaTime;
        if(elapsed >= 1f) {
            elapsed = elapsed % 1f;
            ballPossessionOverTime();
        }

        checkDistances();
    }

    private void FixedUpdate() {

        if(agentOutOfPlay()){
            Done();
        }
        
        if(ballOutOfPlay()){
            Done();
        }
    }

    public override void InitializeAgent() 
    {
        
    }

    public override float[] Heuristic()
    {
        var action = new float[3];


        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            action[0] = 2f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[1] = 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2f;
        }


        //DISABLE ALL BEHAVIOURS
        if (Input.GetKey(KeyCode.V))
        {
            action[2] = 1f;
        }
        //DEFEND THE BALL
        if (Input.GetKey(KeyCode.B))
        {
            action[2] = 2f;
        }
        //SHOOT THE BALL
        if (Input.GetKey(KeyCode.N))
        {
            action[2] = 3f;
        }
        //PASS THE BALL
        if (Input.GetKey(KeyCode.M))
        {
            action[2] = 4f;
        }

        return action;
    }

    public override void CollectObservations()
    {
        AddVectorObs(agentRBody.angularVelocity.magnitude);
        AddVectorObs(agentCore.distanceToBall());
        AddVectorObs(angleBetweenAgentAndBall());
        AddVectorObs(teamBool);
    }

    public override void AgentAction(float[] vectorAction)
    {
        if (vectorAction[0] == 1)
        {
            vectorAction[1] = 1;
            vectorAction[0] = 0;
            controller.Controller(vectorAction);
        }
        else if (vectorAction[0] == 2)
        {
            vectorAction[1] = 0;
            vectorAction[0] = -1;
            controller.Controller(vectorAction);
        }
        else if (vectorAction[1] == 1)
        {
            vectorAction[1] = -1;
            vectorAction[0] = 0;
            controller.Controller(vectorAction);
        }
        else if (vectorAction[1] == 2)
        {
            vectorAction[1] = 0;
            vectorAction[0] = 1;
            controller.Controller(vectorAction);
        }
        else if(vectorAction[2] == 1){
            behaviourHandler.disableAllBehaviours();
        }
        else if(vectorAction[2] == 2){
            behaviourHandler.setGoalKeepBehaviour(gameEnvironment.getNearestPlayerToBall());
        }
        else if(vectorAction[2] == 3){
            behaviourHandler.setStrikeTheBallBehaviour();
        }
        else if(vectorAction[2] == 4){
            behaviourHandler.setPassTheBallBehaviour();
        }
        
    }

    public override void AgentReset()
    {        
        gameEnvironment.resetGame();
    }


    // -------------------------------------------------- TRAINER GENERIC FUNCS -------------------------------------------------- //


    public float AngleDir(Vector3 fwd, Vector3 targetDir){
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, Vector3.up);

        return Mathf.Sign(dir);
    }

    public float angleBetweenAgentAndBall(){
        Vector3 agentToBallVec = Ball.transform.localPosition - agentCore.transform.localPosition;
        Vector3 agentToForwardVec = agentCore.transform.forward*-1;

        return Vector3.Angle(agentToForwardVec, agentToBallVec) * AngleDir(agentToForwardVec, agentToBallVec);
    }

    public void setTeamBool(){
        if(gameEnvironment.getNearestPlayerToBall() != null)
            if(agentCore.team == AgentCore.Team.BLUE){
                if(gameEnvironment.getNearestPlayerToBall().team == AgentCore.Team.BLUE){
                    teamBool = 1;
                }
                else{
                    teamBool = 0;
                }
            }else{
                if(gameEnvironment.getNearestPlayerToBall().team == AgentCore.Team.BLUE){
                    teamBool = 0;
                }
                else{
                    teamBool = 1;
                }
            }
    }


    // -------------------------------------------------- REWARDS SYSTEM -------------------------------------------------- //

    public void foulCommited(){
        //AddReward(-3f);
    }

    public void ballOutOfBounds(){
        //AddReward(-0.05f);
    }

    public void goalScored(AgentCore.Team team){
        setTeamReward(team, 10f);
    }

    public void goalSuffered(AgentCore.Team team){
        setTeamReward(team, -10f);
        Done();
    }

    public void ballPossessionOverTime(){
        /*if(gameEnvironment.getNearestPlayerToBall() != null){
            if(gameEnvironment.getNearestPlayerToBall().team == AgentCore.Team.BLUE){
                addTeamReward(AgentCore.Team.BLUE, 0.002f);
            }else{
                addTeamReward(AgentCore.Team.RED, 0.002f);
            }
        }*/
    }

    public void matchWinner(AgentCore.Team winTeam){
        /*addTeamReward(winTeam, 10f);
        Debug.Log("MATCH OVER - WINNER IS" + winTeam);*/
        Done();
    }

    public void tie(){
        /*addTeamReward(AgentCore.Team.BLUE, 3f);
        addTeamReward(AgentCore.Team.RED, 3f);
        Debug.Log("MATCH OVER TIE");*/
        Done();
    }

    public void setTeamReward(AgentCore.Team team, float reward){
        if(team == AgentCore.Team.BLUE){
            foreach(AgentCore agent in gameEnvironment.blueTeamAgents){
                agent.higherBehaviour.SetReward(reward);
            }
        }else{
            foreach(AgentCore agent in gameEnvironment.redTeamAgents){
                agent.higherBehaviour.SetReward(reward);
            }
        }
    }

    public void addTeamReward(AgentCore.Team team, float reward){
        if(team == AgentCore.Team.BLUE){
            foreach(AgentCore agent in gameEnvironment.blueTeamAgents){
                agent.higherBehaviour.AddReward(reward);
            }
        }else{
            foreach(AgentCore agent in gameEnvironment.redTeamAgents){
                agent.higherBehaviour.AddReward(reward);
            }
        }
    }

    public void checkDistances(){
        foreach(AgentCore agent in gameEnvironment.redTeamAgents){
            if(agentCore.distanceToPlayer(agent) < 2){
                AddReward(-0.1f);
            }
        }
        foreach(AgentCore agent in gameEnvironment.blueTeamAgents){
            if(agentCore.distanceToPlayer(agent) < 2){
                AddReward(-0.1f);
            }
        }
    }



    // -------------------------------------------------- VERIFICATIONS -------------------------------------------------- //

    public bool agentOutOfPlay(){
        if(agentCore.transform.localPosition.x > 16.5 || agentCore.transform.localPosition.x < -16.5){
            SetReward(-10f);
            Debug.Log("AGENT OUT OF PLAY");
            return true;
        }
        else if(agentCore.transform.localPosition.z > 10 || agentCore.transform.localPosition.z < -10){
            SetReward(-10f);
            Debug.Log("AGENT OUT OF PLAY");
            return true;
        }

        return false;
    }

    public bool ballOutOfPlay(){
        if(Ball.transform.localPosition.y < 0.2){
            SetReward(-10f);
            Debug.Log("BALL OUT OF PLAY");
            return true;
        }
        return false;
    }

}