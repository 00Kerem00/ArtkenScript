using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpStairs : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public SpriteMask[] spriteMasks;

    void Start() { Debug.Log(spriteMasks[0].backSortingLayerID); }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        GetComponent<EdgeCollider2D>().enabled = false;
        foreach (SpriteRenderer sprite in sprites)
            sprite.sortingLayerName = "Default";
        foreach (SpriteMask mask in spriteMasks) { mask.backSortingLayerID = 0; mask.frontSortingLayerID = 0; }
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        GetComponent<EdgeCollider2D>().enabled = true;
        foreach (SpriteRenderer sprite in sprites)
            sprite.sortingLayerName = "FrontOfCharacter";
        foreach (SpriteMask mask in spriteMasks) { mask.frontSortingLayerID = -375590083; mask.backSortingLayerID = -375590083; }
    }
}
