using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingBaloonTrigger : MonoBehaviour
{    
    public int textNumber;
    public TalkingBaloonManager talkingBaloonManager;
    public float talkingTime;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo") 
        {
            collider.gameObject.GetComponent<Leo>().talkingBaloon.StartTalking(talkingBaloonManager.textArray[textNumber], talkingTime);
            Destroy(gameObject);
        }
    }
}
