using UnityEngine;
using System.Collections;

public class LimitedMouseLook : MonoBehaviour {
	[SerializeField] bool inverted = false;
	[SerializeField] float sensitivity = 5f;
	[SerializeField] float minX = -50f;
	[SerializeField] float maxX = +50f;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float rot = Input.GetAxis ("Mouse Y") * sensitivity;
		if (!inverted) {
			rot *= -1;
		}
		transform.Rotate(rot, 0f, 0f);

		transform.localRotation = ClampRotationAroundXAxis(transform.localRotation);
			//Quaternion.Euler (new Vector3(Mathf.Clamp(transform.localRotation.x, minX, maxX), 0, 0));
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, minX, maxX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}
}
