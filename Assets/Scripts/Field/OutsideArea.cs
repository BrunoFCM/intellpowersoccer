using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideArea : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Collider Ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.name == Ball.name){
            gameEnvironment.setBallOutOfBounds();
            gameEnvironment.setBallOutOfBoundsTimeOut(true);
        }
    }
}
