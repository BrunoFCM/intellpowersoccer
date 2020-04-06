using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsInfo : MonoBehaviour
{
    public float speed;
    public float angularSpeed;
    protected Rigidbody r;

    // Start is called before the first frame update
    void Start()
    {
        r = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = r.velocity.magnitude;
        angularSpeed = r.angularVelocity.magnitude;
    }
}
