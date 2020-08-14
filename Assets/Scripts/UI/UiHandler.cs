using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    public GameEnvironmentInfo gameEnvironment;
    public Camera mainMenuCamera;

    //PLAY MENU VARS
    private bool playCameraRotationBool;
    public GameObject quitBox;
    public GameObject settingsBox;
    public GameObject playBox;
    public GameObject blueTeamBox;
    public GameObject redTeamBox;
    public GameObject backBox;


    //BACK PLAY MENU VARS
    private bool backPlayCameraRotationBool;

    // Start is called before the first frame update
    void Start()
    {
        playCameraRotationBool = false;
        backPlayCameraRotationBool = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(getCameraBool())
            handleCameraRotation();

        if(getCameraBackPlayBool())
            handleBackPlayCameraRotation();
    }

    public void handleCameraRotation(){
        //Debug.Log(mainMenuCamera.transform.rotation.eulerAngles.y);
        if(mainMenuCamera.transform.rotation.eulerAngles.y > 55){
            setCameraBool(false);
        } 
        else{
            mainMenuCamera.transform.Rotate( Vector3.up * ( 12 * Time.deltaTime ));

            playBox.transform.position = new Vector3(playBox.transform.position.x - 310 * Time.deltaTime, playBox.transform.position.y, playBox.transform.position.z);
            settingsBox.transform.position = new Vector3(settingsBox.transform.position.x - 210 * Time.deltaTime, settingsBox.transform.position.y, settingsBox.transform.position.z);
            quitBox.transform.position = new Vector3(quitBox.transform.position.x - 160 * Time.deltaTime, quitBox.transform.position.y, quitBox.transform.position.z);

            blueTeamBox.transform.position = new Vector3(blueTeamBox.transform.position.x - 210 * Time.deltaTime, blueTeamBox.transform.position.y, blueTeamBox.transform.position.z);
            redTeamBox.transform.position = new Vector3(redTeamBox.transform.position.x - 210 * Time.deltaTime, redTeamBox.transform.position.y, redTeamBox.transform.position.z);
            backBox.transform.position = new Vector3(backBox.transform.position.x - 210 * Time.deltaTime, backBox.transform.position.y, backBox.transform.position.z);
        }
    }

    public void handleBackPlayCameraRotation(){
        //Debug.Log(mainMenuCamera.transform.rotation.eulerAngles.y);
        if(mainMenuCamera.transform.rotation.eulerAngles.y < 1){
            setCameraBackPlayBool(false);
        } 
        else{
            mainMenuCamera.transform.Rotate( Vector3.up * ( -12 * Time.deltaTime ));

            playBox.transform.position = new Vector3(playBox.transform.position.x + 310 * Time.deltaTime, playBox.transform.position.y, playBox.transform.position.z);
            settingsBox.transform.position = new Vector3(settingsBox.transform.position.x + 210 * Time.deltaTime, settingsBox.transform.position.y, settingsBox.transform.position.z);
            quitBox.transform.position = new Vector3(quitBox.transform.position.x + 160 * Time.deltaTime, quitBox.transform.position.y, quitBox.transform.position.z);

            blueTeamBox.transform.position = new Vector3(blueTeamBox.transform.position.x + 210 * Time.deltaTime, blueTeamBox.transform.position.y, blueTeamBox.transform.position.z);
            redTeamBox.transform.position = new Vector3(redTeamBox.transform.position.x + 210 * Time.deltaTime, redTeamBox.transform.position.y, redTeamBox.transform.position.z);
            backBox.transform.position = new Vector3(backBox.transform.position.x + 210 * Time.deltaTime, backBox.transform.position.y, backBox.transform.position.z);
        }
    }

    public void prepareGame(AgentCore.Team team){
        if(team == AgentCore.Team.RED)
            GameEnvironmentInfo.choosenTeam = true;
        else
            GameEnvironmentInfo.choosenTeam = false;
            
        SceneManager.LoadScene(1);
    }

    public void setCameraBool(bool x){
        playCameraRotationBool = x;
    }

    public void setCameraBackPlayBool(bool x){
        backPlayCameraRotationBool = x;
    }

    public bool getCameraBackPlayBool(){
        return backPlayCameraRotationBool;
    }

    public bool getCameraBool(){
        return playCameraRotationBool;
    }
}
