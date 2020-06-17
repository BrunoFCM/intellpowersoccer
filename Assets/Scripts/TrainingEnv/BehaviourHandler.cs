using UnityEngine;
using MLAgents.Sensor;

public class BehaviourHandler : MonoBehaviour
{
    public GameObject strikeTheBallTrainer;
    public GameObject passTheBallTrainer;
    public GameObject goalKeepTrainer;
    public GameEnvironmentInfo gameEnvironmentInfo;
    public AgentCore agentCore;
    // Start is called before the first frame update
    void Start()
    {
        strikeTheBallTrainer.SetActive(false);
        goalKeepTrainer.SetActive(false);
        passTheBallTrainer.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPassTheBallBehaviour(){
        gameObject.GetComponentInChildren<RayPerceptionSensorComponentBase>().detectableTags[1] = gameEnvironmentInfo.getNearestTeamMate(agentCore).tag;
        strikeTheBallTrainer.SetActive(false);
        goalKeepTrainer.SetActive(false);
        passTheBallTrainer.SetActive(true);
    }

    public void setStrikeTheBallBehaviour(){
        strikeTheBallTrainer.SetActive(true);
        goalKeepTrainer.SetActive(false);
        passTheBallTrainer.SetActive(false);
    }

    public void setGoalKeepBehaviour(AgentCore shooter){
        gameObject.GetComponentInChildren<RayPerceptionSensorComponentBase>().detectableTags[1] = gameEnvironmentInfo.getNearestTeamMate(shooter).tag;
        strikeTheBallTrainer.SetActive(false);
        goalKeepTrainer.SetActive(true);
        passTheBallTrainer.SetActive(false);
    }

    public void disableAllBehaviours(){
        strikeTheBallTrainer.SetActive(false);
        goalKeepTrainer.SetActive(false);
        passTheBallTrainer.SetActive(false);  
    }
}
