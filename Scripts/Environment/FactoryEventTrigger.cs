using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryEventTrigger : MonoBehaviour
{
    public FactoryEvents events;
    public int eventNumber;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo")
        {
            switch (eventNumber)
            {
                case 0: events.FactoryEvent_0(); break;
                case 1: events.FactoryEvent_1(); break;
                case 2: events.FactoryEvent_2(); break;
                case 3: events.FactoryEvent_3(); break;
                case 4: events.FactoryEvent_4(); Destroy(gameObject); break;
                case 5: events.FactoryEvent_5(); break;
                case 6: events.FactoryEvent_6(); Destroy(gameObject); break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo")
        {
            switch (eventNumber)
            {
                case 0: events.FactoryEvent_0Rewind(); break;
                case 2: events.FactoryEvent_2Rewind(); break;
                case 3: events.FactoryEvent_3Rewind(); break;
                case 5: events.FactoryEvent_3Rewind(); break; 
            }
        }
    }
}
