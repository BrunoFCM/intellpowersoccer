using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEnvironmentInfo : MonoBehaviour
{
    //GAME VARS
        //Game Duration of 40min = 2400s
        private float gameTime = 2400.0f;
        private int redScore = 0;
        private int blueScore = 0;
        public List<AgentCore> redTeamAgents;
        public List<AgentCore> blueTeamAgents;
        public Ball Ball;
        private AgentCore.Team tossingWinner;
        private bool positioningInXEnd;

        //While this is true, players cannot commit fouls bacause they are in a indirect free kick
        private bool foulTimeOut;
        //While this is true, players cannot commit Ball Out of Bounds because they are in a indirect free kick
        private bool ballOutOfBoundsTimeOut;


    //AGENTS GENERAL INFO
        //Player who has the ball in its possession
        private AgentCore playerWithBall;
        //Last player touchinh the ball
        private AgentCore lastPlayerTouchingTheBall;
        //Players who are confronting the who has the ball in its possession (playerWithBall)
        private AgentCore opponent;
        private bool initialPositions;
        private AgentCore initialPlayerTakingKick;
        private Vector3 initialPlayerTakingKickPos;


    //NUMBER OF PLAYERS AT EACH AREA OF THE FIELD
        private List<AgentCore> playersAtHalfSideAreaBlue;
        private List<AgentCore> playersAtHalfSideAreaRed;
        private List<AgentCore> playersAtSmallAreaBlue;
        private List<AgentCore> playersAtSmallAreaRed;
        private List<AgentCore> playersAtOutsideArea;


    //BALL OUT OF BOUNDS INFO
        //If outOfBound is set to true means that we are in a Ball Out Of Bounds period of the game (all agents are paused)
        private bool outOfBounds;
        private Vector3 outBoundsBallPos;
        private Vector3 outBoundsAgentPos;
        private AgentCore outBoundsAgent;
        private float outBoundsAgentRot;
        private bool outOfBoundsAreaFreeKick;
        private Vector3 outBoundsAreaFreeKickAgentPos;
        private AgentCore outBoundsFreeKickAgent;
        private float outBoundsFreeKickAgentRot;


    //foul INFO
        //if foulCommited is set to true mean that we are in a fault period of the game (all agents are paused)
        private bool foulCommited;
        private Vector3 foulBallPos;
        private Vector3 foulAgentPos;
        private AgentCore foulAgent;
        private float foulAgentRot;
        private bool penaltyActive;
        private AgentCore penaltyAgent;
        private Vector3 penaltyAgentPos;
        private float penaltyAgentRot;
        private bool faulTimeOut;



    // Start is called before the first frame update
    void Start()
    {
        playerWithBall = null;
        lastPlayerTouchingTheBall = null;
        opponent = null;

        playersAtHalfSideAreaBlue = new List<AgentCore>();
        playersAtHalfSideAreaRed = new List<AgentCore>();
        playersAtSmallAreaBlue = new List<AgentCore>();
        playersAtSmallAreaRed = new List<AgentCore>();
        playersAtOutsideArea = new List<AgentCore>();

        ballOutOfBoundsTimeOut = false;

        setBallCenterPos();

        if (new System.Random().Next(0, 2) == 0)
            setInitialPositions(AgentCore.Team.BLUE);
        else
            setInitialPositions(AgentCore.Team.RED);

        

        lastPlayerTouchingTheBall = null;
        faulTimeOut = false;

        positioningInXEnd = false;
    
    }

    // Update is called once per frame
    void Update()
    {   
        if(!foulCommited)
            foulControlSystem();

        if(outOfBounds)
            limitWalkingArea(outBoundsAgent, outBoundsAgentPos, outBoundsAgentRot);

        if(outOfBoundsAreaFreeKick)
            limitWalkingArea(outBoundsFreeKickAgent, outBoundsAreaFreeKickAgentPos, outBoundsFreeKickAgentRot);

        if(!foulTimeOut && !outOfBounds && !outOfBoundsAreaFreeKick && !initialPositions && !penaltyActive)
            if(threeInTheGoalAreafoul()){
                Debug.Log("3 in the Goal Area Foul Committed");
            }

        if(initialPositions){
            limitInicialPosWalkingArea(initialPlayerTakingKick, initialPlayerTakingKickPos);
        }

        if(lastPlayerTouchingTheBall != null){
            outOfBoundsAreaFreeKick = false;
            penaltyActive = false;
            foulTimeOut = false;
            initialPositions = false;
        }

        if(penaltyActive){
            limitWalkingArea(penaltyAgent, penaltyAgentPos, penaltyAgentRot);
        }

        if(foulTimeOut){
            limitFoulWalkingArea(foulAgent, foulAgentPos);
        } 
    }

    private void FixedUpdate() {
        //if(foulTimeOut || penaltyActive || outOfBoundsAreaFreeKick || outOfBounds)
            //detectPlayersAtSamePosBug();
    }










// ----------------------------------------------------------- GENERAL GAME FUNCS -----------------------------------------------------------

    //Sets the initial positions os the players in the field
    public void setInitialPositions(AgentCore.Team team){
        
        if(team == AgentCore.Team.BLUE){
            redTeamAgents[0].transform.localPosition = new Vector3(-4f, 0.25f, 0f);
            redTeamAgents[1].transform.localPosition = new Vector3(-11.5f, 0.25f, 0f);
            redTeamAgents[2].transform.localPosition = new Vector3(-7, 0.25f, 4.5f);
            redTeamAgents[3].transform.localPosition = new Vector3(-7, 0.25f, -4.5f);

            blueTeamAgents[0].transform.localPosition = new Vector3(1.5f, 0.25f, 0f);
            blueTeamAgents[1].transform.localPosition = new Vector3(11.5f, 0.25f, 0f);
            blueTeamAgents[2].transform.localPosition = new Vector3(2f, 0.25f, 5f);
            blueTeamAgents[3].transform.localPosition = new Vector3(2f, 0.25f, -5f);

            initialPlayerTakingKick = blueTeamAgents[0];
            initialPlayerTakingKickPos = blueTeamAgents[0].transform.localPosition;
        }
        else{
            redTeamAgents[0].transform.localPosition = new Vector3(-1.5f, 0.25f, 0f);
            redTeamAgents[1].transform.localPosition = new Vector3(-11.5f, 0.25f, 0f);
            redTeamAgents[2].transform.localPosition = new Vector3(-2, 0.25f, 5f);
            redTeamAgents[3].transform.localPosition = new Vector3(-2, 0.25f, -5f);

            blueTeamAgents[0].transform.localPosition = new Vector3(4.25f, 0.25f, 0f);
            blueTeamAgents[1].transform.localPosition = new Vector3(11.5f, 0.25f, 0f);
            blueTeamAgents[2].transform.localPosition = new Vector3(7f, 0.25f, 4.5f);
            blueTeamAgents[3].transform.localPosition = new Vector3(7f, 0.25f, -4.5f);

            initialPlayerTakingKick = redTeamAgents[0];
            initialPlayerTakingKickPos = redTeamAgents[0].transform.localPosition;
        }

        foreach(AgentCore agent in redTeamAgents){
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
        foreach(AgentCore agent in blueTeamAgents){
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }

        lastPlayerTouchingTheBall = null;
        initialPositions = true;
    }

    private void limitInicialPosWalkingArea(AgentCore agent, Vector3 pos){
        Vector3 centerPosition = Ball.transform.localPosition; //center of circle
        float distance = Vector3.Distance(agent.transform.localPosition, centerPosition);
        
        if (distance > 3)
        {
            agent.transform.localPosition = initialPlayerTakingKickPos;
            agent.stopChair();

            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
    }

    public void setPenaltyPositions(AgentCore.Team teamTakingKick){
        setBallPenaltyPos(teamTakingKick);
        
        if(teamTakingKick == AgentCore.Team.RED){
            redTeamAgents[0].transform.localPosition = new Vector3(10f, 0.25f, 0f);
            redTeamAgents[1].transform.localPosition = new Vector3(-3f, 0.25f, 0f);
            redTeamAgents[2].transform.localPosition = new Vector3(4f, 0.25f, 4.5f);
            redTeamAgents[3].transform.localPosition = new Vector3(4f, 0.25f, -4.5f);

            blueTeamAgents[0].transform.localPosition = new Vector3(5f, 0.25f, 0f);
            blueTeamAgents[1].transform.localPosition = new Vector3(14.5f, 0.25f, 0f);
            blueTeamAgents[2].transform.localPosition = new Vector3(6f, 0.25f, 3f);
            blueTeamAgents[3].transform.localPosition = new Vector3(6f, 0.25f, -3f);

            penaltyAgent = redTeamAgents[0];
            penaltyAgentPos = new Vector3(10f, 0.25f, 0f);
            penaltyAgentRot = -90;
        }
        else{
            redTeamAgents[0].transform.localPosition = new Vector3(-5f, 0.25f, 0f);
            redTeamAgents[1].transform.localPosition = new Vector3(-14.5f, 0.25f, 0f);
            redTeamAgents[2].transform.localPosition = new Vector3(-6, 0.25f, 3f);
            redTeamAgents[3].transform.localPosition = new Vector3(-6, 0.25f, -3f);

            blueTeamAgents[0].transform.localPosition = new Vector3(-10f, 0.25f, 0f);
            blueTeamAgents[1].transform.localPosition = new Vector3(3f, 0.25f, 0f);
            blueTeamAgents[2].transform.localPosition = new Vector3(-4f, 0.25f, 4.5f);
            blueTeamAgents[3].transform.localPosition = new Vector3(-4f, 0.25f, -4.5f);

            penaltyAgent = blueTeamAgents[0];
            penaltyAgentPos = new Vector3(-10f, 0.25f, 0f);
            penaltyAgentRot = 0;
        }

        foreach(AgentCore agent in redTeamAgents){
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
        foreach(AgentCore agent in blueTeamAgents){
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
    }

    public void setFreeGoalAreaKickPositions(AgentCore.Team teamTakingKick){
        setBallFreeGoalAreaKick(teamTakingKick);

        if(teamTakingKick == AgentCore.Team.BLUE){
            redTeamAgents[0].transform.localPosition = new Vector3(0f, 0.25f, 0f);
            redTeamAgents[1].transform.localPosition = new Vector3(-11.5f, 0.25f, 0f);
            redTeamAgents[2].transform.localPosition = new Vector3(-3.5f, 0.25f, 4.5f);
            redTeamAgents[3].transform.localPosition = new Vector3(-3.5f, 0.25f, -4.5f);

            blueTeamAgents[0].transform.localPosition = new Vector3(4f, 0.25f, 0f);
            blueTeamAgents[1].transform.localPosition = new Vector3(11.5f, 0.25f, 0f);
            blueTeamAgents[2].transform.localPosition = new Vector3(7f, 0.25f, 4f);
            blueTeamAgents[3].transform.localPosition = new Vector3(7f, 0.25f, -4f);

            outBoundsAreaFreeKickAgentPos = new Vector3(11.5f, 0.25f, 0f);
            outBoundsFreeKickAgent = blueTeamAgents[1];
            outBoundsFreeKickAgentRot = 90;
        }
        else{
            redTeamAgents[0].transform.localPosition = new Vector3(-4f, 0.25f, 0f);
            redTeamAgents[1].transform.localPosition = new Vector3(-11.5f, 0.25f, 0f);
            redTeamAgents[2].transform.localPosition = new Vector3(-7f, 0.25f, 4f);
            redTeamAgents[3].transform.localPosition = new Vector3(-7f, 0.25f, -4f);

            blueTeamAgents[0].transform.localPosition = new Vector3(0f, 0.25f, 0f);
            blueTeamAgents[1].transform.localPosition = new Vector3(11.5f, 0.25f, 0f);
            blueTeamAgents[2].transform.localPosition = new Vector3(3.5f, 0.25f, 4.5f);
            blueTeamAgents[3].transform.localPosition = new Vector3(3.5f, 0.25f, -4.5f);

            outBoundsAreaFreeKickAgentPos = new Vector3(-11.5f, 0.25f, 0f);
            outBoundsFreeKickAgent = redTeamAgents[1];
            outBoundsFreeKickAgentRot = 0;
        }

        foreach(AgentCore agent in redTeamAgents){
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
        foreach(AgentCore agent in blueTeamAgents){
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
    }

    public void disableAllChairs(AgentCore exception1, AgentCore exception2){
        foreach(AgentCore agent in redTeamAgents){
            if(agent != exception1 && agent != exception2){
                agent.stopChair();
            }
        }
        foreach(AgentCore agent in blueTeamAgents){
            if(agent != exception1 && agent != exception2){
                agent.stopChair();
            }
        }
    }

    public void enableAllChairs(){
        foreach(AgentCore agent in redTeamAgents){
            agent.enableChair();
        }
        foreach(AgentCore agent in blueTeamAgents){
            agent.enableChair();
        }
    }

    public void setBallCenterPos(){
        Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Ball.transform.localPosition = new Vector3(0, 0.44f, 0);
    }

    public void setBallFreeGoalAreaKick(AgentCore.Team teamTakingTheKick){
        Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if(teamTakingTheKick == AgentCore.Team.BLUE)
            Ball.transform.localPosition = new Vector3(10f, 0.44f, 0);
        else
            Ball.transform.localPosition = new Vector3(-10f, 0.44f, 0);
    }

    public void setBallPenaltyPos(AgentCore.Team teamTakingTheKick){
        Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if(teamTakingTheKick == AgentCore.Team.RED)
            Ball.transform.localPosition = new Vector3(11.5f, 0.44f, 0);
        else
            Ball.transform.localPosition = new Vector3(-11.5f, 0.44f, 0);
    }

    // Sets the player that has the ball and the opponent; sets null if there is none respectively;
    // It also checks if anyone commited the two-on-one foul
    // Opponent is the player who is in a radius of 3m of the player with the ball
    public void foulControlSystem(){

        var possibleAgentsNearBall = new List<AgentCore>(blueTeamAgents.Count + redTeamAgents.Count);
        possibleAgentsNearBall.AddRange(blueTeamAgents);
        possibleAgentsNearBall.AddRange(redTeamAgents);
        
        //Orders the List of possibleAgentsNearBall by the distanc eof each one to the ball (closest to farest)
        possibleAgentsNearBall = possibleAgentsNearBall.OrderBy(x => x.distanceToBall()).ToList();

        /*
            Debug.Log("-------------------------");
            foreach(AgentCore agent in possibleAgentsNearBall){
                Debug.Log(agent.name + " distance: " +agent.distanceToBall());
            }
        */

        //Check if There's a player with the ball in its possession.
        if(possibleAgentsNearBall[0].isNearBall()){
            //Found a player with the ball in its possession.
            playerWithBall = possibleAgentsNearBall[0];
           // Debug.Log("Player with ball possession: " + playerWithBall.name);

            //Since there's an player with ball possession already, check if theres an opponent
            for(int i = 1; i < possibleAgentsNearBall.Count; i++){
                if(possibleAgentsNearBall[i].distanceToPlayer(playerWithBall) < 3){
                    if(possibleAgentsNearBall[i].team != playerWithBall.team){
                        //Found an opponent
                        opponent = possibleAgentsNearBall[i];
                       // Debug.Log("Opponent: " + opponent.name);

                        //Since there's an opponnent already, we need to check if theres anyone commiting the two-on-one foul
                        for(int j = i+1; j < possibleAgentsNearBall.Count; j++){
                            if(possibleAgentsNearBall[j].distanceToPlayer(playerWithBall) < 4.5){
                                if(possibleAgentsNearBall[j].team == playerWithBall.team){
                                    if(possibleAgentsNearBall[j].type != AgentCore.Type.GOALKEEPER && possibleAgentsNearBall[j-1].type != AgentCore.Type.GOALKEEPER){
                                        
                                        if(j+1 < possibleAgentsNearBall.Count){
                                            if(possibleAgentsNearBall[j+1].type != AgentCore.Type.GOALKEEPER){
                                                //Found a player commiting a foul
                                                Debug.Log(possibleAgentsNearBall[j].name + " commited Two-on-One foul!");
                                                twoOnOnefoulMechanism(possibleAgentsNearBall[0], possibleAgentsNearBall[i], possibleAgentsNearBall[j]);
                                            }
                                        }else{
                                            //Found a player commiting a foul
                                            Debug.Log(possibleAgentsNearBall[j].name + " commited Two-on-One foul!");
                                            twoOnOnefoulMechanism(possibleAgentsNearBall[0], possibleAgentsNearBall[i], possibleAgentsNearBall[j]);
                                        }

                                        
                                    }
                                }
                            }
                            else{
                                break;
                            }
                        }
                        break;
                    }
                    else
                        opponent = null;
                }
                else{
                    opponent = null;
                    break;
                }
            }
        }
        else{
            playerWithBall = null;
            opponent = null;
        }
    }

    public bool getBallOutOfBoundsTimeOut(bool a){
        return ballOutOfBoundsTimeOut;
    }

    public void setBallOutOfBoundsTimeOut(bool a){
        ballOutOfBoundsTimeOut = a;
    }

    public void setLastPlayerTouchingBall(AgentCore agent){
        lastPlayerTouchingTheBall = agent;
    }

    public void setPlayersAtHalfSideAreaBlue(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);

        playersAtHalfSideAreaBlue.Add(agent);
        agent.setCurrentPositionInField(AgentCore.Areas.halfFieldBlue);
    }

    public void setPlayersAtHalfSideAreaRed(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);

        playersAtHalfSideAreaRed.Add(agent);
        agent.setCurrentPositionInField(AgentCore.Areas.halfFieldRed);
    }

    public void setPlayersAtSmallAreaBlue(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);

        playersAtSmallAreaBlue.Add(agent);
        agent.setCurrentPositionInField(AgentCore.Areas.smallBlueArea);
    }

    public void setPlayersAtSmallAreaRed(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);

        playersAtSmallAreaRed.Add(agent);
        agent.setCurrentPositionInField(AgentCore.Areas.smallRedArea);
    }

    public void setPlayersAtOutsideArea(AgentCore agent){

        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);

        playersAtOutsideArea.Add(agent);
    }

    public void setGoalAtRedGoal(){
        blueScore += 1;
        setBallCenterPos();
        setInitialPositions(AgentCore.Team.RED);
        Debug.Log("Score: Blue - " + blueScore);
        Debug.Log("Score: Red - " + redScore);
    }

    public void setGoalAtBlueGoal(){
        redScore += 1;
        setBallCenterPos();
        setInitialPositions(AgentCore.Team.BLUE);
        Debug.Log("Score: Blue - " + blueScore);
        Debug.Log("Score: Red - " + redScore);
    }










// ----------------------------------------------------------- foul FUNCS -----------------------------------------------------------

    public bool checkOutsideOfBounds(Vector3 pos){
        if(pos.x > 14f || 
            pos.x < -14f || 
            pos.z > 7.5f ||
            pos.z < -7.5f)
            return true;
        
        return false;
    }

    public bool checkIfPenalty(AgentCore playerCommitedfoul){
        if(playerCommitedfoul.team == AgentCore.Team.BLUE){
            if(Ball.positionInField == Ball.Areas.smallBlueArea){
                Debug.Log("Penalty");
                return true;
            }
        }else{
            if(Ball.positionInField == Ball.Areas.smallRedArea){
                Debug.Log("Penalty");
                return true;
            }
        }

        return false;
    }

    public void twoOnOnefoulMechanism(AgentCore playerAssistCommitedfoul, AgentCore playerTakingTheKick, AgentCore playerCommitedfoul){
        setfoulCommited(true);

        if(!checkIfPenalty(playerCommitedfoul)){
                placeTeamByAreaFoul(playerTakingTheKick, playerTakingTheKick.team);
        }
        else{
            if(playerCommitedfoul.team == AgentCore.Team.RED){
                setPenaltyPositions(AgentCore.Team.BLUE);
                lastPlayerTouchingTheBall = null;
                penaltyActive = true;
            }
            else
            {
                setPenaltyPositions(AgentCore.Team.RED);
                lastPlayerTouchingTheBall = null;
                penaltyActive = true;
            }
        }
        
        /*setfoulCommited(true);
        playerCommitedfoul.setfoul();
        foulAgent = playerWithBall;
        foulBallPos = Ball.transform.localPosition;
        
        if(foulAgent.transform.localPosition.x >= 0){
            foulAgentPos = new Vector3();
            foulAgentRot
            spawnWheelchairAtNewSpot(1,1,1, foulAgent, 0);
        }
        else{

        }*/
        
    }

    public bool threeInTheGoalAreafoul(){

        int teamMembersInAreaCounter = 0;

        if(playerWithBall == null)
            return false;

        if(playerWithBall.team == AgentCore.Team.RED){
            if(playersAtSmallAreaBlue.Count > 2){
                for(int i = 0; i < playersAtSmallAreaBlue.Count; i++){
                    if(playersAtSmallAreaBlue[i].team == AgentCore.Team.BLUE){
                        teamMembersInAreaCounter++;
                    }
                }
                if(teamMembersInAreaCounter > 2){
                    if(Ball.positionInField == Ball.Areas.smallBlueArea){
                        Debug.Log("Penalty");
                        setPenaltyPositions(AgentCore.Team.RED);
                        lastPlayerTouchingTheBall = null;
                        penaltyActive = true;
                    }else{
                        placeTeamByAreaFoul(playerWithBall, playerWithBall.team);
                    }

                    return true;
                }
            }
        }
        else if(playerWithBall.team == AgentCore.Team.BLUE){
            if(playersAtSmallAreaRed.Count > 2){
                for(int i = 0; i < playersAtSmallAreaRed.Count; i++){
                    if(playersAtSmallAreaRed[i].team == AgentCore.Team.RED){
                        teamMembersInAreaCounter++;
                    }
                }
                if(teamMembersInAreaCounter > 2){
                    if(Ball.positionInField == Ball.Areas.smallRedArea){
                        Debug.Log("Penalty");
                        setPenaltyPositions(AgentCore.Team.BLUE);
                        lastPlayerTouchingTheBall = null;
                        penaltyActive = true;
                    }else{
                        placeTeamByAreaFoul(playerWithBall, playerWithBall.team);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public void indirectFreeKickOutsideGoalArea(AgentCore agentSufferingFoul, AgentCore agentCommitingFoul, Vector3 foulPos){
        
        if(agentSufferingFoul.team == AgentCore.Team.RED){
            //Team RED suffer foul in Blue Area (Attacking)
            if(foulPos.x > 0){
                //Player spawns must be mainly at Bottom
                if(foulPos.z > 0){

                }
                //Player spawns must be mainly at Top
                else{

                }
            }
            //Team RED suffer foul in RED Area (Defending)
            else{

            }
        }
        else{
            
        }
    }

    public void placeTeamByAreaFoul(AgentCore playerTakingTheKick, AgentCore.Team teamTakingKick){

        Vector3 ballSavedPosition = new Vector3(Ball.transform.localPosition.x, Ball.transform.localPosition.y, Ball.transform.localPosition.z);
        
        Ball.stopIt();
        Ball.transform.localPosition = ballSavedPosition;

        //BLUE ATTACKING
        if(teamTakingKick == AgentCore.Team.BLUE){
            List<AgentCore> remainBlueTeamAgents = new List<AgentCore>(blueTeamAgents.Count);

            foreach(AgentCore agent in blueTeamAgents){
                remainBlueTeamAgents.Add(agent);
            }

            remainBlueTeamAgents.Remove(playerTakingTheKick);



            //ZONE 1
            if(Ball.transform.localPosition.x >= -14 && Ball.transform.localPosition.x < -7){
                if(Ball.transform.localPosition.z > 0){

                    positionPlayer(redTeamAgents[0], -7.5f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -13f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -11.5f, 3f, -90);
                    positionPlayer(redTeamAgents[3], -6f, 0f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -5f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -9f, 5f, 90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -5f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -9f, 5f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -5f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -9f, 5f, 90);
                    }

                }
                else{

                    positionPlayer(redTeamAgents[0], -7.5f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -13f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -11.5f, -3f, -90);
                    positionPlayer(redTeamAgents[3], -6f, 0f, -90);

                    if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -5f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -9f, -5f, 90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -5f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -9f, -5f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -5f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -9f, -5f, 90);
                    }

                }
            }
            //ZONE 2
            else if(Ball.transform.localPosition.x >= -7 && Ball.transform.localPosition.x < 0){
                if(Ball.transform.localPosition.z > 0){

                    positionPlayer(redTeamAgents[0], -4.5f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -11.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -9f, 4f, -90);
                    positionPlayer(redTeamAgents[3], -7f, 0f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 1f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -6f, 5f, -90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], 1f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -6f, 5f, -90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], 1f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -6f, 5f, -90);
                    }

                }
                else{

                    positionPlayer(redTeamAgents[0], -4.5f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -11.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -9f, -4f, -90);
                    positionPlayer(redTeamAgents[3], -7f, 0f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 1f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -6f, -5f, -90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], 1f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], -6f, -5f, -90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], 1f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -6f, -5f, -90);
                    }
                }

            }
            //ZONE 3
            else if(Ball.transform.localPosition.x >= 0 && Ball.transform.localPosition.x < 7){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(redTeamAgents[0], 0f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -10.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -5.5f, 4f, -90);
                    positionPlayer(redTeamAgents[3], -3f, -0.5f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 9f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 4f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 5f, 5f, 90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], 9f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 4f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 5f, 5f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], 9f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 4f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 5f, 5f, 90);
                    }
                }
                else{
                    positionPlayer(redTeamAgents[0], 0f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -10.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -5.5f, -4f, -90);
                    positionPlayer(redTeamAgents[3], -3f, -0.5f, -90);

                    if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 9f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 4f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 5f, -5f, 90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], 9f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 4f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 5f, -5f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], 9f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 4f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 5f, -5f, 90);
                    }
                }
            }
            //ZONE 4
            else if(Ball.transform.localPosition.x >= 7 && Ball.transform.localPosition.x <= 14){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(redTeamAgents[0], 4f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -6f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -1.5f, 4f, -90);
                    positionPlayer(redTeamAgents[3], 1f, -0.5f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 12f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 7.5f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 6f, 5f, -90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], 12f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 7.5f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 6f, 5f, -90);                  
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], 12f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 7.5f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 6f, 5f, -90);
                    }
                }
                else{
                    positionPlayer(redTeamAgents[0], 4f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -6f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -1.5f, -4f, -90);
                    positionPlayer(redTeamAgents[3], 1f, 0.5f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 12f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 7.5f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[2], 6f, -5f, -90);
                    }
                    else if(remainBlueTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[1], 12f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 7.5f, 0f, 90); 
                        positionPlayer(remainBlueTeamAgents[2], 6f, -5f, -90);                   
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[2], 12f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 7.5f, 0f, 90); 
                        positionPlayer(remainBlueTeamAgents[0], 6f, -5f, -90);
                    }
                }
            }
        }
        else{
            List<AgentCore> remainRedTeamAgents = new List<AgentCore>(redTeamAgents.Count);

            foreach(AgentCore agent in redTeamAgents){
                remainRedTeamAgents.Add(agent);
            }

            remainRedTeamAgents.Remove(playerTakingTheKick);

            //ZONE 1
            if(Ball.transform.localPosition.x >= -14 && Ball.transform.localPosition.x < -7){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(blueTeamAgents[0], -4f, 2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 6f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 1.5f, 4f, 90);
                    positionPlayer(blueTeamAgents[3], -1f, -0.5f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -12f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -7.5f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -6f, 5f, 90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], -12f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -7.5f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -6f, 5f, 90);                   
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], -12f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -7.5f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], -6f, 5f, 90); 
                    }
                }
                else{
                    positionPlayer(blueTeamAgents[0], -4f, -2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 6f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 1.5f, -4f, 90);
                    positionPlayer(blueTeamAgents[3], -1f, 0.5f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -12f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -7.5f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -6f, -5f, 90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], -12f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -7.5f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -6f, -5f, 90);                    
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], -12f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -7.5f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], -6f, -5f, 90);
                    }

                }
            }
            //ZONE 2
            else if(Ball.transform.localPosition.x >= -7 && Ball.transform.localPosition.x < 0){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(blueTeamAgents[0], 0f, 2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 5.5f, 4f, 90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -5f, 5f, -90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], -9f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -5f, 5f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], -9f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], -5f, 5f, -90);
                    }
                }
                else{
                    positionPlayer(blueTeamAgents[0], 0f, -2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 5.5f, -4f, 90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, 90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -5f, -5f, -90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], -9f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[2], -5f, -5f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], -9f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], -5f, -5f, -90);
                    }
                }

            }
            //ZONE 3
            else if(Ball.transform.localPosition.x >= 0 && Ball.transform.localPosition.x < 7){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(blueTeamAgents[0], 0f, 2.5f, -90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, -90);
                    positionPlayer(blueTeamAgents[2], 4f, 1.5f, -90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, -90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, 2f, 90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, 90);
                        positionPlayer(remainRedTeamAgents[2], 6f, 5f, 90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], -9f, 2f, 90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, 90);
                        positionPlayer(remainRedTeamAgents[2], 6f, 5f, 90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], -9f, 2f, 90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, 90);
                        positionPlayer(remainRedTeamAgents[0], 6f, 5f, 90);
                    }
                }
                else{
                    positionPlayer(blueTeamAgents[0], 0f, -2.5f, -90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, -90);
                    positionPlayer(blueTeamAgents[2], 4f, -1.5f, -90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, -90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, -2f, 90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, 90);
                        positionPlayer(remainRedTeamAgents[2], 6f, -5f, 90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], -9f, -2f, 90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, 90);
                        positionPlayer(remainRedTeamAgents[2], 6f, -5f, 90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], -9f, -2f, 90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, 90);
                        positionPlayer(remainRedTeamAgents[0], 6f, -5f, 90);
                    }
                }
            }
            //ZONE 4
            else if(Ball.transform.localPosition.x >= 7 && Ball.transform.localPosition.x <= 14){
                if(Ball.transform.localPosition.z > 0){

                    positionPlayer(blueTeamAgents[0], 7.5f, 2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 13f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 11.5f, 3f, 90);
                    positionPlayer(blueTeamAgents[3], 6f, 0f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[1], 5f, -3f, -90);
                        positionPlayer(remainRedTeamAgents[2], 9f, 5f, -90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], 5f, -3f, -90);
                        positionPlayer(remainRedTeamAgents[2], 9f, 5f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[1], 5f, -3f, -90);
                        positionPlayer(remainRedTeamAgents[0], 9f, 5f, -90);
                    }

                }
                else{

                    positionPlayer(blueTeamAgents[0], 7.5f, -2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 13f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 11.5f, -3f, 90);
                    positionPlayer(blueTeamAgents[3], 6f, 0f, 90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[1], 5f, 3f, -90);
                        positionPlayer(remainRedTeamAgents[2], 9f, -5f, -90);
                    }
                    else if(remainRedTeamAgents[1].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[1], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], 5f, 3f, -90);
                        positionPlayer(remainRedTeamAgents[2], 9f, -5f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[2], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[1], 5f, 3f, -90);
                        positionPlayer(remainRedTeamAgents[0], 9f, -5f, -90);
                    }
                }
            }
        }

        Ball.transform.localPosition = ballSavedPosition;
        positionAgentTakingKick(playerTakingTheKick);
        detectPlayersAtSamePosBugAfterFoul(playerTakingTheKick);

    }

    public void positionAgentTakingKick(AgentCore playerTakingTheKick){

        float xBall = Ball.transform.localPosition.x;
        float zBall = Ball.transform.localPosition.z;
        float xGoal;

        if(playerTakingTheKick.team == AgentCore.Team.RED)
            xGoal = 14;
        else
            xGoal = -14;

        float zGoal = 0;
        float t = -1.5f/Vector3.Distance(new Vector3(xBall, 0, zBall), new Vector3(xGoal, 0, zGoal));
        float newX = ((1 - t)*xBall + t*xGoal);
        float newZ = ((1 - t)*zBall + t*zGoal);


        playerTakingTheKick.transform.localPosition = new Vector3(newX, playerTakingTheKick.transform.localPosition.y, newZ);

        playerTakingTheKick.stopChair();

        foulAgent = playerTakingTheKick;
        foulAgentPos = playerTakingTheKick.transform.localPosition;
        foulBallPos = Ball.transform.localPosition;
        foulTimeOut = true;
        lastPlayerTouchingTheBall = null;
    }

    private void limitFoulWalkingArea(AgentCore agent, Vector3 pos){
        Vector3 centerPosition = foulBallPos; //center of circle
        float distance = Vector3.Distance(agent.transform.localPosition, centerPosition);
        
        if (distance > 3)
        {
            agent.transform.localPosition = new Vector3(pos.x, pos.y, pos.z);
            agent.stopChair();
            agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));
        }
    }

    public void detectPlayersAtSamePosBugAfterFoul(AgentCore agentTakingKick){
        List<AgentCore> allPlayers = new List<AgentCore>(blueTeamAgents.Count + redTeamAgents.Count);

        bool bugged = true;

        while(bugged){
            allPlayers = new List<AgentCore>(blueTeamAgents.Count + redTeamAgents.Count);
            allPlayers.AddRange(blueTeamAgents);
            allPlayers.AddRange(redTeamAgents);
            allPlayers.Remove(agentTakingKick);
            allPlayers.Add(agentTakingKick);

            while(allPlayers.Count >= 2){
                for(int j = 1; j < allPlayers.Count; j++){
                    if(allPlayers[0].distanceToPlayer(allPlayers[j]) < 2.5){
                        Debug.Log("FOUND BUG IN PLAYERS POSITIONS");
                        if(allPlayers[0].team == AgentCore.Team.RED){
                            allPlayers[0].transform.localPosition = new Vector3(allPlayers[0].transform.localPosition.x - 0.8f, 0.25f, allPlayers[0].transform.localPosition.z);
                        }
                        else{
                            allPlayers[0].transform.localPosition = new Vector3(allPlayers[0].transform.localPosition.x + 0.8f, 0.25f, allPlayers[0].transform.localPosition.z);
                        }
                    }
                }
                allPlayers.Remove(allPlayers[0]);
            }

            

            bugged = false;

            allPlayers = new List<AgentCore>(blueTeamAgents.Count + redTeamAgents.Count);
            allPlayers.AddRange(blueTeamAgents);
            allPlayers.AddRange(redTeamAgents);
            allPlayers.Remove(agentTakingKick);
            allPlayers.Add(agentTakingKick);

            while(allPlayers.Count >= 2){
                for(int j = 1; j < allPlayers.Count; j++){
                    if(allPlayers[0].distanceToPlayer(allPlayers[j]) < 2.5){
                        Debug.Log("FOUND BUG IN PLAYERS POSITIONS AGAIN");
                        bugged = true;
                        if(allPlayers[0].team == AgentCore.Team.RED){
                            allPlayers[0].transform.localPosition = new Vector3(allPlayers[0].transform.localPosition.x - 0.8f, 0.25f, allPlayers[0].transform.localPosition.z);
                        }
                        else{
                            allPlayers[0].transform.localPosition = new Vector3(allPlayers[0].transform.localPosition.x + 0.8f, 0.25f, allPlayers[0].transform.localPosition.z);
                        }
                    }
                }
                allPlayers.Remove(allPlayers[0]);
            }
        
        }

        positioningInXEnd = true;

    }


    /*public bool checkPositionBugInZ(AgentCore agentTakingKick){
        List<AgentCore> allPlayers = new List<AgentCore>(blueTeamAgents.Count + redTeamAgents.Count);

        allPlayers.AddRange(blueTeamAgents);
        allPlayers.AddRange(redTeamAgents);
        allPlayers.Remove(agentTakingKick);


        bool check = false;

        foreach(AgentCore player in allPlayers){
            if(agentTakingKick.distanceToPlayer(player) < 3){
                if(player.transform.localPosition.z > 0){
                    player.transform.localPosition = new Vector3(player.transform.localPosition.z - 2.5f, 0.25f, player.transform.localPosition.z);
                    check = true;
                }
                else{
                    player.transform.localPosition = new Vector3(player.transform.localPosition.z + 2.5f, 0.25f, player.transform.localPosition.z);
                    check = true;
                }
            }
        }

        if(check)
            return true;

        return false;
    }*/










