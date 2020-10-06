using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EscapeClickScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;
    private Vector3 rectPos;
    public GameEnvironmentInfo gameEnvironmentInfo;
    
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
            Debug.Log("Mouse is over GameObject.");
                rect.transform.position = new Vector3(rect.transform.position.x+200, rect.transform.position.y, rect.transform.position.z);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            Debug.Log("Mouse is no longer on GameObject.");
                rect.transform.position = new Vector3(rect.transform.position.x-200, rect.transform.position.y, rect.transform.position.z);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Game Object was Pressed");
        if(this.tag == "resume"){
            gameEnvironmentInfo.ResumeGame();
        }
        else if(this.tag == "restart"){
            gameEnvironmentInfo.resetGame();
            gameEnvironmentInfo.ResumeGame();
        }
        else if(this.tag == "exit"){
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        }
    }
}
