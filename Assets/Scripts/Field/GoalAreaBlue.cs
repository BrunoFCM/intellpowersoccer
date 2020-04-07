using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAreaBlue : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Collider Ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.name == Ball.name){
            gameEnvironment.setGoalAtBlueGoal();
        }
    }
}