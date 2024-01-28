using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntagonistPorter : MonoBehaviour
{
    public Transform antagonistPortPoint;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        Debug.Log("Trigger Enter");
        if (collider.gameObject.tag == "Antagonist")
            collider.gameObject.GetComponent<Transform>().position = antagonistPortPoint.position;
    }
}
