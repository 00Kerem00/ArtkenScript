using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessenger : MonoBehaviour
{
    public Animation animation;
    public Text text;
    private bool visible = false;
    public string[] textArray;

    public void ShowMessage(string message) 
    {
        text.text = message;
        animation.Play("UIMessengerShow");
        visible = true;
    }

    public void ShowMessage(string message, float duration)
    {
        text.text = message;
        animation.Play("UIMessengerShow");
        CancelInvoke("HideMessage");
        Invoke("HideMessage", duration);
        visible = true;
    }

    public void HideMessage() 
    {
        if (visible)
        {
            animation.Play("UIMessengerHide");
            visible = false;
        }
    }

    private void Start() 
    {
        textArray = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "UIMessenger"));
    }
}
