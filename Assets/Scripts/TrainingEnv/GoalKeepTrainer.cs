using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;

public class GoalKeepTrainer : Agent
{
    Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    public StrikeTheBallTrainer strikeTheBallTrainer;
    public Ball Ball;
    Vector3 ballPos;
    AgentCore shooter;
    int site;
    bool oponentStrike;
     

    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
        ballPos = Vector3.zero;
        shooter = gameEnvironment.redTeamAgents[0];
        oponentStrike = false;
        site = -1;
    }

    void Update()
    {

    }

    private void FixedUpdate() {
        if(agentOutOfPlay()){
            Done();
        }
    }

    public override void InitializeAgent() 
    {
        agentRBody = GetComponent<Rigidbody>();
        ballPos = Vector3.zero;
        shooter = gameEnvironment.redTeamAgents[0];
        oponentStrike = false;
        site = -1;
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    public override void CollectObservations()
    {
        AddVectorObs(agentCore.distanceToBall());
        AddVectorObs(angleBetweenAgentAndBall());
        AddVectorObs(agentCore.distanceToPlayer(shooter));
        AddVectorObs(angleBetweenAgentAndShooter());
    }

    public override void AgentAction(float[] vectorAction)
    {
        controller.Controller(vectorAction);
    }

    public override void AgentReset()
    {        
        site = -1;
        oponentStrike = false;
        strikeTheBallTrainer.Done();
    }

    public void scoredRedGoal(){
        SetReward(-2);
        Debug.Log("GOAL SCORED REWARD -2");
        Done();
    }

    public void scoredBlueGoal(){
        SetReward(-2);
        Debug.Log("GOAL SCORED REWARD -2");
        Done();
    }

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

    public float angleBetweenAgentAndShooter(){
        Vector3 agentToBallVec = shooter.transform.localPosition - agentCore.transform.localPosition;
        Vector3 agentToForwardVec = agentCore.transform.forward*-1;

        return Vector3.Angle(agentToForwardVec, agentToBallVec) * AngleDir(agentToForwardVec, agentToBallVec);
    }

    public void oponentStriked(){
        oponentStrike = true;
    }

    public void touchedBall(){
       if(oponentStrike){
           SetReward(5);
           Debug.Log("GOAL AVOIDED REWARD 5");
           Done();
       }
    }

    public bool agentOutOfPlay(){
        if(site == -1)
            return false;

        if(site > 0){
            if(agentCore.transform.localPosition.x < 14f || agentCore.transform.localPosition.x > 16f){
                SetReward(-1f);
                return true;
            }
            else if(agentCore.transform.localPosition.z < -4f || agentCore.transform.localPosition.z > 4f){
                SetReward(-1f);
                return true;
            }
        }
        else{
            if(agentCore.transform.localPosition.x > -14f || agentCore.transform.localPosition.x < -16f){
                SetReward(-1f);
                return true;
            }
            else if(agentCore.transform.localPosition.z < -4f || agentCore.transform.localPosition.z > 4f){
                SetReward(-1f);
                return true;
            }
        }

        return false;
    }

    public void setSite(int i){
        site = i;
    }

}