// ----------------------------------------------------------- BALL OUT OF BOUNDS FUNCS -----------------------------------------------------------

    public void setBallOutOfBounds(){
        if(!ballOutOfBoundsTimeOut){
            Debug.Log("Out of Bounds");
            outBoundsAgent = getPlayerTakingsKick(lastPlayerTouchingTheBall);
            ballOutOfBoundsMechanism(outBoundsAgent);
            lastPlayerTouchingTheBall = null;
        }
        else{
            Debug.Log("Timeout for ball out of bounds");
            outBoundsAgent = getPlayerTakingsKick(lastPlayerTouchingTheBall);
            ballOutOfBoundsMechanism(outBoundsAgent);
            setOutOfBounds(true);
        }
    }

    public AgentCore playerRecieveingBall(AgentCore agent){
        AgentCore possibleAgent;

        if(agent.team != AgentCore.Team.BLUE){
            possibleAgent = redTeamAgents.OrderBy(x => x.distanceToBall()).ToList()[1];
            Vector3 newPos = new Vector3();

            if(possibleAgent.type != AgentCore.Type.GOALKEEPER){
                newPos = calculatePlayerWhoRecievesTheBallNewPos(possibleAgent);
            }
            else{
                possibleAgent = redTeamAgents.OrderBy(x => x.distanceToBall()).ToList()[2];
                newPos = calculatePlayerWhoRecievesTheBallNewPos(possibleAgent);
            }

            Debug.Log("Nr of players: "+redTeamAgents.Count);

            possibleAgent.transform.localPosition = newPos;
            Debug.Log("Ball x: " + Ball.transform.position.x);
            Debug.Log("Ball z: " + Ball.transform.position.z);
            possibleAgent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - possibleAgent.transform.localPosition));
        }
        else{
            possibleAgent = blueTeamAgents.OrderBy(x => x.distanceToBall()).ToList()[1];
            Vector3 newPos = new Vector3();

            if(possibleAgent.type != AgentCore.Type.GOALKEEPER){
                newPos = calculatePlayerWhoRecievesTheBallNewPos(possibleAgent);
            }
            else{
                possibleAgent = blueTeamAgents.OrderBy(x => x.distanceToBall()).ToList()[2];
                newPos = calculatePlayerWhoRecievesTheBallNewPos(possibleAgent);
            }

            Debug.Log("Nr of players: "+blueTeamAgents.Count);

            possibleAgent.transform.localPosition = newPos;
            Debug.Log("Ball x: " + Ball.transform.position.x);
            Debug.Log("Ball z: " + Ball.transform.position.z);
            possibleAgent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - possibleAgent.transform.localPosition));
        }

        return possibleAgent;
    }

    public Vector3 calculatePlayerWhoRecievesTheBallNewPos(AgentCore agent){

            float xBall = Ball.transform.localPosition.x;
            float zBall = Ball.transform.localPosition.z;
            float xAgent = agent.transform.localPosition.x;
            float zAgent = agent.transform.localPosition.z;
            float t = 4.5f/agent.distanceToBall();
            float newX = ((1 - t)*xBall + t*xAgent);
            float newZ = ((1 - t)*zBall + t*zAgent);

            return new Vector3(newX, agent.transform.localPosition.y, newZ);
    }

    //Responsible for spawning player and ball at the right positions after BallOutOfBounds
    private void ballOutOfBoundsMechanism(AgentCore agent){
        float x = Ball.transform.localPosition.x;
        float y = Ball.transform.localPosition.y;
        float z = Ball.transform.localPosition.z;

        bool notCorner = false;


        if(x < 0){
            if(z > 0){
                //Top Left Half Field Bounds
                if(z > 7.5){
                    Ball.transform.position = new Vector3(x, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.2327683f, 8.65f, agent, 0);
                }
                //Top Left Corner
                else{
                    Ball.transform.position = new Vector3(-14f, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                    if(agent.team == AgentCore.Team.RED){
                        outOfBounds = false;
                        setFreeGoalAreaKickPositions(AgentCore.Team.RED);
                        notCorner = true;    
                    }
                    else
                        spawnWheelchairAtNewSpot(-15.15f, 0.2327683f, 8.65f, agent, -45);
                }
            }
            else{
                //Bottom Left Corner
                if(z > -7.5){
                    Ball.transform.position = new Vector3(-14f, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                    if(agent.team == AgentCore.Team.RED){
                        outOfBounds = false;
                        setFreeGoalAreaKickPositions(AgentCore.Team.RED);
                        notCorner = true;    
                    }
                    else
                        spawnWheelchairAtNewSpot(-15.15f, 0.2327683f, -8.65f, agent, 225);
                }
                //Bottom Left Half Field Bounds
                else{
                    Ball.transform.position = new Vector3(x, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.2327683f, -8.65f, agent, 180);
                }
            }
        }else{
            if(z > 0){
                //Top Right Half Field Bounds
                if(z > 7.5){
                    Ball.transform.position = new Vector3(x, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.2327683f, 8.65f, agent, 0);
                }
                //Top Right Corner
                else{
                    Ball.transform.position = new Vector3(14f, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                    if(agent.team == AgentCore.Team.BLUE){
                        outOfBounds = false;
                        setFreeGoalAreaKickPositions(AgentCore.Team.BLUE);
                        notCorner = true;    
                    }
                    else
                        spawnWheelchairAtNewSpot(15.15f, 0.2327683f, 8.65f, agent, 45);
                }
            }
            else{
                //Bottom Right Corner
                if(z > -7.5){
                    Ball.transform.position = new Vector3(14f, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                    if(agent.team == AgentCore.Team.BLUE){
                        outOfBounds = false;
                        setFreeGoalAreaKickPositions(AgentCore.Team.BLUE);
                        notCorner = true;
                    }
                    else
                        spawnWheelchairAtNewSpot(15.15f, 0.2327683f, -8.65f, agent, 135);
                }
                //Bottom Right Half Field Bounds
                else{
                    Ball.transform.position = new Vector3(x, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.2327683f, -8.65f, agent, 180);
                }
            }
        }

        if(!notCorner){
            AgentCore a = playerRecieveingBall(agent);
            setOutOfBounds(true);
            placeTeamByAreaOutOfBounds(agent, a, agent.team);
        }
        else{
            outOfBoundsAreaFreeKick = true;
        }
    }

    public void positionPlayer(AgentCore agent, float x, float z, float rotation){
        agent.transform.localPosition = new Vector3(x, agent.transform.localPosition.y, z);
        agent.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    public void placeTeamByAreaOutOfBounds(AgentCore playerTakingTheKick, AgentCore playerRecieveingBall, AgentCore.Team teamTakingKick){
        
        //BLUE ATTACKING
        if(teamTakingKick == AgentCore.Team.BLUE){
            List<AgentCore> remainBlueTeamAgents = new List<AgentCore>(blueTeamAgents.Count);

            foreach(AgentCore agent in blueTeamAgents){
                remainBlueTeamAgents.Add(agent);
            }

            remainBlueTeamAgents.Remove(playerTakingTheKick);
            remainBlueTeamAgents.Remove(playerRecieveingBall);




            //ZONE 1
            if(Ball.transform.localPosition.x >= -14 && Ball.transform.localPosition.x < -7){
                if(Ball.transform.localPosition.z > 0){

                    positionPlayer(redTeamAgents[0], -7.5f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -13f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -11.5f, 3f, -90);
                    positionPlayer(redTeamAgents[3], -6f, 0f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -5f, -3f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -5f, -3f, 90);
                    }

                }
                else{

                    positionPlayer(redTeamAgents[0], -7.5f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -13f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -11.5f, -3f, -90);
                    positionPlayer(redTeamAgents[3], -6f, 0f, -90);

                    if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -5f, 3f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -5f, 3f, 90);
                    }

                }
            }
            //ZONE 2
            else if(Ball.transform.localPosition.x >= -7 && Ball.transform.localPosition.x < 0){
                if(Ball.transform.localPosition.z > 0){

                    positionPlayer(redTeamAgents[0], -4.5f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -11.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -9f, 4f, -90);
                    positionPlayer(redTeamAgents[3], -7f, 0f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 1f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], 1f, 3f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                    }

                }
                else{

                    positionPlayer(redTeamAgents[0], -4.5f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -11.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -9f, -4f, -90);
                    positionPlayer(redTeamAgents[3], -7f, 0f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 1f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[1], -2f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], 1f, -3f, 90);
                        positionPlayer(remainBlueTeamAgents[0], -2f, 0f, 90);
                    }
                }

            }
            //ZONE 3
            else if(Ball.transform.localPosition.x >= 0 && Ball.transform.localPosition.x < 7){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(redTeamAgents[0], 0f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -10.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -5.5f, 4f, -90);
                    positionPlayer(redTeamAgents[3], -3f, -0.5f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 9f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 4f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], 9f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 4f, 0f, 90);
                    }
                }
                else{
                    positionPlayer(redTeamAgents[0], 0f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -10.5f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -5.5f, -4f, -90);
                    positionPlayer(redTeamAgents[3], -3f, -0.5f, -90);

                    if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 9f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 4f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], 9f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 4f, 0f, 90);
                    }
                }
            }
            //ZONE 4
            else if(Ball.transform.localPosition.x >= 7 && Ball.transform.localPosition.x <= 14){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(redTeamAgents[0], 4f, 2.5f, -90);
                    positionPlayer(redTeamAgents[1], -6f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -1.5f, 4f, -90);
                    positionPlayer(redTeamAgents[3], 1f, -0.5f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 12f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 7.5f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], 12f, 2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 7.5f, 0f, 90);                    
                        }
                }
                else{
                    positionPlayer(redTeamAgents[0], 4f, -2.5f, -90);
                    positionPlayer(redTeamAgents[1], -6f, 0f, -90);
                    positionPlayer(redTeamAgents[2], -1.5f, -4f, -90);
                    positionPlayer(redTeamAgents[3], 1f, 0.5f, -90);

                     if(remainBlueTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainBlueTeamAgents[0], 12f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[1], 7.5f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainBlueTeamAgents[1], 12f, -2f, 90);
                        positionPlayer(remainBlueTeamAgents[0], 7.5f, 0f, 90);                    
                        }
                }
            }
        }
        else{
            List<AgentCore> remainRedTeamAgents = new List<AgentCore>(redTeamAgents.Count);

            foreach(AgentCore agent in redTeamAgents){
                remainRedTeamAgents.Add(agent);
            }

            remainRedTeamAgents.Remove(playerTakingTheKick);
            remainRedTeamAgents.Remove(playerRecieveingBall);

            //ZONE 1
            if(Ball.transform.localPosition.x >= -14 && Ball.transform.localPosition.x < -7){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(blueTeamAgents[0], -4f, 2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 6f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 1.5f, 4f, 90);
                    positionPlayer(blueTeamAgents[3], -1f, -0.5f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -12f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -7.5f, 0f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], -12f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -7.5f, 0f, -90);                    
                        }
                }
                else{
                    positionPlayer(blueTeamAgents[0], -4f, -2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 6f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 1.5f, -4f, 90);
                    positionPlayer(blueTeamAgents[3], -1f, 0.5f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -12f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -7.5f, 0f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], -12f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -7.5f, 0f, -90);                    
                        }

                }
            }
            //ZONE 2
            else if(Ball.transform.localPosition.x >= -7 && Ball.transform.localPosition.x < 0){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(blueTeamAgents[0], 0f, 2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 5.5f, 4f, 90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], -9f, 2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, -90);
                    }
                }
                else{
                    positionPlayer(blueTeamAgents[0], 0f, -2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 5.5f, -4f, 90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, 90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], -9f, -2f, -90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, -90);
                    }
                }

            }
            //ZONE 3
            else if(Ball.transform.localPosition.x >= 0 && Ball.transform.localPosition.x < 7){
                if(Ball.transform.localPosition.z > 0){
                    positionPlayer(blueTeamAgents[0], 0f, 2.5f, -90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, -90);
                    positionPlayer(blueTeamAgents[2], 5.5f, 4f, -90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, -90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, 2f, 90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], -9f, 2f, 90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, 90);
                    }
                }
                else{
                    positionPlayer(blueTeamAgents[0], 0f, -2.5f, -90);
                    positionPlayer(blueTeamAgents[1], 10.5f, 0f, -90);
                    positionPlayer(blueTeamAgents[2], 5.5f, -4f, -90);
                    positionPlayer(blueTeamAgents[3], 3f, -0.5f, -90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], -9f, -2f, 90);
                        positionPlayer(remainRedTeamAgents[1], -4f, 0f, 90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], -9f, -2f, 90);
                        positionPlayer(remainRedTeamAgents[0], -4f, 0f, 90);
                    }
                }
            }
            //ZONE 4
            else if(Ball.transform.localPosition.x >= 7 && Ball.transform.localPosition.x <= 14){
                if(Ball.transform.localPosition.z > 0){

                    positionPlayer(blueTeamAgents[0], 7.5f, 2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 13f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 11.5f, 3f, 90);
                    positionPlayer(blueTeamAgents[3], 6f, 0f, 90);

                     if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[1], 5f, -3f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], 5f, -3f, -90);
                    }

                }
                else{

                    positionPlayer(blueTeamAgents[0], 7.5f, -2.5f, 90);
                    positionPlayer(blueTeamAgents[1], 13f, 0f, 90);
                    positionPlayer(blueTeamAgents[2], 11.5f, -3f, 90);
                    positionPlayer(blueTeamAgents[3], 6f, 0f, 90);

                    if(remainRedTeamAgents[0].type == AgentCore.Type.GOALKEEPER){
                        positionPlayer(remainRedTeamAgents[0], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[1], 5f, 3f, -90);
                    }
                    else{
                        positionPlayer(remainRedTeamAgents[1], 2f, 0f, -90);
                        positionPlayer(remainRedTeamAgents[0], 5f, 3f, -90);
                    }

                }
            }
        }
    }

    public void setOutOfBounds(bool b){
        outOfBounds = b;
    }

    public void setfoulCommited(bool b){
        foulCommited = b;
    }

    //Limits the area which the agent/player can walk when in a free/indirect/outofbounds kick
    private void limitWalkingArea(AgentCore agent, Vector3 pos, float rot){
        Vector3 centerPosition = outBoundsBallPos; //center of circle
        float distance = Vector3.Distance(agent.transform.localPosition, centerPosition);
        
        if (distance > 3)
        {
            spawnWheelchairAtNewSpot(pos.x, pos.y, pos.z, agent, rot);
        }
    }

    //Spawns the player and Constrains the radius bounds of the indirect kick where player can move before shoot/pass the ball
    private void spawnWheelchairAtNewSpot(float x, float y, float z, AgentCore agent, float rotation){ 
        agent.transform.localPosition = new Vector3(x, y, z);
        agent.stopChair();

        outBoundsBallPos = Ball.transform.localPosition;
        outBoundsAgentPos = new Vector3(x, y, z);
        outBoundsAgentRot = rotation;

        agent.transform.rotation = Quaternion.LookRotation(-(Ball.transform.localPosition - agent.transform.localPosition));

    }

    public AgentCore getPlayerTakingsKick(AgentCore player){
        Debug.Log(player.name);

        if(player.team == AgentCore.Team.RED){
            return blueTeamAgents.OrderBy(x => x.distanceToBall()).ToList()[0];
        }
        else{
            return redTeamAgents.OrderBy(x => x.distanceToBall()).ToList()[0];
        }
    }
    
}