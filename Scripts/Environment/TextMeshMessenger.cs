using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMeshMessenger : MonoBehaviour
{
    public static void CreateTextMeshMessenger(string message, Vector3 position)
    {
        TextMesh messenger = Instantiate(Resources.Load(@"Environment\TextMeshMessenger", typeof(GameObject)) as GameObject,
            position, Quaternion.Euler(0, 0, 0)).GetComponentInChildren<TextMesh>();

        messenger.text = message;
//        messenger.font = GameObject.Find("UIMessenger").GetComponent<Text>().font;
        Color color = new Color();
        ColorUtility.TryParseHtmlString("#FFE8C4", out color);
        messenger.color = color;
    }

    private float r, g, b;

    private Transform tf;
    public TextMesh textMesh;
    public float alphaValueOfTextMeshColor = 255;
    private void Start()
    {
        textMesh.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "UI";
        AnimateTextMesh(); Invoke("Destroy", 1); Color color = textMesh.color; r = color.r; g = color.g; b = color.b; tf = GetComponent<Transform>();
    }
    private void Destroy() { Destroy(gameObject); }
    private void AnimateTextMesh()
    {
        alphaValueOfTextMeshColor = 255;
        StartCoroutine(TextMeshAnimation());
    }

    IEnumerator TextMeshAnimation()
    {
        while (alphaValueOfTextMeshColor > 0)
        {
            yield return new WaitForEndOfFrame();
            tf.position += new Vector3(0, 0.01f, 0);
            alphaValueOfTextMeshColor -= 3;
            textMesh.color = new Color(r, g, b, alphaValueOfTextMeshColor / 255);
        }
    }
}
