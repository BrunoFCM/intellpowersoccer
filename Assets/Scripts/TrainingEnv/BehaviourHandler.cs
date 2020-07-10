using UnityEngine;
using MLAgents.Sensor;

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
        disableAllBehaviours();
        gameObject.GetComponentInChildren<RayPerceptionSensorComponentBase>().detectableTags[1] = gameEnvironmentInfo.getNearestTeamMate(agentCore).tag;
        passTheBallTrainer.SetActive(true);
    }

    public void setStrikeTheBallBehaviour(){
        disableAllBehaviours();
        strikeTheBallTrainer.SetActive(true);
    }

    public void setGoalKeepBehaviour(AgentCore shooter){
        disableAllBehaviours();
        gameObject.GetComponentInChildren<RayPerceptionSensorComponentBase>().detectableTags[1] = gameEnvironmentInfo.getNearestTeamMate(shooter).tag;
        goalKeepTrainer.SetActive(true);
    }

    public void setDribbleBallBehaviour(string PointTag){
        disableAllBehaviours();
        gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].detectableTags[1] = PointTag;
        gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[1].detectableTags[1] = PointTag; 
        dribbleBallTrainer.SetActive(true);
    }

    public void setMoveToPointBehaviour(string PointTag){
        disableAllBehaviours();
        gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[0].detectableTags[0] = PointTag;
        gameObject.GetComponentsInChildren<RayPerceptionSensorComponentBase>()[1].detectableTags[0] = PointTag;
        moveToPointTrainer.SetActive(true);
    }

    public void setIntersectBallBehaviour(){
        disableAllBehaviours();
        gameObject.GetComponentInChildren<RayPerceptionSensorComponentBase>().detectableTags[1] = gameEnvironmentInfo.getNearestPlayerToBall().tag;
        gameObject.GetComponentInChildren<RayPerceptionSensorComponentBase>().detectableTags[2] = gameEnvironmentInfo.getNearestTeamMate(gameEnvironmentInfo.getNearestPlayerToBall()).tag;
        intersectBallTrainer.SetActive(true);
    }

    public void disableAllBehaviours(){
        strikeTheBallTrainer.SetActive(false);
        goalKeepTrainer.SetActive(false);
        passTheBallTrainer.SetActive(false); 
        moveToPointTrainer.SetActive(false);
        dribbleBallTrainer.SetActive(false);
        intersectBallTrainer.SetActive(false); 
    }
}
