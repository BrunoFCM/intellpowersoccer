using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour {

	private float m_horizontalInput;
	private float m_verticalInput;

	public WheelCollider largeWheelL, largeWheelR, smallWheelRB, smallWheelLB, smallWheelRF, smallWheelLF;
	public Transform largeWheelLT, largeWheelRT, smallWheelRBT, smallWheelLBT, smallWheelRFT, smallWheelLFT;
	public float motorForce = 10;

	public void GetInput()
	{
		m_horizontalInput = Input.GetAxis("Horizontal");
		m_verticalInput = Input.GetAxis("Vertical");
	}

	private float CalculateConstantC(float controllerAngle, bool right){

		//2 motors positive (go forward)
		if((135 <= controllerAngle && controllerAngle <= 180) || (controllerAngle <= -135 && controllerAngle > -180)){ 
			if(right)
				return 1;
			else
				return 1;
		}
		//left - negative; right - positive (go left)
		if(controllerAngle <= -45 && controllerAngle > -135){
			if(right)
				return 1;
			else
				return -1;
		}
		//left - negative; right - negative (go backwards)
		if((0 <= controllerAngle && controllerAngle < 45) || (controllerAngle <= 0 && controllerAngle > -45)){
			if(right)
				return -1;
			else
				return -1;
		}
		//left - positive; right - negative (go right)
		if(45 <= controllerAngle && controllerAngle < 135){
			if(right)
				return -1;
			else
				return 1;
		}

		return 0;
	}

	private void Accelerate()
	{
		float controllerAngle = Mathf.Atan2(m_horizontalInput, m_verticalInput) * Mathf.Rad2Deg;
		float vectorMagnitude = new Vector2(m_horizontalInput, m_verticalInput).magnitude;

		largeWheelR.motorTorque = vectorMagnitude * CalculateConstantC(controllerAngle, true) * motorForce;
		largeWheelL.motorTorque = vectorMagnitude * CalculateConstantC(controllerAngle, false) * motorForce;

	}

	private void UpdateWheelPoses()
	{
		UpdateWheelPose(largeWheelL, largeWheelLT);
		UpdateWheelPose(largeWheelR, largeWheelRT);
		UpdateWheelPose(smallWheelLB, smallWheelLBT);
		UpdateWheelPose(smallWheelRB, smallWheelRBT);
		UpdateWheelPose(smallWheelLF, smallWheelLFT);
		UpdateWheelPose(smallWheelRF, smallWheelRFT);
	}

	private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
	{
		Vector3 _pos = _transform.position;
		Quaternion _quat = _transform.rotation;

		_collider.GetWorldPose(out _pos, out _quat);

		_transform.position = _pos;
		_transform.rotation = _quat;
	}

	private void FixedUpdate()
	{
		GetInput();
		Accelerate();
		UpdateWheelPoses();
	}

}