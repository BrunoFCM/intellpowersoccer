using UnityEngine;
using Unity.MLAgents.Sensors;

public class BehaviourHandler : MonoBehaviour
{
    public GameObject strikeTheBallTrainer;
    public GameObject passTheBallTrainer;
    public GameObject goalKeepTrainer;
    public GameObject moveToPointTrainer;
    public GameObject dribbleBallTrainer;
    public GameObject intersectBallTrainer;
    public GameEnvironmentInfo gameEnvironmentInfo;
    public AgentCore agentCore;
    // Start is called before the first frame update
    void Start()
    {
        disableAllBehaviours();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPassTheBallBehaviour(){
        if(GameEnvironmentInfo.choosenTeam){
            if(agentCore.tag == "Agent1")
                return;
        }
        else{
            if(agentCore.tag == "Agent5")
                return;
        }

        if(!passTheBallTrainer.activeSelf){
            disableAllBehaviours();
            passTheBallTrainer.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].DetectableTags[1] = gameEnvironmentInfo.getNearestTeamMate(agentCore).tag;
            passTheBallTrainer.SetActive(true);
        }
    }

    public void setStrikeTheBallBehaviour(){
        if(GameEnvironmentInfo.choosenTeam){
            if(agentCore.tag == "Agent1")
                return;
        }
        else{
            if(agentCore.tag == "Agent5")
                return;
        }

        if(!strikeTheBallTrainer.activeSelf){
            disableAllBehaviours();
            strikeTheBallTrainer.SetActive(true);
        }
    }

    public void setGoalKeepBehaviour(AgentCore shooter){
        if(GameEnvironmentInfo.choosenTeam){
            if(agentCore.tag == "Agent1")
                return;
        }
        else{
            if(agentCore.tag == "Agent5")
                return;
        }

        if(!goalKeepTrainer.activeSelf){
            disableAllBehaviours();
            goalKeepTrainer.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].DetectableTags[1] = gameEnvironmentInfo.getNearestTeamMate(shooter).tag;
            goalKeepTrainer.SetActive(true);
        }
    }

    public void setDribbleBallBehaviour(){
        if(GameEnvironmentInfo.choosenTeam){
            if(agentCore.tag == "Agent1")
                return;
        }
        else{
            if(agentCore.tag == "Agent5")
                return;
        }

        if(!dribbleBallTrainer.activeSelf){
            disableAllBehaviours();
            /*gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].DetectableTags[1] = PointTag;
            gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[1].DetectableTags[1] = PointTag; */
            dribbleBallTrainer.SetActive(true);
        }
    }

    public void setMoveToPointBehaviour(){
        if(GameEnvironmentInfo.choosenTeam){
            if(agentCore.tag == "Agent1")
                return;
        }
        else{
            if(agentCore.tag == "Agent5")
                return;
        }

        if(!moveToPointTrainer.activeSelf){
            disableAllBehaviours();
            /*gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].DetectableTags[0] = PointTag;
            gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[1].DetectableTags[0] = PointTag;*/
            moveToPointTrainer.SetActive(true);
        }
    }

    public void setIntersectBallBehaviour(AgentCore agent, AgentCore nearestPlayer){
        if(GameEnvironmentInfo.choosenTeam){
            if(agentCore.tag == "Agent1")
                return;
        }
        else{
            if(agentCore.tag == "Agent5")
                return;
        }
        
        if(!intersectBallTrainer.activeSelf){
            disableAllBehaviours();
            intersectBallTrainer.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].DetectableTags[1] = agent.tag;
            intersectBallTrainer.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].DetectableTags[2] = nearestPlayer.tag;
            intersectBallTrainer.SetActive(true);
        }
    }

    public void disableAllBehaviours(){
        strikeTheBallTrainer.SetActive(false);
        goalKeepTrainer.SetActive(false);
        passTheBallTrainer.SetActive(false); 
        moveToPointTrainer.SetActive(false);
        dribbleBallTrainer.SetActive(false);
        intersectBallTrainer.SetActive(false); 
    }

    public void disableAllAgentBehaviours(){
        foreach(AgentCore agent in gameEnvironmentInfo.redTeamAgents){
            agent.disableAllBehaviours();
        }

        foreach(AgentCore agent in gameEnvironmentInfo.blueTeamAgents){
            agent.disableAllBehaviours();
        }
    }
}
