using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class GoalKeepTrainer : Agent
{
    public Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    StrikeTheBallTrainer strikeTheBallTrainer;
    public Ball Ball;
    Vector3 ballPos;
    AgentCore shooter;
    int site;
    bool oponentStrike;
     

    void Start()
    {
        /*  -- ONLY FOR TRAINING --
        
        agentRBody = GetComponent<Rigidbody>();
        ballPos = Vector3.zero;
        shooter = gameEnvironment.redTeamAgents[0];
        oponentStrike = false;
        site = -1; 
        
        */

        shooter = gameEnvironment.redTeamAgents[0];
        oponentStrike = false;
    }

    void Update()
    {

    }

    private void FixedUpdate() {
        /*  -- ONLY FOR TRAINING --

        if(agentOutOfPlay()){
            EndEpisode();
        }
        
        */
    }

    public override void Initialize() 
    {
        /*  -- ONLY FOR TRAINING --
        
        agentRBody = GetComponent<Rigidbody>();
        ballPos = Vector3.zero;
        shooter = gameEnvironment.redTeamAgents[0];
        oponentStrike = false;
        site = -1;
        
        */

        //agentRBody = GetComponent<Rigidbody>();
        //shooter = gameEnvironment.getPlayerShooting();
        //oponentStrike = false;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(agentCore.distanceToBall());
        sensor.AddObservation(angleBetweenAgentAndBall());
        sensor.AddObservation(agentCore.distanceToPlayer(shooter));
        sensor.AddObservation(angleBetweenAgentAndShooter());
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        controller.Controller(vectorAction);
    }

    public override void OnEpisodeBegin()
    {   
        /*  -- ONLY FOR TRAINING --
        
        site = -1;
        oponentStrike = false;
        strikeTheBallTrainer.EndEpisode();

        */
    }

    public void scoredRedGoal(){
        SetReward(-2);
        //Debug.Log("GOAL SCORED REWARD -2");
        EndEpisode();
    }

    public void scoredBlueGoal(){
        SetReward(-2);
        //Debug.Log("GOAL SCORED REWARD -2");
        EndEpisode();
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
           //Debug.Log("GOAL AVOIDED REWARD 5");
           EndEpisode();
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