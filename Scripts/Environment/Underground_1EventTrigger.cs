using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underground_1EventTrigger : MonoBehaviour
{
    public Underground_1Events events;
    public int eventNumber;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Leo")
            events.StartUnderground_1Event(eventNumber);
    }
    private void OnTriggerExit2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo")
            events.StartUnderground_1RewindEvent(eventNumber);
    }
}
