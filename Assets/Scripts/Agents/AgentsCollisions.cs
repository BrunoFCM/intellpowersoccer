using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsCollisions : MonoBehaviour
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
        /*if (
                collision.gameObject.tag != "Ball" &&
                collision.gameObject.tag != "field" && collision.gameObject.tag != "wall"
            ) {
                foreach(Collider col in GetComponents<Collider>())
                    Physics.IgnoreCollision(collision.collider, col);
                foreach(Collider col in GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(collision.collider, col);
            }*/
    }
}
