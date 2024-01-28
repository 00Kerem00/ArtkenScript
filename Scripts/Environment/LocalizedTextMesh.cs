using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizedTextMesh : MonoBehaviour
{
    public TextMesh textMesh;
    public int textNumber;

    void Start()
    {
        GetComponent<MeshRenderer>().sortingLayerName = "UI";
        textMesh.text = GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().texts[textNumber];
    }

    public static GameObject SpawnLocalizedText(int[] texts, Vector2 location) 
    {
        GameObject textMesh = Instantiate(Resources.Load<GameObject>(@"Prefabs\UI\LocalizedText"));
        textMesh.name = "LocalizedText";
        textMesh.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        textMesh.GetComponent<Transform>().position = location;
        textMesh.GetComponent<Animation>().Play("ShowTextMesh");
        textMesh.GetComponent<TextMesh>().text = "";
        string[] textArrayOfScene = GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().texts;
        foreach (int i in texts)
            textMesh.GetComponent<TextMesh>().text += textArrayOfScene[i] + "\n";

        return textMesh;
    }
    public static void SpawnLocalizedText(int[] texts, Vector2 location, float delayTime) 
    {
        StaticCoroutine.DoCoroutine(spawnDelayedLocalizedText(texts, location, delayTime));
    }
    private static IEnumerator spawnDelayedLocalizedText(int[] texts, Vector2 location, float delayTime) 
    {
        yield return new WaitForSeconds(delayTime);
        SpawnLocalizedText(texts, location);
    }

    public static void DestroyLocalizedText(GameObject localizedText) { StaticCoroutine.DoCoroutine(destroyLocalizedText(localizedText)); }
    private static IEnumerator destroyLocalizedText(GameObject localizedText) 
    {
        localizedText.GetComponent<Animation>().Play("HideTextMesh");
        yield return new WaitForSeconds(0.5f);
        Destroy(localizedText);
    }
}
