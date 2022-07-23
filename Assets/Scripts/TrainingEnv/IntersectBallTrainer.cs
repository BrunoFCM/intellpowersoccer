using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class IntersectBallTrainer : Agent
{
    public Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    public Ball Ball;
    private AgentCore agent1;
    private GameObject passTheBallTrainerA1;
    private AgentCore agent2;
    private GameObject passTheBallTrainerA2;
    Vector3 ballPos;
    AgentCore opponentWithBall;    
    AgentCore opponentRecieveingBall; 

    float timeLeft;
    int rndAgent;

    void Start()
    {
              
    }

    void Update()
    {
        /*timeLeft -= Time.deltaTime;

        if(timeLeft <= 0){
            AddReward(0.1f);
            // EndEpisode();
        }

        if(checkBallWassPassed()){
            Debug.Log("BALL WAS PASSED, REWARD -1");
            SetReward(-1);
            // EndEpisode();
        }

        checkOpponentProximmity();*/
    }

    private void FixedUpdate() {
        
        /*if(agentOutOfPlay()){
            // EndEpisode();
        }
        
        if(ballOutOfPlay()){
            // EndEpisode();
        }

        checkAgentPos();*/
    }

    public override void Initialize() 
    {        
        timeLeft = 15f;
        agentRBody = GetComponent<Rigidbody>();
        rndAgent = 0;
        agent1 = gameEnvironment.getNearestPlayerToBall();
        agent2 = gameEnvironment.getNearestTeamMate(agent1);
        
        /*stopAgents();
        positionPlayers();
        checkPositions();
        positionBall(); 
        disablePassingBehaviours();
        activatePassingBehaviour();*/

        opponentWithBall = gameEnvironment.getNearestOpponentWithBall(agentCore);
        opponentRecieveingBall = gameEnvironment.getNearestTeamMate(opponentWithBall);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(opponentWithBall.transform.localPosition.x);
        sensor.AddObservation(opponentWithBall.transform.localPosition.z);

        sensor.AddObservation(opponentRecieveingBall.transform.localPosition.x);
        sensor.AddObservation(opponentRecieveingBall.transform.localPosition.z);
        
        sensor.AddObservation(agentCore.distanceToBall());
        
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        //vectorAction = Heuristic();
        controller.Controller(vectorAction);
    }

    public override void OnEpisodeBegin()
    {
        /*timeLeft = 15f;
        agentRBody = GetComponent<Rigidbody>();
        rndAgent = 0;
        
        stopAgents();
        positionPlayers();
        checkPositions();
        positionBall(); 
        disablePassingBehaviours();
        activatePassingBehaviour();

        opponentWithBall = gameEnvironment.getNearestOpponentWithBall(agentCore);
        opponentRecieveingBall = gameEnvironment.getNearestTeamMate(opponentWithBall);*/
    }


    //------------------------------------------------------------- PASSING BALL MECHANISM -------------------------------------------------------------

    public void positionPlayers(){
        rndAgent = Random.Range(0,2);
        AgentCore agentPassing;
        AgentCore agentRecieveing;

        if(rndAgent > 0){
            agentPassing = agent1;
            agentRecieveing = agent2;
        }
        else{
            agentPassing = agent2;
            agentRecieveing = agent1;
        }

        float x = Random.Range(-12.5f, 12.5f);
        float z = Random.Range(-5.5f, 5.5f);

        // POSITION AGENT PASSING THE BALL
        agentPassing.transform.localPosition = new Vector3(x, 0.25f, z);


        // POSITION AGENT RECIEVING THE BALL
        Vector2 annullusCoords = generatePointInsideAnnullus(4, 8);

        if(x + annullusCoords.x > 13f || x + annullusCoords.x < -13f)
            annullusCoords.x = - annullusCoords.x;

        if(z + annullusCoords.y > 6.5f || z + annullusCoords.y < -6.5f)
            annullusCoords.y = - annullusCoords.y;

        agentRecieveing.transform.localPosition = new Vector3(x + annullusCoords.x, 0.25f, z + annullusCoords.y);


        // POSITION LEARNING AGENT
        annullusCoords = generatePointInsideAnnullus(3.5f, 6);

        if(x + annullusCoords.x > 13f || x + annullusCoords.x < -13f)
            annullusCoords.x = - annullusCoords.x;

        if(z + annullusCoords.y > 6.5f || z + annullusCoords.y < -6.5f)
            annullusCoords.y = - annullusCoords.y;

        agentCore.transform.localPosition = new Vector3(x + annullusCoords.x, 0.25f, z + annullusCoords.y);
        
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
        AgentCore agentPassing;

        if(rndAgent > 0){
            agentPassing = agent1;
        }
        else{
            agentPassing = agent2;
        }

        float x = Random.Range(-0.75f, 0.75f);
        float z = Random.Range(-0.75f, 0.75f);

        if(x > 0){
            if(z > 0){
                Ball.transform.localPosition = new Vector3(agentPassing.transform.localPosition.x + 0.5f + x,
                                                     agentPassing.transform.localPosition.y, 
                                                     agentPassing.transform.localPosition.z + 0.5f + z);
            }else{
                Ball.transform.localPosition = new Vector3(agentPassing.transform.localPosition.x + 0.5f + x,
                                                     agentPassing.transform.localPosition.y, 
                                                     agentPassing.transform.localPosition.z - 0.5f + z);
            }
        }else{
            if(z > 0){
                Ball.transform.localPosition = new Vector3(agentPassing.transform.localPosition.x - 0.5f + x,
                                                     agentPassing.transform.localPosition.y, 
                                                     agentPassing.transform.localPosition.z + 0.5f + z);
            }else{
                Ball.transform.localPosition = new Vector3(agentPassing.transform.localPosition.x - 0.5f + x,
                                                     agentPassing.transform.localPosition.y, 
                                                     agentPassing.transform.localPosition.z - 0.5f + z);
            }
        }

        ballPos = Ball.transform.localPosition;

        Ball.stopIt();
        
    }

    public void disablePassingBehaviours(){
        passTheBallTrainerA1.SetActive(false);
        passTheBallTrainerA2.SetActive(false);
    }

    public void activatePassingBehaviour(){
        if(rndAgent > 0){
            passTheBallTrainerA1.SetActive(true);
        }
        else{
            passTheBallTrainerA2.SetActive(true);
        }
    }


    public void touchedBall(){
        Debug.Log("OBJECTIVE COMPLETE, REWARD 5");
        AddReward(5);
        // EndEpisode();
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

    public void checkAgentPos(){
        if(Vector3.Distance(agentCore.transform.localPosition, ballPos) > 6){
            SetReward(-0.5f);
            // EndEpisode();
        }
    }
    public bool checkBallWassPassed(){
        if(rndAgent > 0){
            if(agent2.distanceToBall() < 2){
                return true;
            }
        }
        else{
            if(agent1.distanceToBall() < 2){
                return true;
            }
        }

        return false;
    }

    public void checkOpponentProximmity(){
        if(rndAgent > 0){
            if(agentCore.distanceToPlayer(agent1) < 1.8f){
                AddReward(-0.01f);
                Debug.Log("DISTANCE TO OPPONENT IS BAD, REWARD -0.01");
            }
        }
        else{
            if(agentCore.distanceToPlayer(agent2) < 1.8f){
                AddReward(-0.01f);
                Debug.Log("DISTANCE TO OPPONENT IS BAD, REWARD -0.01");
            }
        }
        
    }

}
