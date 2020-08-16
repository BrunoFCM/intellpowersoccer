using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCore : MonoBehaviour
{
    Rigidbody agentRBody;
    public GameObject tracker;
    public BehaviourHandler behaviourHandler;
    public GameEnvironmentInfo gameEnvironment;
    public WheelchairAgentController wheelchairAgentController;
    public GameObject figurePoint;
    bool trackerBool;

    Vector2 originalPosition;

    public enum Team{
        BLUE,
        RED,
    }
    public enum Type{
        DEFENSIVE,
        ATTACK,
        GOALKEEPER,
    }

    public enum Areas{
        smallRedArea,
        smallBlueArea,
        halfFieldBlue,
        halfFieldRed,
    }

    private Areas currentPositionInField;


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
        public bool controllerDisabled;


    // Start is called before the first frame update
    void Start()
    {
        agentRBody = GetComponent<Rigidbody>();
        trackerBool = false;
        controllerDisabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //DetectAreaCollisions();
        if(!controllerDisabled)
            if(GameEnvironmentInfo.choosenTeam){
                if(tag == "Agent1"){
                    wheelchairAgentController.Controller(null);
                    transform.GetChild(11).gameObject.SetActive(true);
                }
            }else{
                if(tag == "Agent5"){
                    wheelchairAgentController.Controller(null);
                    transform.GetChild(11).gameObject.SetActive(true);
                }
            }
        
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

    }

    public void disableController(){
        controllerDisabled = true;
    }

    public void enableController()
    {
        controllerDisabled = false;
    }
    public void stopChair(){
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().transform.localRotation = Quaternion.Euler(0f,0f,0f);



        transform.GetChild(7).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(7).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetChild(7).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().transform.localRotation = Quaternion.Euler(-90f,0f,0f);
        transform.GetChild(7).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().transform.localPosition = new Vector3(0.2361405f, 0.2113286f, -0.2106989f);
        
        transform.GetChild(7).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(7).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetChild(7).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().transform.localRotation = Quaternion.Euler(-90f,0f,0f);
        transform.GetChild(7).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().transform.localPosition = new Vector3(0.2359247f, 0.1291838f, -0.1733153f);


        transform.GetChild(9).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(9).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetChild(9).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().transform.localRotation = Quaternion.Euler(-90f,0f,0f);
        transform.GetChild(9).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().transform.localPosition = new Vector3(-0.2361402f, 0.2113283f, -0.2106989f);

        transform.GetChild(9).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(9).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetChild(9).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().transform.localRotation = Quaternion.Euler(-90f,0f,0f);
        transform.GetChild(9).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().transform.localPosition = new Vector3(-0.2359247f, 0.1291838f, -0.1733153f);

        wheelchairAgentController.motorForce = 0;
        wheelchairAgentController.motorForce = 50;
    }

    public void disableChair(){
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(2).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.GetChild(3).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                transform.GetChild(3).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.GetChild(4).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                transform.GetChild(4).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.GetChild(5).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    transform.GetChild(5).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.GetChild(7).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(7).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetChild(7).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;

        transform.GetChild(9).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.GetChild(9).transform.GetChild(1).gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetChild(9).transform.GetChild(0).gameObject.GetComponent<Rigidbody>().rotation = Quaternion.identity;

        wheelchairAgentController.motorForce = 0;
    }

    public void enableChair(){
        wheelchairAgentController.motorForce = 50;
    }


    //Player Distance to another Player player
    public float distanceToPlayer(AgentCore player){
        return Vector3.Distance(transform.position, player.transform.position);
    }

    //Palyer distance to the Ball
    public float distanceToBall(){
        return Vector3.Distance(transform.position, ball.transform.position);
    }

    public void setPlayersAtHalfSideAreaBlue(){
        //Debug.Log("Collision with halfSideAreaBlue");
        gameEnvironment.setPlayersAtHalfSideAreaBlue(this);
    }

    public void setPlayersAtHalfSideAreaRed(){
        //Debug.Log("Collision with halfSideAreaRed");
        gameEnvironment.setPlayersAtHalfSideAreaRed(this);
    }

    public void setPlayersAtSmallAreaBlue(){
        //Debug.Log("Collision with smallAreaBlue");
        gameEnvironment.setPlayersAtSmallAreaBlue(this);
    }

    public void setPlayersAtSmallAreaRed(){
        //Debug.Log("Collision with smallAreaRed");
        gameEnvironment.setPlayersAtSmallAreaRed(this);
    }

    public void setCurrentPositionInField(Areas a){
        currentPositionInField = a;
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

    public Areas getCurrentPositionInField(){
        return currentPositionInField;
    }


    //SETS
    public void setNrOfGoals(){
        nrOfGoals += 1;
    }

    public void setFault(){
        if(nrOfNeutralFaults >= 1)
            setNrOfYellowCards();
        else
            nrOfNeutralFaults += 1;
    }

    public void setNrOfYellowCards(){
        nrOfYellowCards += 1;
         if(nrOfYellowCards == 2)
            setExpelled();            
    }

    public void setExpelled(){
         isExpelled = true;
    }

    public void disableTracker(){
        /* COMMENT TO TRAIN
        behaviourHandler.disableAllBehaviours();
        tracker.SetActive(false);
        trackerBool = false;*/
    }
    public void enableTracker(){
        tracker.SetActive(true);
        trackerBool = true;
    }

    public bool getTrackerBool(){
        return trackerBool;
    }

    public void setPassTheBallBehaviour(){
        figurePoint.SetActive(false);
        behaviourHandler.setPassTheBallBehaviour();
    }

    public void setStrikeTheBallBehaviour(){
        figurePoint.SetActive(false);
        behaviourHandler.setStrikeTheBallBehaviour();
    }

    public void setGoalKeepBehaviour(AgentCore shooter){
        behaviourHandler.setGoalKeepBehaviour(shooter);
        figurePoint.SetActive(false);
    }

    public void setDribbleBallBehaviour(Vector2 figurePos){
        //Debug.Log(figurePos.magnitude);
        //if(!behaviourHandler.dribbleBallTrainer.activeSelf){
            figurePoint.SetActive(true);
            figurePoint.transform.position = new Vector3(figurePos.x, 0.5f, figurePos.y);
            behaviourHandler.setDribbleBallBehaviour();
        //}
    }

    public void setMoveToPointBehaviour(Vector2 figurePos){
        //Debug.Log(figurePos.magnitude);
        //if(!behaviourHandler.moveToPointTrainer.activeSelf){
            figurePoint.SetActive(true);
            figurePoint.transform.position = new Vector3(figurePos.x, 0.5f, figurePos.y);
            behaviourHandler.setMoveToPointBehaviour();
        //}
    }

    public void setIntersectBallBehaviour(AgentCore agent, AgentCore nearestPlayer){
        figurePoint.SetActive(false);
        behaviourHandler.setIntersectBallBehaviour(agent, nearestPlayer);
    }

    public void disableAllBehaviours(){
        figurePoint.SetActive(false);
        behaviourHandler.disableAllBehaviours();
    }

    public float getDistanceToGoal(){
        if(team == AgentCore.Team.RED)
            return Vector3.Distance(transform.position, new Vector3(-14,0,0));
        else
            return Vector3.Distance(transform.position, new Vector3(14,0,0));
    }

    public void setOriginalPosition(Vector2 pos){
        originalPosition = pos;
    }

    public Vector2 getOriginalPosition(){
        return originalPosition;
    }
}
