using UnityEngine;
using MLAgents;


public class MoveToPointTrainer : Agent
{
    public Rigidbody agentRBody;
    public AgentCore agentCore;
    public WheelchairAgentController controller;
    public GameEnvironmentInfo gameEnvironment;
    public GameObject pointFigure;
    private Vector2 point;
    private float timeLeft;
    private float timeSec;

    void Start()
    {
              
    }

    void Update()
    {
        /*timeLeft -= Time.deltaTime;

        timeSec -= Time.deltaTime;

        if(timeSec < 1){
            generatePoint();
            timeSec = 10f;
        }

        if(timeLeft <= 0){
            AddReward(-0.1f);
            //Done();
        }

        if(checkAgentIsInPoint()){
            SetReward(10);
            Debug.Log("SET 10 REWARD! CONGRATS");
            //Done();
        }*/
    }

    private void FixedUpdate() {    
        /*if(agentOutOfPlay()){
            //Done();
        }*/
    }

    public override void InitializeAgent() 
    {        
        /*timeLeft = 60f;
        timeSec = 10f;
        agentRBody = GetComponent<Rigidbody>();
        
        stopAgents();
        positionPlayers();
        generatePoint();*/

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
        AddVectorObs(Vector3.Distance(agentCore.transform.localPosition, new Vector3(point.x, 0, point.y)));
    }

    public override void AgentAction(float[] vectorAction)
    {
        controller.Controller(vectorAction);
    }

    public override void AgentReset()
    {
        timeLeft = 60f;
        timeSec = 10f;
        agentRBody = GetComponent<Rigidbody>();
        
        stopAgents();
        positionPlayers();
        generatePoint();
    }


    //------------------------------------------------------------- PASSING BALL MECHANISM -------------------------------------------------------------

    public bool checkAgentIsInPoint(){
        if(Vector3.Distance(agentCore.transform.localPosition, new Vector3(point.x, 0, point.y)) < 0.5f){
            return true;
        }
        return false;
    }
    public void generatePoint(){    
        Vector2 annullusCoords = generatePointInsideAnnullus(3.5f, 8f);

        point = new Vector2(agentCore.transform.localPosition.x + annullusCoords.x, agentCore.transform.localPosition.z + annullusCoords.y);
        pointFigure.transform.localPosition = new Vector3(agentCore.transform.localPosition.x + annullusCoords.x, 0.5f, agentCore.transform.localPosition.z + annullusCoords.y);

        while(pointOutOfPlay(point)){
            annullusCoords = generatePointInsideAnnullus(3.5f, 8f);

            point = new Vector2(agentCore.transform.localPosition.x + annullusCoords.x, agentCore.transform.localPosition.z + annullusCoords.y);
            pointFigure.transform.localPosition = new Vector3(agentCore.transform.localPosition.x + annullusCoords.x, 0.5f, agentCore.transform.localPosition.z + annullusCoords.y);
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

    public Vector2 generatePointInsideAnnullus(float R1, float R2){
        float rnd = Random.Range(0.0f, 1.0f);
        float theta = 360 * rnd;
        float dist = Mathf.Sqrt(rnd*((R1*R1)-(R2*R2))+(R2*R2));

        float x =  dist * Mathf.Cos(theta);
        float y =  dist * Mathf.Sin(theta);

        return new Vector2(x,y);
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

}
