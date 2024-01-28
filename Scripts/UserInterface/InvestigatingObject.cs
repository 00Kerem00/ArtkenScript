using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatingObject : MonoBehaviour
{
    public AudioClip closeSound;
    public IEnumerator closeEvent;
    public string investigatinObjectName;
    public GameObject[] investigatingParts;
    public GameObject[] deactivatingObjects;
    public UnityEngine.UI.Button[] interactionButtons;
    public UnityEngine.UI.Text[] texts;
    public UnityEngine.UI.Text[] localizedText;
    public int[] localizedTextNumbers;
    private InvestigateUI investigateUI;
    public RectTransform[] movingObjects;
    public Vector3 touchOffset;
    public RectTransform selectedMovingObject;

    private void ButtonFunction(GameObject interactable) { investigateUI.OnClickDeepInvestigate(interactable); }

    private void Awake() 
    {
        investigateUI = GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>();
        SetInvestigatingObjectStates();
        OnOpenObject();
    }

    private void OnOpenObject() 
    {
        Debug.Log(investigatinObjectName);
        switch (investigatinObjectName) 
        {
            case "InvestigatingTable": StartCoroutine(GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().OnOpenInvestigatingTable(this)); break;
            case "InvestigatingEncryptedDoorUI": StartCoroutine(GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().OnOpenInvestigateEncryptedDoorUI(this)); break;
            case "InvestigatingShelf": StartCoroutine(GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().OnOpenInvestigatingShelf(this)); break;
            case "InvestigatingFireplace": StartCoroutine(GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().OnOpenInvestigatingFireplace(this)); break;
            case "ArtkenCard": StartCoroutine(GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().OnOpenArtkenCard()); break;
            case "TV_Interface": StartCoroutine(GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().OnOpenTV_Interface(this)); break;
            case "InvestigatingSpinningTable": StartCoroutine(GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().OnOpenSpinningTable(this)); break;
            case "Inves_MazeBox": StartCoroutine(GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().OnOpenMazeBox(this)); break;
            case "U0Cabinet_0": StartCoroutine(GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().OnOpenCabinet_0(this)); break;
            case "U0Cabinet_1": StartCoroutine(GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().OnOpenCabinet_1(this)); Debug.Log("Open Cabinet-1"); break; 
            case "Inves_FenceDoor": StartCoroutine(GameObject.Find("Underground_0Events").GetComponent<Underground_0Events>().OnOpenFenceDoor(this)); break;
        }
    }

    private void Update() 
    {
        selectedMovingObject.position = Camera.main.ScreenPointToRay(Input.mousePosition).origin - touchOffset;
    }

    public void OnClickMessageButton(int messageNumber) 
    {
        UIMessenger uiMessenger = GameObject.Find("UIMessenger").GetComponent<UIMessenger>();
        uiMessenger.ShowMessage(uiMessenger.textArray[messageNumber], 2);
    }

    private void SetInvestigatingObjectStates() 
    {
        SetInteractionButtonFunctions();
        SetDeactivatingObjectStates();
        SetMovingObjectPositions();
        SetTexts();
        SetLocalizedTexts();
    }
    private void SetInteractionButtonFunctions() 
    {
        foreach (UnityEngine.UI.Button button in interactionButtons)
        {
            button.onClick.AddListener(delegate { ButtonFunction(button.gameObject); });
        }
    }
    private void SetDeactivatingObjectStates() 
    {
        if (deactivatingObjects.Length != 0)
        {
            bool[] states = DatabaseManager.GetDeactivatingObjectStates(investigatinObjectName);

            if(states.Length != 0)
                for (int i = 0; i < deactivatingObjects.Length; i++)
                {
                    if (states[i] != null)
                        deactivatingObjects[i].SetActive(states[i]);
                    else
                        deactivatingObjects[i].SetActive(true);
                }
        }
    }
    private void SetMovingObjectPositions() 
    {
        if (movingObjects.Length != 0)
        {
            Vector2[] positions = DatabaseManager.GetMovingObjectPositions(investigatinObjectName);

            if (positions.Length != 0)
                for (int i = 0; i < movingObjects.Length; i++)
                {
                    if (positions[i] != null)
                        movingObjects[i].position = positions[i];
                }
        }
    }
    private void SetTexts() 
    {
        if (texts.Length != 0) 
        {
            string[] values = DatabaseManager.GetTextsOfInvestigationObject(investigatinObjectName);

            for (int i = 0; i < values.Length; i++ )
            {
                texts[i].text = values[i];
            }
        }
    }
    private void SetLocalizedTexts() 
    {
        string[] textArray;
        switch(GeneralVariables.scene)
        {
            case GeneralVariables.Scene.Factory: textArray = GameObject.Find("FactoryEvents").GetComponent<FactoryEvents>().texts; break;
            default: textArray = new string[0]; break;
        }

        if (localizedText.Length != 0)
            for (int i = 0; i < localizedText.Length; i++)
                localizedText[i].text = textArray[localizedTextNumbers[i]];

    }

    public void SetInvestigatingObjectStatesInDatabase() 
    {
        SetDeactivatingObjectInDatabase();
        SetMovingObjectPositionsInDatabase();
        SetTextsInDatabase();
    }
    private void SetDeactivatingObjectInDatabase() 
    {
        if (deactivatingObjects.Length != 0)
        {
            List<bool> values = new List<bool>();

            foreach (GameObject go in deactivatingObjects)
                values.Add(go.activeSelf);

            DatabaseManager.SetDeactivatingObjectStates(investigatinObjectName, values.ToArray());
        }
    }
    private void SetMovingObjectPositionsInDatabase() 
    {
        if (movingObjects.Length != 0)
        {
            List<Vector2> values = new List<Vector2>();

            foreach (RectTransform rt in movingObjects)
                values.Add(rt.position);

            DatabaseManager.SetMovingObjectPositions(investigatinObjectName, values.ToArray());
        }
    }
    private void SetTextsInDatabase() 
    {
        if (texts.Length != 0)
        {
            List<string> values = new List<string>();

            foreach (UnityEngine.UI.Text text in texts)
                values.Add(text.text);

            DatabaseManager.SetTextsOfInvestigatingObject(investigatinObjectName, values.ToArray());
        }
    }

    public void SetEventInteractionButtonClick(int buttonNumber, UnityEngine.Events.UnityAction function) 
    {
        interactionButtons[buttonNumber].onClick.AddListener(function);
    }

    public void SetEventButtonInteraction(int buttonNumber, IEnumerator sceneEvent) 
    {
        interactionButtons[buttonNumber].gameObject.GetComponent<InteractableObject>().sceneEvent = sceneEvent;
    }
    public void SetEventBeforeButtonInteraction(int buttonNumber, IEnumerator eventBeforeInteraction)
    {
        interactionButtons[buttonNumber].gameObject.GetComponent<InteractableObject>().eventBeforeInteraction = eventBeforeInteraction;
    }
}
