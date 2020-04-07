using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAreaBlue : MonoBehaviour
{
    public AgentCore agentCore;
    public Collider Agent;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.name == Agent.name){
            agentCore.setPlayersAtSmallAreaBlue();
        }
    }
}
