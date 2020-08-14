using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;
    private Vector3 rectPos;
    public UiHandler uiHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        rectPos = rect.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(!uiHandler.getCameraBool()){
            Debug.Log("Mouse is over GameObject.");
            if(this.tag != "bluesTeam" && this.tag != "redsTeam" && this.tag != "backPlay"){
                rect.transform.position = new Vector3(rect.transform.position.x+200, rect.transform.position.y, rect.transform.position.z);
            }
            else{
                rect.transform.position = new Vector3(rect.transform.position.x-200, rect.transform.position.y, rect.transform.position.z);
            }
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!uiHandler.getCameraBool()){
            Debug.Log("Mouse is no longer on GameObject.");
            if(this.tag != "bluesTeam" && this.tag != "redsTeam" && this.tag != "backPlay"){
                rect.transform.position = new Vector3(rect.transform.position.x-200, rect.transform.position.y, rect.transform.position.z);
            }
            else{
                rect.transform.position = new Vector3(rect.transform.position.x+200, rect.transform.position.y, rect.transform.position.z);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Game Object was Pressed");
        if(this.tag == "play"){
            rect.transform.position = rectPos;
            playMenu();
        }
        else if(this.tag == "settings"){

        }
        else if(this.tag == "exit"){
            Application.Quit();
        }
        else if(this.tag == "backPlay"){
            backPlayMenu();
        }
        else if(this.tag == "redsTeam"){
            uiHandler.prepareGame(AgentCore.Team.RED);
        }
        else if(this.tag == "bluesTeam"){
            uiHandler.prepareGame(AgentCore.Team.BLUE);
        }
    }

    public void playMenu(){
        uiHandler.setCameraBool(true);
    }

    public void backPlayMenu(){
        uiHandler.setCameraBackPlayBool(true);
    }
}
