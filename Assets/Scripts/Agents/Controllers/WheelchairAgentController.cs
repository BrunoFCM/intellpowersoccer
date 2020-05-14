using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelchairAgentController : MonoBehaviour
{
    Rigidbody agentRBody;

    //WHEELCHAIR CONTROLLER VARS
	public Rigidbody largeWheelL, largeWheelR;
	public CapsuleCollider largeWheelLCol, largeWheelRCol;
	public Transform largeWheelLT, largeWheelRT;
	public float motorForce;
	public float maxAngularVelocity;
     

    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
        largeWheelL.maxAngularVelocity = maxAngularVelocity;
		largeWheelR.maxAngularVelocity = maxAngularVelocity;
    }

    private void FixedUpdate() {
        //Controller(null);
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

    public void Controller(float[] vectorAction){


      
        var m_horizontalInput = Input.GetAxis("Horizontal");
        var m_verticalInput = Input.GetAxis("Vertical");

        if(vectorAction != null){
            m_horizontalInput = vectorAction[0];
            m_verticalInput = vectorAction[1];
        }

        float controllerAngle = Mathf.Atan2(m_verticalInput, m_horizontalInput) * Mathf.Rad2Deg;

		if(controllerAngle < 0){
			controllerAngle += 360;
		}

		float vectorMagnitude = new Vector2(m_horizontalInput, m_verticalInput).magnitude;

		largeWheelR.AddTorque(transform.right * (- vectorMagnitude * CalculateConstantC(controllerAngle, true) * motorForce));
		largeWheelL.AddTorque(transform.right * (- vectorMagnitude * CalculateConstantC(controllerAngle, false) * motorForce));

    }
}