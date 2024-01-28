using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _2DTriggerBox : MonoBehaviour
{
    public string targetObject = "";
    public IEnumerator enterEvent;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (targetObject != "" && collider.gameObject.name == targetObject)
            StartCoroutine(enterEvent);
    } 
}
