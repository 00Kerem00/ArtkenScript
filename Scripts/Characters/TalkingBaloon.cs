using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingBaloon : MonoBehaviour
{
    private int maxCharacterOfLine = 30;
    public Animation anim;
    public TextMesh textMesh;

    public void StartTalking(string text, float time) 
    {
        anim.Play("TalkingBaloonOpen");
        PrintTextToTextMesh(text);

        Invoke("FinishTalking", time);
    }

    public void FinishTalking() 
    {
        anim.Play("TalkingBaloonClose");
    }

    public void PrintTextToTextMesh(string text) 
    {
        StartCoroutine(printTextToTextMesh(text));
    }
    private IEnumerator printTextToTextMesh(string text) 
    {
        textMesh.text = "";
        int[] spaceLocs = GetSpaceLocations(text);
        int spaceNumber = 0;
        int[] lineMaxLocs = GetLineLocations(spaceLocs);
        int currentLine = 0;

        for (int i = 0; i < text.Length; i++) 
        {
            if (spaceLocs[spaceNumber + 1] > lineMaxLocs[currentLine]) { textMesh.text += "\n"; currentLine++; }
            if (text[i] == ' ') spaceNumber++; 

            textMesh.text += text[i];
            yield return new WaitForSeconds(0.01f);
        }
    }

    public int[] GetLineLocations(int[] spaceLocations) 
    {
        List<int> result = new List<int>();
        int lineCount = 1;
        int currentLineLoc = maxCharacterOfLine;

        foreach (int i in spaceLocations) 
        {
            if (i > currentLineLoc) { result.Add(i); lineCount++; currentLineLoc = lineCount * maxCharacterOfLine; }
        }
        result.Add(lineCount * maxCharacterOfLine);
        

        Debug.Log(result.Count);
        return result.ToArray();
    }

    public int[] GetSpaceLocations(string text) 
    {
        List<int> locs = new List<int>();
        int number = 0;

        foreach (char c in text) 
        {
            if (c == ' ')
                locs.Add(number);
            number++;
        }

        locs.Add(text.Length); 
        locs.Add(text.Length);


        return locs.ToArray();
    }

    private void Start() 
    {
        textMesh.gameObject.GetComponent<MeshRenderer>().sortingLayerName = "UI";
    }
}
