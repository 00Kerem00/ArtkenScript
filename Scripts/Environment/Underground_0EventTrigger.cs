using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Underground_0EventTrigger : MonoBehaviour
{
    public Underground_0Events events;
    public int number;
    public IEnumerator enterEvent, exitEvent;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo") 
        {
            events.StartUnderground_0Event(number);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Leo")
        {
            events.StartUnderground_0EventRewind(number);
        }
    }
}
