using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public enum Areas{
        smallRedArea,
        smallBlueArea,
        halfFieldBlue,
        halfFieldRed,
    }

    public Areas positionInField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setPositionInField(Areas area){
        positionInField = area;
    }

    public void stopIt(){
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
