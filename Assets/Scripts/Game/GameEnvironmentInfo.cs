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

        public GameObject Ball;

        //While this is true, players cannot commit faults bacause they are in a indirect free kick
        private bool faultTimeOut;
        //While this is true, players cannot commit Ball Out of Bounds because they are in a indirect free kick
        private bool ballOutOfBoundsTimeOut;


    //AGENTS GENERAL INFO
        //Player who has the ball in its possession
        private AgentCore playerWithBall;
        //Last player touchinh the ball
        private AgentCore lastPlayerTouchingTheBall;
        //Players who are confronting the who has the ball in its possession (playerWithBall)
        private AgentCore opponent;


    //NUMBER OF PLAYERS AT EACH AREA OF THE FIELD
        private List<AgentCore> playersAtHalfSideAreaBlue;
        private List<AgentCore> playersAtHalfSideAreaRed;
        private List<AgentCore> playersAtSmallAreaBlue;
        private List<AgentCore> playersAtSmallAreaRed;
        private List<AgentCore> playersAtOutsideArea;

    //BALL OUT OF BOUNDS INFO
        private bool outOfBounds;
        private Vector3 ballPos;
        private Vector3 outBoundsAgentPos;
        private AgentCore outBoundsAgent;
        private float outBoundRot;



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
    }

    // Update is called once per frame
    void Update()
    {
        setPlayerWithBallAndOpponent();
        /*if(lastPlayerTouchingTheBall != null)
            //Debug.Log("Last Player touching the ball: " + lastPlayerTouchingTheBall.name);
        if(playerWithBall != null)
            Debug.Log("Player with ball possession: " + playerWithBall.name);
        if(opponent != null)
            Debug.Log("Opponent: " + opponent.name);*/
        if(outOfBounds)
            limitWalkingArea();
  
    }

    // Sets the player that has the ball and the opponent; sets null e there is none respectively;
    // It also checks if anyone commited the two-on-one fault
    // Opponent is the player who is in a radius of 3m of the player with the ball
    public void setPlayerWithBallAndOpponent(){

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

                        //Since there's an opponnent already, we need to check if theres anyone commiting the two-on-one fault
                        for(int j = i+1; j < possibleAgentsNearBall.Count; j++){
                            if(possibleAgentsNearBall[j].distanceToPlayer(playerWithBall) < 3){
                                if(possibleAgentsNearBall[j].team != playerWithBall.team){
                                    //Found a player commiting a fault
                                    possibleAgentsNearBall[j].setFault();
                                    // Debug.Log(possibleAgentsNearBall[j].name + " commited Two-on-One Fault!");
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
        playersAtHalfSideAreaBlue.Add(agent);
    }

    public void setPlayersAtHalfSideAreaRed(AgentCore agent){
        playersAtHalfSideAreaRed.Add(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);
    }

    public void setPlayersAtSmallAreaBlue(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Add(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);
    }

    public void setPlayersAtSmallAreaRed(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Add(agent);
        playersAtOutsideArea.Remove(agent);
        playersAtHalfSideAreaBlue.Remove(agent);
    }

    public void setPlayersAtOutsideArea(AgentCore agent){
        playersAtHalfSideAreaRed.Remove(agent);
        playersAtSmallAreaBlue.Remove(agent);
        playersAtSmallAreaRed.Remove(agent);
        playersAtOutsideArea.Add(agent);
        playersAtHalfSideAreaBlue.Remove(agent);
    }

    public void setGoalAtRedGoal(){
        blueScore += 1;
        Debug.Log("Score: Blue - " + blueScore);
        Debug.Log("Score: Red - " + redScore);
    }

    public void setGoalAtBlueGoal(){
        redScore += 1;
        Debug.Log("Score: Blue - " + blueScore);
        Debug.Log("Score: Red - " + redScore);
    }

    public void setBallOutOfBounds(){
        if(!ballOutOfBoundsTimeOut){
            Debug.Log("Out of Bounds");
            setPlayerTakingTheOutBoundsKick();
            ballOutOfBoundsMechanism(outBoundsAgent);
            setOutOfBounds(true);
        }
        else{
            Debug.Log("Timeout for ball out of bounds");
        }
    }

    public void setOutOfBounds(bool b){
        outOfBounds = b;
    }

    //Limits the area which the agent/player can walk when in a free/indirect kick
    private void limitWalkingArea(){
        Vector3 centerPosition = ballPos; //center of circle
        float distance = Vector3.Distance(outBoundsAgent.transform.localPosition, centerPosition);
        
        if (distance > 3)
        {
            spawnWheelchairAtNewSpot(outBoundsAgentPos.x, outBoundsAgentPos.y, outBoundsAgentPos.z, outBoundsAgent, outBoundRot);
        }
    }



    //Spawns the player and Constrains the radius bounds of the indirect kick where player can move before shoot/pass the ball
    private void spawnWheelchairAtNewSpot(float x, float y, float z, AgentCore agent, float rotation){        

        //Spawn player/agent
        agent.stopChair(rotation);

        agent.getAgentRBody().transform.rotation = Quaternion.identity * Quaternion.Euler(0, rotation, 0);
        agent.getAgentRBody().transform.localPosition = new Vector3(x, y, z);

        ballPos = Ball.transform.localPosition;
        outBoundsAgentPos = new Vector3(x, y, z);
        outBoundRot = rotation;
    }

    public void setPlayerTakingTheOutBoundsKick(){
        Debug.Log(lastPlayerTouchingTheBall.name);
        var redAgents = new List<AgentCore>(redTeamAgents.Count);
        var blueAgents = new List<AgentCore>(blueTeamAgents.Count);

        redAgents.AddRange(redTeamAgents);
        blueAgents.AddRange(blueTeamAgents);
        
        redAgents = redAgents.OrderBy(x => x.distanceToBall()).ToList();
        blueAgents = blueAgents.OrderBy(x => x.distanceToBall()).ToList();

        if(lastPlayerTouchingTheBall.team == AgentCore.Team.RED){
            outBoundsAgent = blueAgents[0];
        }
        else{
            outBoundsAgent = redAgents[0];
        }
    }

    //Responsible for spawning player and ball at the right positions after BallOutOfBounds
    private void ballOutOfBoundsMechanism(AgentCore agent){
        float x = Ball.transform.localPosition.x;
        float y = Ball.transform.localPosition.y;
        float z = Ball.transform.localPosition.z;


        if(x < 0){
            if(z > 0){
                //Top Left Half Field Bounds
                if(z > 7.5){
                    Ball.transform.position = new Vector3(x, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.25f, 8.6f, agent, 0);
                }
                //Top Left Corner
                else{
                    Ball.transform.position = new Vector3(-14f, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(-15.1f, 0.25f, 8.6f, agent, -45);
                }
            }
            else{
                //Bottom Left Corner
                if(z > -7.5){
                    Ball.transform.position = new Vector3(-14f, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(-15.1f, 0.25f, -8.6f, agent, 225);
                }
                //Bottom Left Half Field Bounds
                else{
                    Ball.transform.position = new Vector3(x, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.25f, -8.6f, agent, 180);
                }
            }
        }else{
            if(z > 0){
                //Top Right Half Field Bounds
                if(z > 7.5){
                    Ball.transform.position = new Vector3(x, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.25f, 8.6f, agent, 0);
                }
                //Top Right Corner
                else{
                    Ball.transform.position = new Vector3(14f, 0.44f, 7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(15.1f, 0.25f, 8.6f, agent, 45);
                }
            }
            else{
                //Bottom Right Corner
                if(z > -7.5){
                    Ball.transform.position = new Vector3(14f, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(15.1f, 0.25f, -8.6f, agent, 135);
                }
                //Bottom Right Half Field Bounds
                else{
                    Ball.transform.position = new Vector3(x, 0.44f, -7.5f);
                    Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    Ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                    spawnWheelchairAtNewSpot(x, 0.25f, -8.6f, agent, 180);
                }
            }
        }

    }
}
