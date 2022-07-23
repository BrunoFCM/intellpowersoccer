using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class DribbleBallTrainer : Agent
{
    public Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    public Ball Ball;
    public GameObject pointFigure;
    private Vector2 point;
    private Vector3 ballPos;
    private float timeLeft;

    void Start()
    {
              
    }

    void Update()
    {
        /*timeLeft -= Time.deltaTime;

        if(timeLeft <= 0){
            AddReward(-0.1f);
            //EndEpisode();
        }

        if(checkBallIsInPoint()){
            SetReward(10);
            Debug.Log("SET 10 REWARD! CONGRATS");
            //EndEpisode();
        }*/
    }

    private void FixedUpdate() {
        
        /*if(agentOutOfPlay()){
            //EndEpisode();
        }
        
        if(ballOutOfPlay()){
            //EndEpisode();
        }

        //checkAgentPos();*/
    }

    public override void Initialize() 
    {        
        //timeLeft = 60f;
        agentRBody = GetComponent<Rigidbody>();
        
        /*stopAgents();
        positionPlayers();
        positionBall();
        generatePoint();*/

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.ContinuousActions;
        actions[0] = Input.GetAxis("Horizontal");
        actions[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        /*sensor.AddObservation(agentCore.transform.localPosition.x);
        sensor.AddObservation(agentCore.transform.localPosition.z);

        sensor.AddObservation(point.x);
        sensor.AddObservation(point.y);
        
        sensor.AddObservation(agentCore.distanceToBall());
        sensor.AddObservation(angleBetweenAgentAndBall());

        sensor.AddObservation(agentRBody.velocity.magnitude);*/
        sensor.AddObservation(Vector3.Distance(Ball.transform.localPosition, new Vector3(point.x, 0, point.y)));
        
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        controller.Controller(vectorAction);
    }

    public override void OnEpisodeBegin()
    {
        /*timeLeft = 60f;
        agentRBody = GetComponent<Rigidbody>();
        
        stopAgents();
        positionPlayers();
        positionBall(); 
        generatePoint();*/
    }


    //------------------------------------------------------------- PASSING BALL MECHANISM -------------------------------------------------------------

    public bool checkBallIsInPoint(){
        if(Vector3.Distance(Ball.transform.localPosition, new Vector3(point.x, 0, point.y)) < 1f){
            return true;
        }
        return false;
    }
    public void generatePoint(){
        /*float x = Random.Range(-12.5f, 12.5f);
        float z = Random.Range(-5.5f, 5.5f);

        Vector2 annullusCoords = generatePointInsideAnnullus(3.5f, 6);

        if(x + annullusCoords.x > 13f || x + annullusCoords.x < -13f)
            annullusCoords.x = - annullusCoords.x;

        if(z + annullusCoords.y > 6.5f || z + annullusCoords.y < -6.5f)
            annullusCoords.y = - annullusCoords.y;

        point = new Vector2(x + annullusCoords.x, z + annullusCoords.y);
        pointFigure.transform.localPosition = new Vector3(x + annullusCoords.x, 0.5f, z + annullusCoords.y);*/

    
        Vector2 annullusCoords = generatePointInsideAnnullus(3.5f, 6f);

        point = new Vector2(Ball.transform.localPosition.x + annullusCoords.x, Ball.transform.localPosition.z + annullusCoords.y);
        pointFigure.transform.localPosition = new Vector3(Ball.transform.localPosition.x + annullusCoords.x, 0.5f, Ball.transform.localPosition.z + annullusCoords.y);

        while(pointOutOfPlay(point)){
            annullusCoords = generatePointInsideAnnullus(3.5f, 6f);

            point = new Vector2(Ball.transform.localPosition.x + annullusCoords.x, Ball.transform.localPosition.z + annullusCoords.y);
            pointFigure.transform.localPosition = new Vector3(Ball.transform.localPosition.x + annullusCoords.x, 0.5f, Ball.transform.localPosition.z + annullusCoords.y);
        }

    }

    public void positionPlayers(){
        float x = Random.Range(-12.5f, 12.5f);
        float z = Random.Range(-5.5f, 5.5f);

        // POSITION LEARNING AGENT
        Vector2 annullusCoords = generatePointInsideAnnullus(3.5f, 6);

        if(x + annullusCoords.x > 13f || x + annullusCoords.x < -13f)
            annullusCoords.x = - annullusCoords.x;

        if(z + annullusCoords.y > 6.5f || z + annullusCoords.y < -6.5f)
            annullusCoords.y = - annullusCoords.y;

        agentCore.transform.localPosition = new Vector3(x + annullusCoords.x, 0.25f, z + annullusCoords.y);
        
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

    public Vector2 generatePointInsideAnnullus(float R1, float R2){
        float rnd = Random.Range(0.0f, 1.0f);
        float theta = 360 * rnd;
        float dist = Mathf.Sqrt(rnd*((R1*R1)-(R2*R2))+(R2*R2));

        float x =  dist * Mathf.Cos(theta);
        float y =  dist * Mathf.Sin(theta);

        return new Vector2(x,y);
    }

    public void positionBall(){
        float x = Random.Range(-3f, 3f);
        float z = Random.Range(-3f, 3f);

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


    public void touchedBall(){
        AddReward(0.01f);
        Debug.Log("ADDED 0.01 REWARD!");
    }

    public void stopAgents(){
        agentCore.stopChair();
    }

    public bool agentOutOfPlay(){
        if(agentCore.transform.localPosition.x > 16.5f || agentCore.transform.localPosition.x < -16.5f){
            SetReward(-0.01f);
            return true;
        }
        else if(agentCore.transform.localPosition.z > 9.5f || agentCore.transform.localPosition.z < -9.5f){
            SetReward(-0.01f);
            return true;
        }

        return false;
    }

    public bool pointOutOfPlay(Vector2 Point){
        if(Point.x > 16.5f || Point.x < -16.5f){
            return true;
        }
        else if(Point.y > 9.5f || Point.y < -9.5f){
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
            ////EndEpisode();
        }
    }

}
