using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationInterface : MonoBehaviour
{
    public string[] textArray;

    public Animation animation;
    public GameObject blurredImage;
    public Text conversationText;
    public Button nextButton;
    public Image secondCharacter;

    public int conversationLength;
    private int showedTextCount;
    public int[] conversation;
    public int[] conversationCharacterQueue;
    private int lastSecondCharacter = 2;

    public IEnumerator onFinishConversation;

    private void Start() 
    {
        textArray = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "Conversation"));
    }

    public void StartConversation(int[] conversation, int[] conversationCharacterQueue) 
    {
        try
        {
            HideBackUI();
        }
        catch { }
        StopLeo();
        this.conversation = conversation;
        this.conversationCharacterQueue = conversationCharacterQueue;
        showedTextCount = 0;
        conversationLength = conversation.Length;

        blurredImage.SetActive(true);
        OpenAccordingToCharacter();

        PrintTextToConversationText(textArray[conversation[0]], true);
    }
    public void FinishConversation() 
    {
        try
        {
            ShowBackUI();
        }
        catch { }
        ContinueLeo();
        blurredImage.SetActive(false);
        CloseAccordingToCharacter();

        if (onFinishConversation != null)
            StartCoroutine(onFinishConversation);
    }

    public void OnClickNextButton() 
    {
        showedTextCount++;
        Debug.Log("Index: " + showedTextCount + ", Length: " + conversationLength);
        if (showedTextCount < conversationLength)
        {
            UpdateCharacter();
            PrintTextToConversationText(textArray[conversation[showedTextCount]], false);
        }
        else
            FinishConversation();
    }

    public void OpenAccordingToCharacter() 
    {
        if (conversationCharacterQueue[0] == 1)
            animation.Play("ConversationUI_Open");
        else
            animation.Play("ConversationUI_Open_SC");
    }
    public void CloseAccordingToCharacter() 
    {
        if (conversationCharacterQueue[showedTextCount - 1] == 1)
            animation.Play("ConversationUI_Close");
        else
            animation.Play("ConversationUI_Close_SC");
    }
    public void UpdateCharacter() 
    {
        if (conversationCharacterQueue[showedTextCount - 1] != conversationCharacterQueue[showedTextCount])
        {
            if (conversationCharacterQueue[showedTextCount] == 1)
                animation.Play("ConversationUI_SCToLeo");
            else
            {
                animation.Play("ConversationUI_LeoToSC");
            }
        }
    }
    public void SetSecondCharacterImage(string scName) 
    {
        secondCharacter.sprite = Resources.Load<Sprite>(@"UserInterface\ConversationUI\" + scName);
    }

    public void PrintTextToConversationText(string text, bool delay) 
    {
        StartCoroutine(printTextToConversationText(text, delay));
    }
    private IEnumerator printTextToConversationText(string text, bool delay) 
    {
        nextButton.interactable = false;
        conversationText.text = "";

        if (delay)
            yield return new WaitForSeconds(0.5f);

        int textLength = text.Length;

        for (int i = 0; i < textLength; i++) 
        {
            conversationText.text += text[i];
            yield return new WaitForSeconds(0.01f);
        }
        nextButton.interactable = true;
    }

    private void StopLeo() 
    {
        DefaultController controller = GameObject.Find("Leo").GetComponent<Leo>().currentController.GetComponent<DefaultController>();
        controller.enabled = false;
        controller.animator.SetBool("Running", false);
        StartCoroutine(controller.OnUp_MoveLeft());
        StartCoroutine(controller.OnUp_MoveRight());
    }
    private void ContinueLeo() 
    {
        DefaultController controller = GameObject.Find("Leo").GetComponent<Leo>().currentController.GetComponent<DefaultController>();
        controller.enabled = true;
    }

    private void HideBackUI() 
    {
        Leo leo = GameObject.Find("Leo").GetComponent<Leo>();
        leo.currentController.GetComponent<Animation>().Play("HideWeaponIndicator");
        GameObject.Find("HealthIndicator").GetComponent<Animation>().Play("HideWeaponIndicator");
        if (leo.weaponIndicator.gameObject.GetComponent<CanvasGroup>().alpha == 1)
            leo.weaponIndicator.gameObject.GetComponent<Animation>().Play("HideWeaponIndicator");
    }
    private void ShowBackUI() 
    {
        Leo leo = GameObject.Find("Leo").GetComponent<Leo>();
        leo.currentController.GetComponent<Animation>().Play("ShowWeaponIndicator");
        GameObject.Find("HealthIndicator").GetComponent<Animation>().Play("ShowWeaponIndicator");
        if (leo.weaponIndicator.gameObject.GetComponent<CanvasGroup>().alpha == 0)
            leo.weaponIndicator.gameObject.GetComponent<Animation>().Play("ShowWeaponIndicator");
    }
}
