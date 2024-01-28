using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightPanel : MonoBehaviour
{
    // STATICS
    public static List<string> enemies = new List<string>();
    public static FightPanel instance;
    public static FightPanel GetInstance() 
    {
        if (instance != null)
            return instance;
        else
        {
            instance = GameObject.Find("FightPanel").GetComponent<FightPanel>();
            if (instance == null)
                return new GameObject().AddComponent<FightPanel>();
            else
                return instance;
        }
    }
    public static List<GameObject> nextEnemies = new List<GameObject>();

    public static void OpenFightPanel()
    {
//        GetInstance().gameObject.SetActive(true);
        GetInstance().anim.Play("FightPanelOpen");
        instance.leoHealthBars[0].localPosition = new Vector2(GetBarPosition((int)instance.leo.health), instance.leoHealthBars[0].localPosition.y);
        instance.leoHealthBars[1].localPosition = instance.leoHealthBars[0].localPosition;
    }
    public static void CloseFightPanel()
    {
        StaticCoroutine.DoCoroutine(closeFightPanel());
    }
    private static IEnumerator closeFightPanel()
    {
        instance.anim.Play("FightPanelClose");
        yield return new WaitForSeconds(1);
//        instance.gameObject.SetActive(false);
    }

    public static void AddEnemy(string enemyName, int health, string faceSprite)
    {
        if (enemies.Exists(delegate(string item) { return enemyName == item; }))
            return;

        enemies.Add(enemyName);
        GetInstance().enemyHealthBarMovementCoroutines.Add(null);
        if (enemies.Count == 1)
        {
            OpenFightPanel(); Debug.Log("First Enemy");
            nextEnemies[0].name = enemyName;
        }
        else
        {
            instance.enemyPanel.Play("FP_AntagonistPanelShrink");
            RectTransform newEnemyPanel = Instantiate(Resources.Load<GameObject>(@"Prefabs\UI\EnemyHealthPanel")).GetComponent<RectTransform>();
            newEnemyPanel.SetParent(instance.enemyPanel.gameObject.GetComponent<Transform>());
            newEnemyPanel.offsetMin = new Vector2(0, 0);
            newEnemyPanel.offsetMax = new Vector2(0, 0); Debug.Log("Next Enemy");
            newEnemyPanel.localScale = new Vector3(1, 1, 1);
            newEnemyPanel.gameObject.GetComponent<Animation>().Play("FP_NewEnemyPassDown");
            newEnemyPanel.gameObject.name = enemyName;
            nextEnemies.Add(newEnemyPanel.gameObject);

            instance.enemyHealthBars.Add(newEnemyPanel.gameObject.GetComponent<FP_EnemyPanel>().healthBars);
        }
        foreach (RectTransform bar in nextEnemies[nextEnemies.Count - 1].GetComponent<FP_EnemyPanel>().healthBars)
            bar.localPosition = new Vector2(GetBarPosition(health), bar.localPosition.y);
        nextEnemies[nextEnemies.Count - 1].GetComponent<FP_EnemyPanel>().enemyFace.sprite = Resources.Load<Sprite>(@"UserInterface\FightPanel\" + faceSprite);
    }
    public static void RemoveEnemy(string enemyName, float delayTime)
    {
        StaticCoroutine.DoCoroutine(removeEnemy(enemyName, delayTime));
    }
    public static IEnumerator removeEnemy(string enemyName, float delayTime)
    {

        yield return new WaitForSeconds(delayTime);
//        try
//        {
            if (enemies.Count == 1)
                CloseFightPanel();
            else
            {
                if (enemies.Count == 2)
                instance.enemyPanel.Play("FP_AntagonistPanelExpand");
                Destroy(nextEnemies[enemies.IndexOf(enemyName)]);
                nextEnemies[enemies.IndexOf(enemyName)] = null;
            }
            nextEnemies = UpdateEnemyPanelPositions(nextEnemies);
            instance.enemyHealthBars[enemies.IndexOf(enemyName)] = null;
            instance.enemyHealthBars = UpdateEnemyHealthBars(instance.enemyHealthBars);
                GetInstance().enemyHealthBarMovementCoroutines.RemoveAt(enemies.IndexOf(enemyName));
            enemies.Remove(enemyName); 
//        }
//        catch { }
    }
    public static List<GameObject> UpdateEnemyPanelPositions(List<GameObject> currentList)
    {
        List<GameObject> result = new List<GameObject>();
        foreach (GameObject item in currentList)
        {
            if (item != null)
            {
                result.Add(item);
            }
        }

        for (int i = 0; i < result.Count; i++)
        {
            Debug.Log(i);
            result[i].GetComponent<RectTransform>().offsetMax = new Vector2(0, i * 120);
            result[i].GetComponent<RectTransform>().offsetMin = new Vector2(0, i * -120);
        }
        instance.enemy_0HealthPanel = result[0];
        return result;
    }
    public static List<RectTransform[]> UpdateEnemyHealthBars(List<RectTransform[]> currentHealthBars) 
    {
        List<RectTransform[]> result = new List<RectTransform[]>();
        foreach (RectTransform[] bars in currentHealthBars)
            if (bars != null)
                result.Add(bars);
        if (result.Count == 0)
            result.Add(instance.enemy_0HealthPanel.GetComponent<FP_EnemyPanel>().healthBars);
        return result;
    }

    public static void SetAntagonistHealthBarValue(string enemyName, int health) { GetInstance().setAntagonistHealthBarValue(enemies.IndexOf(enemyName), health); }
    public static void SetLeoHealthBarValue(int health) { instance.setLeoHealthBarValue(health); }

    private static float GetBarPosition(int health)
    {
        float barPosition;
        if (health > 50)
            barPosition = (health - 50) * 2.4f;
        else
            barPosition = -120 + health * 2.4f;
        return barPosition;
    }
    // END STATICS

    public Leo leo;
    public Animation anim;
    public Animation enemyPanel;

    public List<RectTransform[]> enemyHealthBars = new List<RectTransform[]>();
    public List<Coroutine> enemyHealthBarMovementCoroutines = new List<Coroutine>();
    public RectTransform[] enemy_0HealthBars;
    public GameObject enemy_0HealthPanel;

    public RectTransform[] leoHealthBars;
    private Coroutine leoHealthBarMovementCoroutine;

    private void Start() 
    {
        enemyHealthBars.Add(enemy_0HealthBars);
        nextEnemies.Add(enemy_0HealthPanel);
    }
    public void setLeoHealthBarValue(int health) 
    {
        float barPosition;
        if (health > 50)
            barPosition = (health - 50) * 2.4f;
        else
            barPosition = -120 + health * 2.4f;

        if (leoHealthBarMovementCoroutine != null)
            StopCoroutine(leoHealthBarMovementCoroutine);
        leoHealthBarMovementCoroutine = StartCoroutine(MoveBarToTargetPosition(leoHealthBars, barPosition));

    }
    public void setAntagonistHealthBarValue(int antagonistIndex, int health)
    {
        float barPosition = GetBarPosition(health);

        Debug.Log(enemyHealthBarMovementCoroutines[antagonistIndex] != null);
        if (enemyHealthBarMovementCoroutines[antagonistIndex] != null)
            StopCoroutine(enemyHealthBarMovementCoroutines[antagonistIndex]);
        enemyHealthBarMovementCoroutines[antagonistIndex] = StartCoroutine(MoveBarToTargetPosition(enemyHealthBars[antagonistIndex], barPosition));
    }

    public IEnumerator MoveBarToTargetPosition(RectTransform[] bars, float targetLocation)
    {
        Debug.Log(targetLocation);
        Vector2 targetPosition = new Vector2(targetLocation, bars[0].localPosition.y);

        while (Mathf.Abs(bars[0].localPosition.x - targetLocation) > 0.5f)
        {
            Vector2 smoothedPosition = Vector2.Lerp(bars[0].localPosition, targetPosition, 0.1f);
            bars[0].localPosition = smoothedPosition;
            bars[1].localPosition = smoothedPosition;
            yield return new WaitForEndOfFrame();
        }
        bars[0].localPosition = targetPosition;
        bars[1].localPosition = targetPosition;
        Debug.Log(targetLocation);
    }

}
