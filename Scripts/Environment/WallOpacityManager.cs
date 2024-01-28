using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WallOpacityManager : MonoBehaviour
{
    public Transform leo;
    public SpriteRenderer[] sprites;
    public float effectDistance;
    private float center;
    private float alpha;
    public MoveDirection zeroToOne;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo")
            this.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo")
            this.enabled = false;
    }


    private void Start() 
    {
        center = GetComponent<Transform>().position.x;
    }

    private void Update() 
    {
        alpha = (leo.position.x - center) / (effectDistance / 2);
        foreach (SpriteRenderer sprite in sprites)
            sprite.color = new Color(1, 1, 1, alpha);
    }
}
