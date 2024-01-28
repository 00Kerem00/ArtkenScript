using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneSideCollider : MonoBehaviour
{
    EdgeCollider2D collider, trigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Leo")
            collider.enabled = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Leo")
            collider.enabled = true;
    }
    private void Start()
    {
        EdgeCollider2D[] colliders = GetComponents<EdgeCollider2D>();
        collider = colliders[0];
        trigger = colliders[1];
    }
}
