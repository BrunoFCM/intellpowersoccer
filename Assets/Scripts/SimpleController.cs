using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour {

	private float m_horizontalInput;
	private float m_verticalInput;

	public Rigidbody largeWheelL, largeWheelR;
	public CapsuleCollider largeWheelLCol, largeWheelRCol;

	public Transform largeWheelLT, largeWheelRT;
	public float motorForce;

	public float maxAngularVelocity;

	private void Start() {
		largeWheelL.maxAngularVelocity = maxAngularVelocity;
		largeWheelR.maxAngularVelocity = maxAngularVelocity;
	}

	public void GetInput()
	{
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
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

	private void Accelerate()
	{
		float controllerAngle = Mathf.Atan2(m_verticalInput, m_horizontalInput) * Mathf.Rad2Deg;

		if(controllerAngle < 0){
			controllerAngle += 360;
		}

		float vectorMagnitude = new Vector2(m_horizontalInput, m_verticalInput).magnitude;

		largeWheelR.AddTorque(transform.right * (- vectorMagnitude * CalculateConstantC(controllerAngle, true) * motorForce));
		largeWheelL.AddTorque( transform.right * (- vectorMagnitude * CalculateConstantC(controllerAngle, false) * motorForce));

		/*		
			//DEBUG
			Debug.Log("X: " + m_horizontalInput + "\n");
			Debug.Log("Y: " + m_verticalInput + "\n");

			Debug.Log("Motor Torque R: " + largeWheelR.motorTorque + "\n");
			Debug.Log("Motor Torque L: " + largeWheelL.motorTorque + "\n");

			Debug.Log("Controller Angle: " + controllerAngle + "\n");
			Debug.Log("Vector Magnitude: " + vectorMagnitude + "\n");
			Debug.Log("Constant C Right: " + CalculateConstantC(controllerAngle, true) + "\n");
			Debug.Log("Constant C Left: " + CalculateConstantC(controllerAngle, false) + "\n");
		*/

	}

	/*private void UpdateWheelPoses()
	{
		UpdateWheelPose(largeWheelLCol, largeWheelLT);
		UpdateWheelPose(largeWheelRCol, largeWheelRT);
	}

	private void UpdateWheelPose(CapsuleCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void StopJitter(){
		largeWheelR.ConfigureVehicleSubsteps(5,12,15);
		largeWheelL.ConfigureVehicleSubsteps(5,12,15);
	}*/

	private void FixedUpdate()
	{
		GetInput();
		Accelerate();

	}

}