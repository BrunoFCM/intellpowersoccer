using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfMass : MonoBehaviour

{
    public Vector3 CenterOfMassPosition;
    public bool awake;
    protected Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rig.centerOfMass = CenterOfMassPosition;
        rig.WakeUp();
        awake = !rig.IsSleeping();
    }

     private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * CenterOfMassPosition, 0.1f);
    }
}
