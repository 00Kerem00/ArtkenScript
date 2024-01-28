using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownStairsCollider_0 : MonoBehaviour
{
    public DownStairs downStairs;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        downStairs.OnEnterCollider_0();
    }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        downStairs.OnExitCollider_0();
    }
}
