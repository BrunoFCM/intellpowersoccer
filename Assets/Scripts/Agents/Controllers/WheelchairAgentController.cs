using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WheelchairAgentController : Agent
{

    Rigidbody agentRBody;
    public FeetCollisions feetCollisions;
    public GoalAreaBlue goalAreaBlue;
    public GoalAreaRed goalAreaRed;
    public HalfSideAreaBlue halfSideAreaBlue;
    public HalfSideAreaRed halfSideAreaRed;
    public SmallAreaBlue smallAreaBlue;
    public SmallAreaRed smallAreaRed;
    public OutsideArea outsideArea;

	public Rigidbody largeWheelL, largeWheelR;
	public CapsuleCollider largeWheelLCol, largeWheelRCol;
	public Transform largeWheelLT, largeWheelRT;
	public float motorForce;
	public float maxAngularVelocity;
    public Transform Target;
     float timeLeft = 2000.0f;
     
     void Update()
     {
         timeLeft -= Time.deltaTime;
     }

    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
        largeWheelL.maxAngularVelocity = maxAngularVelocity;
		largeWheelR.maxAngularVelocity = maxAngularVelocity;
    }

    public override void InitializeAgent() {
        // Move the target to a new spot
        updateTargetPos();
    }

    public override void CollectObservations()
    {
        // Agent velocity
        AddVectorObs(agentRBody.velocity.x);
        AddVectorObs(agentRBody.velocity.z);
    }

    private float CalculateConstantC(float controllerAngle, bool right){

		//1º Quadrante
		if(0 <= controllerAngle && controllerAngle < 90){
			if(right)
				return controllerAngle/45 - 1;
			else
				return 1;
		}
		//2º Quadrante
		else if(90 <= controllerAngle && controllerAngle < 180){
			if(right)
				return 1;
			else
				return (180-controllerAngle)/45 - 1;
		}


		//3º Quadrante
		//Exceção de sensibilidade
		else if(180 <= controllerAngle && controllerAngle < 202.5){
			if(right)
				return 1;
			else
				return -1;
		}
		else if(202.5 <= controllerAngle && controllerAngle < 270){
			if(right)
				return -1;
			else
				return (270-controllerAngle)/45 - 1;
		}
		

		//4º Quadrante
		else if(270 <= controllerAngle && controllerAngle < 337.5){
			if(right)
				return (controllerAngle-270)/45 - 1;
			else
				return -1;
		}
		//Exceção de sensibilidade
		else if(337.5 <= controllerAngle && controllerAngle < 360){
			if(right)
				return -1;
			else
				return 1;
		}
		
		else return 0;
	}

    public void DetectAreaCollisions(){
        if(goalAreaBlue.getDetected()){
            Debug.Log("Collision with goalAreaBlue");
            goalAreaBlue.SetDetectedFalse();
        }

        if(goalAreaRed.getDetected()){
            Debug.Log("Collision with goalAreaRed");
            goalAreaRed.SetDetectedFalse();
        }

        if(halfSideAreaBlue.getDetected()){
            Debug.Log("Collision with halfSideAreaBlue");
            halfSideAreaBlue.SetDetectedFalse();
        }

        if(halfSideAreaRed.getDetected()){
            Debug.Log("Collision with halfSideAreaRed");
            halfSideAreaRed.SetDetectedFalse();
        }

        if(smallAreaBlue.getDetected()){
            Debug.Log("Collision with smallAreaBlue");
            smallAreaBlue.SetDetectedFalse();
        }

        if(smallAreaRed.getDetected()){
            Debug.Log("Collision with smallAreaRed");
            smallAreaRed.SetDetectedFalse();
        }

        if(outsideArea.getDetected()){
            Debug.Log("Collision with outsideArea");
            outsideArea.SetDetectedFalse();
        }
    }
    public override void AgentAction(float[] vectorAction)
    {

        //vectorAction = Heuristic();
        
        var m_horizontalInput = vectorAction[0];
        var m_verticalInput = vectorAction[1];

        float controllerAngle = Mathf.Atan2(m_verticalInput, m_horizontalInput) * Mathf.Rad2Deg;

		if(controllerAngle < 0){
			controllerAngle += 360;
		}

		float vectorMagnitude = new Vector2(m_horizontalInput, m_verticalInput).magnitude;

		largeWheelR.AddTorque(transform.right * (- vectorMagnitude * CalculateConstantC(controllerAngle, true) * motorForce));
		largeWheelL.AddTorque(transform.right * (- vectorMagnitude * CalculateConstantC(controllerAngle, false) * motorForce));

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.position, Target.position);

        // Detect area collisions
         if(goalAreaBlue.getDetected()){
            Debug.Log("Collision with goalAreaBlue");
            goalAreaBlue.SetDetectedFalse();
        }

        if(goalAreaRed.getDetected()){
            Debug.Log("Collision with goalAreaRed");
            goalAreaRed.SetDetectedFalse();
        }

        if(halfSideAreaBlue.getDetected()){
            Debug.Log("Collision with halfSideAreaBlue");
            halfSideAreaBlue.SetDetectedFalse();
        }

        if(halfSideAreaRed.getDetected()){
            Debug.Log("Collision with halfSideAreaRed");
            halfSideAreaRed.SetDetectedFalse();
        }

        if(smallAreaBlue.getDetected()){
            Debug.Log("Collision with smallAreaBlue");
            smallAreaBlue.SetDetectedFalse();
        }

        if(smallAreaRed.getDetected()){
            Debug.Log("Collision with smallAreaRed");
            smallAreaRed.SetDetectedFalse();
        }

        if(outsideArea.getDetected()){
            Debug.Log("Collision with outsideArea");
            outsideArea.SetDetectedFalse();
        }

        // Reached target
        if (feetCollisions.getDetected())
        {
            Debug.Log("Ball Found, REWARD TAKEN");
            feetCollisions.SetDetectedFalse();

            //Reward increases by being faster to reach the ball
            SetReward(4000.0f / timeLeft);

            timeLeft = 2000.0f;
            Done();
        }

        // Target or WheelChair Fell off platform or Bugged
        if (Target.position.y < 0 || transform.position.y > 0.5f || transform.position.y < 0.1f){
            Debug.Log("Target or WheelChair Fell off platform or Bugged");
            SetReward(-0.01f);
            Done();
        }


        // Wheelchair took to mutch time to find it, probably a Bug
        if(timeLeft < 0)
        {
            Debug.Log("Took too long to find or Bug occurred");
            Done();
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[2];
        action[0] = Input.GetAxis("Horizontal");
        action[1] = Input.GetAxis("Vertical");
        return action;
    }

    public override void AgentReset()
    {
        //Distance from whellchair to target
        //float distanceToBall = Vector3.Distance(transform.position, Target.position);
        

        //Move Wheelchair to origin if Bug or Fell from Patform
        if(transform.position.y > 0.5f || transform.position.y < 0.1f || timeLeft <= 0){
            agentRBody.angularVelocity = Vector3.zero;
            agentRBody.velocity = Vector3.zero;
            transform.position = new Vector3(0, 0.25f, 0);
            transform.rotation = Quaternion.identity;
        }

        updateTargetPos();

        timeLeft = 2000.0f;
    }

    void updateTargetPos(){
        //Don't allow target spawn already touching the wheelchair
        float x = Random.Range(-1.0f, 1.0f) * 13.0f;
        float z = Random.Range(-1.0f, 1.0f) * 7.0f;

        // Move the target to a new spot
        Target.position = new Vector3(x, 0.5f, z);
    }
}