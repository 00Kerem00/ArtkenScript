using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownStairsCollider_1 : MonoBehaviour
{
    public DownStairs downStairs;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        downStairs.OnEnterCollider_1();
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        downStairs.OnExitCollider_1();
    }
}
