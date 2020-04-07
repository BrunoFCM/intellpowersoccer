using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallAreaRed : MonoBehaviour
{
    public AgentCore agentCore;
    public Collider Agent;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerStay(Collider collision) {
        if (collision.name == Agent.name){
            agentCore.setPlayersAtSmallAreaRed();
        }
    }
}
