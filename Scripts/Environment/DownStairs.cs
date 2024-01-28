using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownStairs : MonoBehaviour
{
    public BoxCollider2D collider_0, collider_1;
    public EdgeCollider2D collider_stairs;
    public Collider2D floor;
    public GameObject CharacterMask;
    public GameObject collider_2;

    public void OnEnterCollider_0() 
    {
        collider_1.enabled = false;
        collider_2.SetActive(false);
    }
    public void OnExitCollider_0() 
    {
        collider_1.enabled = true;
        collider_2.SetActive(true);
    }

    public void OnEnterCollider_1() 
    {
        floor.enabled = false;
        collider_0.enabled = false;
        CharacterMask.SetActive(true);
        collider_stairs.enabled = true;
    }
    public void OnExitCollider_1() 
    {
        collider_stairs.enabled = false;
        floor.enabled = true;
        collider_0.enabled = true;
        CharacterMask.SetActive(false);
    }
}
