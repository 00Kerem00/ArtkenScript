using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingBaloonManager : MonoBehaviour
{
    public string[] textArray;

    private void Start() 
    {
        textArray = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "TalkingBaloon"));
    }
}
