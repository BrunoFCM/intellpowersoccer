﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCore : MonoBehaviour
{
    Rigidbody agentRBody;
    public GameEnvironmentInfo gameEnvironment;
    public WheelchairAgentController wheelchairAgentController;

    public enum Team{
        BLUE,
        RED,
    }
    public enum Type{
        DEFENSIVE,
        ATTACK,
        CENTRAL,
    }


    //AGENT GENERAL INFORMATION
        //Blue Team or Red Team
        public Team team;
        //Agent Type: DEFENSIVE, ATTACK, CENTRAL
        public Type type;
        //Number of scored Goals
        private int nrOfGoals = 0;
        //Fault Types and counted number
        private int nrOfNeutralFaults = 0;
        private int nrOfYellowCards = 0;
        //Is player expelled?
        private bool isExpelled = false;


    //PLAYER COLLISION VARS
        public FeetCollisions feetCollisions;
        public GoalAreaBlue goalAreaBlue;
        public GoalAreaRed goalAreaRed;
        public HalfSideAreaBlue halfSideAreaBlue;
        public HalfSideAreaRed halfSideAreaRed;
        public SmallAreaBlue smallAreaBlue;
        public SmallAreaRed smallAreaRed;
        public OutsideArea outsideArea;


    //GAME OBJECTS
        //Ball Game object
        public GameObject ball;


    // Start is called before the first frame update
    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //DetectAreaCollisions();
    }

    //Is the player near the ball? TRUE yes, FALSE no
    public bool isNearBall(){
        if(distanceToBall() <= 1.5)
            return true;
        else
            return false;
    }

    public void touchedBall(){
        gameEnvironment.setLastPlayerTouchingBall(this);
        wheelchairAgentController.playerTouchedBall();
    }

    //Player Distance to another Player player
    public float distanceToPlayer(AgentCore player){
        return Vector3.Distance(transform.localPosition, player.transform.localPosition);
    }

    //Palyer distance to the Ball
    public float distanceToBall(){
        return Vector3.Distance(transform.localPosition, ball.transform.localPosition);
    }

    public void setPlayersAtHalfSideAreaBlue(){
        Debug.Log("Collision with halfSideAreaBlue");
        gameEnvironment.setPlayersAtHalfSideAreaBlue(this);
    }

    public void setPlayersAtHalfSideAreaRed(){
        Debug.Log("Collision with halfSideAreaRed");
        gameEnvironment.setPlayersAtHalfSideAreaRed(this);
    }

    public void setPlayersAtSmallAreaBlue(){
        Debug.Log("Collision with smallAreaBlue");
        gameEnvironment.setPlayersAtSmallAreaBlue(this);
    }

    public void setPlayersAtSmallAreaRed(){
        Debug.Log("Collision with smallAreaRed");
        gameEnvironment.setPlayersAtSmallAreaRed(this);
    }

    public void setPlayersAtOutsideArea(){
        Debug.Log("Collision with outsideArea");
        gameEnvironment.setPlayersAtOutsideArea(this);
    }


    //GETS
    public int getNrOfGoals(){
        return nrOfGoals;
    }

    public int getTotalNrOfFaults(){
        return nrOfNeutralFaults + nrOfYellowCards;
    }

    public int getNrOfNeutralFaults(){
        return nrOfNeutralFaults;
    }
    public int getNrOfYellowCards(){
        return nrOfYellowCards;
    }

    public Rigidbody getAgentRBody(){
        return agentRBody;
    }

    //SETS
    public void setNrOfGoals(){
        nrOfGoals += 1;
    }

    public void setNrOfNeutralFaults(){
        nrOfNeutralFaults += 1;
    }
    public void setNrOfYellowCards(){
         nrOfYellowCards += 1;

         if(nrOfYellowCards == 2){
             isExpelled = true;
         }
    }


}