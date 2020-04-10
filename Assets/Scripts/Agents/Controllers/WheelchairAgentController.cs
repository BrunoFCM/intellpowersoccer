using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class WheelchairAgentController : Agent
{
    Rigidbody agentRBody;
    public AgentCore agentCore;


    //WHEELCHAIR CONTROLLER VARS
	public Rigidbody largeWheelL, largeWheelR;
	public CapsuleCollider largeWheelLCol, largeWheelRCol;
	public Transform largeWheelLT, largeWheelRT;
	public float motorForce;
	public float maxAngularVelocity;


    //AGENT VARS
    float timeLeft = 2000.0f;
    public Transform Target;
     
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
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);

        // Target or WheelChair Fell off platform or Bugged
    /*
        if (Target.localPosition.y < 0 || transform.localPosition.y > 0.5f || transform.localPosition.y < 0.1f){
            Debug.Log("Target or WheelChair Fell off platform or Bugged");
            SetReward(-0.01f);
            Done();
        }
    */

        // Wheelchair took to mutch time to find it, probably a Bug
        if(timeLeft < 0)
        {
            Debug.Log("Took too long to find or Bug occurred");
            Done();
        }
    }

    public void playerTouchedBall(){
        //Debug.Log("Ball Found, REWARD TAKEN");

        //Reward increases by being faster to reach the ball
        SetReward(4000.0f / timeLeft);

        timeLeft = 2000.0f;
        //Done();
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
        //Move Wheelchair to origin if Bug or Fell from Patform
        if(transform.localPosition.y > 0.5f || transform.localPosition.y < 0.1f || timeLeft <= 0){
            agentRBody.angularVelocity = Vector3.zero;
            agentRBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3(0, 0.25f, 0);
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
        Target.localPosition = new Vector3(0f, 0.44f, 2.0f);
    }
}