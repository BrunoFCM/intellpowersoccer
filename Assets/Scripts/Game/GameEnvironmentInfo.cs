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
    }

    // Update is called once per frame
    void Update()
    {
        setPlayerWithBallAndOpponent();
        //if(lastPlayerTouchingTheBall != null)
            //Debug.Log("Last Player touching the ball: " + lastPlayerTouchingTheBall.name);
        if(playerWithBall != null)
            Debug.Log("Player with ball possession: " + playerWithBall.name);
        if(opponent != null)
            Debug.Log("Opponent: " + opponent.name);
  
    }

    // sets the player that has the ball and the opponent; sets null e there is none respectively;
    // Opponent is the player who is in a radious of 3m of the player with the ball
    public void setPlayerWithBallAndOpponent(){
        var possibleAgentsNearBall = new List<AgentCore>(blueTeamAgents.Count + redTeamAgents.Count);

        possibleAgentsNearBall.AddRange(blueTeamAgents);
        possibleAgentsNearBall.AddRange(redTeamAgents);
        possibleAgentsNearBall = possibleAgentsNearBall.OrderBy(x => x.distanceToBall()).ToList();

/*
        Debug.Log("-------------------------");
        foreach(AgentCore agent in possibleAgentsNearBall){
            Debug.Log(agent.name + " distance: " +agent.distanceToBall());
        }
*/

        if(possibleAgentsNearBall[0].isNearBall()){
            playerWithBall = possibleAgentsNearBall[0];

            for(int i = 1; i < possibleAgentsNearBall.Count; i++){
                if(possibleAgentsNearBall[i].distanceToPlayer(playerWithBall) < 3){
                    if(possibleAgentsNearBall[i].team != possibleAgentsNearBall[0].team){
                        opponent = possibleAgentsNearBall[1];
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
}
