﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Text score;
    public Text time;
    public GameEnvironmentInfo gameEnvironment;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score.text = gameEnvironment.redScore + " - " + gameEnvironment.blueScore;
        time.text = string.Format("{00}:{1:00}", (int)(2400f - gameEnvironment.gameTime) / 60, (int)(2400f - gameEnvironment.gameTime) % 60);
         
    }
}
