using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerForEditor : MonoBehaviour
{
    public DefaultController controller;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(controller.OnDown_MoveLeft());
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(controller.OnDown_MoveRight());

        if (Input.GetKeyUp(KeyCode.A))
            StartCoroutine(controller.OnUp_MoveLeft());
        if (Input.GetKeyUp(KeyCode.D))
            StartCoroutine(controller.OnUp_MoveRight());

        if (Input.GetKeyDown(KeyCode.Space))
            controller.OnClick_Jump();

        if (Input.GetKeyDown(KeyCode.M)) 
        {
            List<string> enemies = FightPanel.enemies;
            string enemyNames = enemies[0];
            for (int i = 1; i < enemies.Count; i++)
                enemyNames += ", " + enemies[i];
            Debug.Log("Enemies: " + enemyNames);
            Debug.Log("Coroutine Count: " + FightPanel.instance.enemyHealthBarMovementCoroutines.Count);
            Debug.Log("Next Enemies Count: " + FightPanel.nextEnemies.Count);
            Debug.Log("Health Bars Count: " + FightPanel.instance.enemyHealthBars.Count);
        }
    }
}
