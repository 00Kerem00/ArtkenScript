using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public bool destroyable = true;

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.tag == "Damaging" && destroyable) 
        {
            if (Event != null)
                StaticCoroutine.DoCoroutine(Event);
            Destroy(gameObject);
        }
    }

    public IEnumerator Event;
}
