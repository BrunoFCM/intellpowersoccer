using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;

public class PassTheBallTrainer : Agent
{
    Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    float timeLeft;
    float timeOfFullPass;
    public Ball Ball;
    bool ballShooted;
    float angularVelocity;
    Vector3 ballPos;
    AgentCore nearestTeamMate;
    int numberOfTouches;
    float timeForTouches;
    bool unlockTouches;
     

    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
        ballShooted = false;
        timeOfFullPass = 1f;
        timeLeft = 60f;
        angularVelocity = 0;
        ballPos = Vector3.zero;
        nearestTeamMate = gameEnvironment.getNearestTeamMate(agentCore);
        numberOfTouches = 0;
        timeForTouches = 0;
    }

    void Update()
    {

        if(ballShooted){
            if(checkBallWasPassedToTeamMate())
                Done();
            //if(checkBallWasPassedToOponent())
                //Done();
        }

        timeLeft -= Time.deltaTime;

        if(ballShooted){
            timeOfFullPass += Time.deltaTime;
        }

        if(timeLeft < 0){
            Done();
        }

        timeForTouches += Time.deltaTime;
 
        if(timeForTouches >= 0.5f){
            unlockTouches = true;
        }
    }

    private void FixedUpdate() {
        if(agentOutOfPlay()){
            Done();
        }
        
        if(ballOutOfPlay()){
            Done();
        }

        checkAgentPos();
        checkBallPos();
    }

    public override void InitializeAgent() 
    {
        agentRBody = GetComponent<Rigidbody>();
        ballShooted = false;
        timeOfFullPass = 1f;
        timeLeft = 60f;
        angularVelocity = 0;
        ballPos = Vector3.zero;
        nearestTeamMate = gameEnvironment.getNearestTeamMate(agentCore);
        
        positionLearningAgent();
        positionPlayers();
        checkPositions();
        positionBall();
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
        // Agent velocity
        AddVectorObs(agentRBody.angularVelocity.magnitude);

        AddVectorObs(agentCore.distanceToBall());
        AddVectorObs(angleBetweenAgentAndBall());

        AddVectorObs(Vector3.Distance(agentCore.transform.localPosition, nearestTeamMate.transform.localPosition));
        AddVectorObs(angleBetweenAgentAndTeamMate(nearestTeamMate));

        AddVectorObs(numberOfTouches);
    }

    public override void AgentAction(float[] vectorAction)
    {
        //vectorAction = Heuristic();
        controller.Controller(vectorAction);
    }

    public override void AgentReset()
    {
        nearestTeamMate = gameEnvironment.getNearestTeamMate(agentCore);
        Debug.Log("Nearest TeamMate: "+nearestTeamMate.name);

        stopAgents();
        positionPlayers();
        checkPositions();
        positionBall();
        ballShooted = false;
        timeOfFullPass = 1f;
        timeLeft = 60f;
        angularVelocity = 0;
        numberOfTouches = 0;
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

    public float angleBetweenAgentAndTeamMate(AgentCore Agent){
        Vector3 agentToBallVec = Agent.transform.localPosition - agentCore.transform.localPosition;
        Vector3 agentToForwardVec = agentCore.transform.forward*-1;

        return Vector3.Angle(agentToForwardVec, agentToBallVec) * AngleDir(agentToForwardVec, agentToBallVec);
    }

    public void positionLearningAgent(){
        float x = Random.Range(-12.5f, 12.5f);
        float z = Random.Range(-5.5f, 5.5f);

        agentCore.transform.localPosition = new Vector3(x, 0.25f, z);
        agentCore.stopChair();
    }


    //------------------------------------------------------------- PASSING BALL MECHANISM -------------------------------------------------------------

    public void positionPlayers(){
        float x = Random.Range(-12.5f, 12.5f);
        float z = Random.Range(-5.5f, 5.5f);

        agentCore.transform.localPosition = new Vector3(x, 0.25f, z);

        if(agentCore.team == AgentCore.Team.RED){
            foreach(AgentCore agentRed in gameEnvironment.redTeamAgents){
                if(agentCore != agentRed){
                    Vector2 annullusCoords = generatePointInsideAnnullus(3, 6);

                    if(x + annullusCoords.x > 13f || x + annullusCoords.x < -13f)
                        annullusCoords.x = - annullusCoords.x;

                    if(z + annullusCoords.y > 6.5f || z + annullusCoords.y < -6.5f)
                        annullusCoords.y = - annullusCoords.y;

                    agentRed.transform.localPosition = new Vector3(x + annullusCoords.x, 0.25f, z + annullusCoords.y);
                }
            }

            foreach(AgentCore agentBlue in gameEnvironment.blueTeamAgents){
                if(agentCore != agentBlue){
                    x = Random.Range(-11.5f, 11.5f);
                    z = Random.Range(-4.5f, 4.5f);
                    agentBlue.transform.localPosition = new Vector3(x, 0.25f, z);
                }
        }
        }else{
            foreach(AgentCore agentBlue in gameEnvironment.blueTeamAgents){
                if(agentCore != agentBlue){
                    Vector2 annullusCoords = generatePointInsideAnnullus(3, 6);

                    if(x + annullusCoords.x > 13f || x + annullusCoords.x < -13f)
                        annullusCoords.x = - annullusCoords.x;

                    if(z + annullusCoords.y > 6.5f || z + annullusCoords.y < -6.5f)
                        annullusCoords.y = - annullusCoords.y;

                    agentBlue.transform.localPosition = new Vector3(x + annullusCoords.x, 0.25f, z + annullusCoords.y);
                }
            }

            foreach(AgentCore agentRed in gameEnvironment.redTeamAgents){
                if(agentCore != agentRed){
                    x = Random.Range(-11.5f, 11.5f);
                    z = Random.Range(-4.5f, 4.5f);
                    agentRed.transform.localPosition = new Vector3(x, 0.25f, z);
                }
            }
        }
    }

    public Vector2 generatePointInsideAnnullus(float R1, float R2){
        float rnd = Random.Range(0.0f, 1.0f);
        float theta = 360 * rnd;
        float dist = Mathf.Sqrt(rnd*((R1*R1)-(R2*R2))+(R2*R2));

        float x =  dist * Mathf.Cos(theta);
        float y =  dist * Mathf.Sin(theta);

        return new Vector2(x,y);
    }

    public void positionBall(){

        float x = Random.Range(-0.75f, 0.75f);
        float z = Random.Range(-0.75f, 0.75f);

        if(x > 0){
            if(z > 0){
                Ball.transform.localPosition = new Vector3(agentCore.transform.localPosition.x + 0.5f + x,
                                                     agentCore.transform.localPosition.y, 
                                                     agentCore.transform.localPosition.z + 0.5f + z);
            }else{
                Ball.transform.localPosition = new Vector3(agentCore.transform.localPosition.x + 0.5f + x,
                                                     agentCore.transform.localPosition.y, 
                                                     agentCore.transform.localPosition.z - 0.5f + z);
            }
        }else{
            if(z > 0){
                Ball.transform.localPosition = new Vector3(agentCore.transform.localPosition.x - 0.5f + x,
                                                     agentCore.transform.localPosition.y, 
                                                     agentCore.transform.localPosition.z + 0.5f + z);
            }else{
                Ball.transform.localPosition = new Vector3(agentCore.transform.localPosition.x - 0.5f + x,
                                                     agentCore.transform.localPosition.y, 
                                                     agentCore.transform.localPosition.z - 0.5f + z);
            }
        }

        ballPos = Ball.transform.localPosition;

        Ball.stopIt();
        
    }

    public bool checkBallWasPassedToTeamMate(){
        if(Vector3.Distance(nearestTeamMate.transform.localPosition, Ball.transform.localPosition) < 2f){
            SetReward((10.0f/timeOfFullPass) - 0.2f*numberOfTouches);
            Debug.Log("BALL PASSED TO TEAM MATE. REWARD: " + ((10.0f/timeOfFullPass) - 0.2f*numberOfTouches));
            return true;
        }
        return false;
    }

    public bool checkBallWasPassedToOponent(){
        if(agentCore.team == AgentCore.Team.RED){
            foreach(AgentCore agent in gameEnvironment.blueTeamAgents){
                if(agentCore != agent){
                    if(agent.distanceToBall() < 1.5f){
                        SetReward(-0.02f);
                        Debug.Log("BALL PASSED TO OPONENT. REWARD: " + -0.02f);
                        return true;
                    }
                }
            }
        }else{
            foreach(AgentCore agent in gameEnvironment.redTeamAgents){
                if(agentCore != agent){
                    if(agent.distanceToBall() < 1.5f){
                        SetReward(-0.02f);
                        Debug.Log("BALL PASSED TO OPONENT. REWARD: " + -0.02f);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void touchedBall(){
        if(unlockTouches){
            numberOfTouches += 1;
            unlockTouches = false;
            timeForTouches = 0;
        }

        //Debug.Log("NUMBER OF TOUCHES: " + numberOfTouches);
        if(agentCore.GetComponent<Rigidbody>().angularVelocity.magnitude > 1.2f){
            angularVelocity = agentRBody.angularVelocity.magnitude;
            //Debug.Log("Ball was kicked, REWARD: " + 30f / (61 - timeLeft));
            //SetReward(30f / (61 - timeLeft));
            SetReward(0.01f);
            ballShooted = true;        }
    }

    public void stopAgents(){
        foreach(AgentCore agent in gameEnvironment.redTeamAgents){
            agent.stopChair();
        }

        foreach(AgentCore agent in gameEnvironment.blueTeamAgents){
            agent.stopChair();
        }
    }

    public void checkPositions(){
        gameEnvironment.detectPlayersAtSamePosBugAfterFoul(agentCore);
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
            Done();
        }
    }

    public void checkAgentPos(){
        if(Vector3.Distance(agentCore.transform.localPosition, ballPos) > 6){
            SetReward(-0.5f);
            Done();
        }
    }
}