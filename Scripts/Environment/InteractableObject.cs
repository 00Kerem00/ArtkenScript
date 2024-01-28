using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public AudioClip interactionSound;
    public IEnumerator sceneEvent;
    public IEnumerator selectEvent;
    public IEnumerator unselectEvent;
    public IEnumerator eventBeforeInteraction;

    public Animation colorAnim;
    public bool interactable;
    public bool isUI = false;
    public enum InteractionType { AddItemToInventory, PassToPanorama, Investigate, DeepInvestigate, AddItemToInventoryInUI, InteractTransportableObject, None, EmptyInteraction }
    public InteractionType interactionType;
    public string[] interactionParams;

    public int interactableName, interactionName;

    private bool closeEnough { get { return Vector2.Distance(GetComponent<Transform>().position, GameObject.Find("Leo").GetComponent<Transform>().position) < GeneralVariables.maxInteractionDistance; } }
    public void Interact() 
    {
        if (eventBeforeInteraction != null)
            StartCoroutine(eventBeforeInteraction);

        if (closeEnough || isUI)
        {
            if (interactionSound != null)
                GameObject.Find("AudioSource").GetComponent<AudioSource>().PlayOneShot(interactionSound);

            switch (interactionType)
            {
                case InteractionType.AddItemToInventory: AddItemToInventory(interactionParams[0],
                    int.Parse(interactionParams[1]), new Vector2(float.Parse(interactionParams[2]), float.Parse(interactionParams[3])), int.Parse(interactionParams[4]), interactionParams[5]); break;
                case InteractionType.PassToPanorama: PassToPanorama(interactionParams[0], interactionParams[1]); break;
                case InteractionType.Investigate: if (interactionParams.Length == 2) InvestigateObject(interactionParams[0], float.Parse(interactionParams[1])); else InvestigateObject(interactionParams[0]); break;
                case InteractionType.DeepInvestigate: DeepInvestigateObject(int.Parse(interactionParams[0])); break;
                case InteractionType.AddItemToInventoryInUI: AddItemToInventoryInUI(interactionParams[0], int.Parse(interactionParams[1]), int.Parse(interactionParams[2]), int.Parse(interactionParams[3]), interactionParams[4]); break;
                case InteractionType.InteractTransportableObject: InteractTransportableObject(interactionParams[0]); break;
                case InteractionType.None: break;
            }

            if (sceneEvent != null)
                StartCoroutine(sceneEvent);

            GameObject.Find("InteractionSelectButton").GetComponent<InteractionManager>().UnselectInteractableObject();
        }
    }

    private void AddItemToInventory(string itemName, int itemCount, Vector3 textMeshMessengerOffset, int textID, string once) 
    {
        GameObject.Find("Leo").GetComponent<Leo>().inventory.AddItem(itemName, itemCount, textID);
        if(textMeshMessengerOffset != null)
        TextMeshMessenger.CreateTextMeshMessenger("+" + itemCount + " " +
            GameObject.Find("InteractionSelectButton").GetComponent<InteractionManager>().interactableNames[interactableName], GetComponent<Transform>().position + textMeshMessengerOffset);

        if (once == "True")
            Destroy(gameObject);
    }
    private void AddItemToInventoryInUI(string itemName, int itemCount, int availability, int textID , string destroyingObjectName) 
    {
        GameObject.Find("Leo").GetComponent<Leo>().inventory.AddItem(itemName, itemCount, availability, textID);
        string itemText = GameObject.Find("InteractionSelectButton").GetComponent<InteractionManager>().interactableNames[interactableName];
        GameObject.Find("UIMessenger").GetComponent<UIMessenger>().ShowMessage("+ " + itemCount + " " + itemText);
        GameObject.Find("UIMessenger").GetComponent<UIMessenger>().Invoke("HideMessage", 1.5f);
        GameObject.Find(destroyingObjectName).SetActive(false);
    }

    private void PassToPanorama(string panoramaName, string specesificStartPosition) 
    {
        StartCoroutine(passToPanorama(panoramaName, specesificStartPosition));
    }
    private IEnumerator passToPanorama(string panoramaName, string specesificStartPosition) 
    {
        GameObject.Find("PanoramaPassingImage").GetComponent<Animation>().Play("FrontImage_Closing");
        yield return new WaitForSeconds(1);
        GameObject.Find("Leo").GetComponent<Transform>().position = GameObject.Find(specesificStartPosition).GetComponent<Transform>().position;
        Camera.main.GetComponent<MultiLayer>().currentPanorama = GameObject.Find(panoramaName).GetComponent<Panorama>();
    }

    private void InvestigateObject(string investigatingObjectName) 
    {
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().ShowInvestigateUI(investigatingObjectName);
    }
    private void InvestigateObject(string investigatingObjectName, float delayTime)
    {
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().ShowInvestigateUI(investigatingObjectName, delayTime);
    }
    private void DeepInvestigateObject(int investigatingPartNumber) 
    {
        GameObject.Find("PInvestigateUI").GetComponent<InvestigateUI>().DeepInvestigate(investigatingPartNumber);
    }

    private void InteractTransportableObject(string objectName)
    {
        TransportableObject transportableObject = GameObject.Find(objectName).GetComponent<TransportableObject>();

        if (!transportableObject.holding)
            HoldTransportableObject(transportableObject);
        else
            ReleaseTransportableObject(transportableObject);
    }
    private void HoldTransportableObject(TransportableObject transportableObject) 
    {
        transportableObject.Interact();
    }
    private void ReleaseTransportableObject(TransportableObject transportableObject) { transportableObject.ReleaseObject(); }

    private void Start() { colorAnim = GetComponent<Animation>(); interactable = true; }
}
