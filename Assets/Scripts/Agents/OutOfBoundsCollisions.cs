using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsCollisions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision) {
        //Debug.Log("COL: "+collision.gameObject.tag);
        if (
                collision.gameObject.tag == "Agent1" || 
                collision.gameObject.tag == "Agent2" || 
                collision.gameObject.tag == "Agent3" || 
                collision.gameObject.tag == "Agent4" || 
                collision.gameObject.tag == "Agent5" || 
                collision.gameObject.tag == "Agent6" || 
                collision.gameObject.tag == "Agent7" || 
                collision.gameObject.tag == "Agent8" ||
                collision.gameObject.tag == "wheels"
            ) {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
    }

    private void OnCollisionStay(Collision collision) {
        //Debug.Log("COL: "+collision.gameObject.tag);
        if (
                collision.gameObject.tag == "Agent1" || 
                collision.gameObject.tag == "Agent2" || 
                collision.gameObject.tag == "Agent3" || 
                collision.gameObject.tag == "Agent4" || 
                collision.gameObject.tag == "Agent5" || 
                collision.gameObject.tag == "Agent6" || 
                collision.gameObject.tag == "Agent7" || 
                collision.gameObject.tag == "Agent8" ||
                collision.gameObject.tag == "wheels"
            ) {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
    }
}
