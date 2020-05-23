using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAreaRed : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Collider Ball;
    public StrikeTheBallTrainer strikeTheBallTrainer;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.name == Ball.name){
            gameEnvironment.setGoalAtRedGoal();
            strikeTheBallTrainer.scoredRedGoal();
        }
    }
}