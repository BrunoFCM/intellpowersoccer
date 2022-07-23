using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;

public class StrikeTheBallTrainer : Agent
{
    public Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    GoalKeepTrainer goalKeepTrainer;
    float timeLeft;
    float timeOfFullPass;
    public Ball Ball;
    bool ballShooted;
    float angularVelocity;
    Vector3 ballPos;
    int numberOfTouches;
    float timeForTouches;
    bool unlockTouches;
    AgentCore goalKeeper;
    int site;
     

    void Start()
    {
        /*  -- ONLY FOR TRAINING --
        
        agentRBody = GetComponent<Rigidbody>();
        ballShooted = false;
        timeOfFullPass = 1f;
        timeLeft = 60f;
        angularVelocity = 0;
        ballPos = Vector3.zero;
        numberOfTouches = 0;
        timeForTouches = 0;
        goalKeeper = gameEnvironment.blueTeamAgents[0];
        site = Random.Range(0, 2);
        goalKeepTrainer.setSite(site);

        */

        ballShooted = false;
        numberOfTouches = 0;
    }

    void Update()
    {        
        timeLeft -= Time.deltaTime;

        if(ballShooted){
            timeOfFullPass += Time.deltaTime;
        }

        timeForTouches += Time.deltaTime;
 
        if(timeForTouches >= 0.5f){
            unlockTouches = true;
        } 
        
    }

    private void FixedUpdate() {
        /*  -- ONLY FOR TRAINING --
        
        if(agentOutOfPlay()){
            goalKeepTrainer.EndEpisode();
        }
        
        if(ballOutOfPlay()){
            goalKeepTrainer.EndEpisode();
        }

        checkAgentPos();
        checkBallPos(); */
    }

    public override void Initialize() 
    {
        /*  -- ONLY FOR TRAINING --
        
        agentRBody = GetComponent<Rigidbody>();
        ballShooted = false;
        timeOfFullPass = 1f;
        timeLeft = 60f;
        angularVelocity = 0;
        ballPos = Vector3.zero;
        site = Random.Range(0, 2);
        goalKeepTrainer.setSite(site);
        
        positionBall();
        positionPlayers();
        
        */

        ballShooted = false;
        numberOfTouches = 0;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent velocity
        sensor.AddObservation(agentRBody.angularVelocity.magnitude);

        sensor.AddObservation(agentCore.distanceToBall());
        sensor.AddObservation(angleBetweenAgentAndBall());

        sensor.AddObservation(numberOfTouches);
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        controller.Controller(vectorAction);
    }

    public override void OnEpisodeBegin()
    {        
        /*  -- ONLY FOR TRAINING --
        
        site = Random.Range(0, 2);
        goalKeepTrainer.setSite(site);
        stopAgents();
        positionBall();
        positionPlayers();
        
        ballShooted = false;
        timeOfFullPass = 1f;
        timeLeft = 60f;
        angularVelocity = 0;
        numberOfTouches = 0;
        
        */
    }

    public void scoredRedGoal(){
        //Debug.Log("ENTRA RED");
        if(ballShooted)
            if(site < 1){
                SetReward((10.0f/timeOfFullPass) - 0.2f*numberOfTouches);
                //Debug.Log("GOAL SCORED. REWARD: " + ((10.0f/timeOfFullPass) - 0.2f*numberOfTouches));
                goalKeepTrainer.EndEpisode();
            }
    }

    public void scoredBlueGoal(){
        //Debug.Log("ENTRA BLUE");
        if(ballShooted)
            if(site > 0){
                SetReward((10.0f/timeOfFullPass) - 0.2f*numberOfTouches);
                //Debug.Log("GOAL SCORED. REWARD: " + ((10.0f/timeOfFullPass) - 0.2f*numberOfTouches));
                goalKeepTrainer.EndEpisode();
            }
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

    public void positionPlayers(){
        float z = Random.Range(-3.0f, 3.0f);

        if(site > 0){
            agentCore.transform.localPosition = new Vector3(10f, 0.25f, 0f);
            goalKeeper.transform.localPosition = new Vector3(15f, 0.25f, z);
        }else{
            agentCore.transform.localPosition = new Vector3(-10f, 0.25f, 0f);
            goalKeeper.transform.localPosition = new Vector3(-15f, 0.25f, z);
        }

        agentCore.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agentCore.transform.localPosition));
        goalKeeper.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - goalKeeper.transform.localPosition));
    }

    public void positionBall(){

        if(site > 0){
            Ball.transform.localPosition = new Vector3(11.5f, 0.44f, 0);
        }else{
            Ball.transform.localPosition = new Vector3(-11.5f, 0.44f, 0);
        }

        ballPos = Ball.transform.localPosition;
        Ball.stopIt();
        
    }

    public void touchedBall(){
        if(unlockTouches){
            numberOfTouches += 1;
            unlockTouches = false;
            timeForTouches = 0;
        }

        //Debug.Log("NUMBER OF TOUCHES: " + numberOfTouches);
        if(agentCore.GetComponent<Rigidbody>().angularVelocity.magnitude > 1.7f){
            angularVelocity = agentRBody.angularVelocity.magnitude;
            //Debug.Log("Ball was kicked, REWARD: " + 30f / (61 - timeLeft));
            //SetReward(30f / (61 - timeLeft));
            SetReward(0.01f);
            ballShooted = true;
            goalKeepTrainer.oponentStriked();
        }
    }

    public void stopAgents(){
        foreach(AgentCore agent in gameEnvironment.redTeamAgents){
            agent.stopChair();
        }

        foreach(AgentCore agent in gameEnvironment.blueTeamAgents){
            agent.stopChair();
        }
    }

    public bool agentOutOfPlay(){
        if(agentCore.transform.localPosition.x > 16.5 || agentCore.transform.localPosition.x < -16.5){
            SetReward(-0.01f);
            return true;
        }
        else if(agentCore.transform.localPosition.z > 9.5 || agentCore.transform.localPosition.z < -9.5){
            SetReward(-0.01f);
            return true;
        }

        return false;
    }

    public bool ballOutOfPlay(){
        if(Ball.transform.localPosition.y < 0.2){
            SetReward(-0.01f);
            return true;
        }
        return false;
    }

    public void checkBallPos(){
        if(Vector3.Distance(Ball.transform.localPosition, ballPos) > 6){
            //AddReward(0.01f);
            goalKeepTrainer.SetReward(4);
            goalKeepTrainer.EndEpisode();
        }
    }

    public void checkAgentPos(){
        if(Vector3.Distance(agentCore.transform.localPosition, ballPos) > 3){
            SetReward(-0.5f);
            goalKeepTrainer.SetReward(4);
            goalKeepTrainer.EndEpisode();
        }
    }

    public void setGoalKeeper(AgentCore agent){
        goalKeeper = agent;     
    }
}