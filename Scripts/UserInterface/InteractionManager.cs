using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public string[] interactableNames;
    public string[] interactionNames;

    public InteractableObject selectedInteractableObject;
    public Animation interactButton;
    public Text buttonText;
    public Leo leo;
    public Coroutine distanceCoroutine;
    public UIMessenger uiMessenger;

    private void Start() 
    {
        interactableNames = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "InteractableNames"));
        interactionNames = TextManager.GetTextArray(TextManager.Address.Build(GeneralVariables.language, GeneralVariables.scene, "InteractionNames"));
    }

    public void OnClickSelectButton() 
    {
        if (distanceCoroutine != null)
            StopCoroutine(distanceCoroutine);

        bool hitToInteractable = false;

        Vector3 touchPosition = Camera.main.ScreenPointToRay(Input.mousePosition).origin;

        RaycastHit2D[] hits = Physics2D.RaycastAll(touchPosition, Vector2.zero);
        Debug.DrawLine(touchPosition, new Vector3(touchPosition.x, touchPosition.y, 10), Color.red, 2);
        hits = GeneralVariables.ReverseHitArray(hits);

        foreach (RaycastHit2D hit in hits) 
        {
            if (hit.collider.gameObject.tag == "Interactable")
            {
                SelectInteractableObject(hit.collider.gameObject);
                Debug.Log(hit.point);
                hitToInteractable = true;
                break;
            }
            else if (hit.collider.gameObject.tag == "Uninteractive") 
            {
                hit.collider.gameObject.GetComponent<Animation>().Play("SelectInteractable");
                uiMessenger.ShowMessage(uiMessenger.textArray[9], 0.5f);
                break;
            }
        }

        if (!hitToInteractable && selectedInteractableObject != null)
            UnselectInteractableObject();
    }
    public void OnClickInteractButton()
    {
        selectedInteractableObject.Interact();
    }

    public void SelectInteractableObject(GameObject selectedObject) 
    {
        if (selectedObject.GetComponent<InteractableObject>().interactionType != InteractableObject.InteractionType.None)
        {
            selectedInteractableObject = selectedObject.GetComponent<InteractableObject>();
            if (selectedInteractableObject.colorAnim != null)
                selectedInteractableObject.colorAnim.Play("SelectInteractable");

            float distance = Vector2.Distance(leo.tf.position, selectedInteractableObject.GetComponent<Transform>().position);
            string interactionName;
            if (distance > GeneralVariables.maxInteractionDistance && !selectedInteractableObject.isUI)
            {
                interactionName = interactionNames[1];
                interactButton.gameObject.GetComponent<Button>().interactable = false;
                Invoke("UnselectInteractableObject", 1.5f);
            }
            else if (selectedInteractableObject.interactable)
            {
                interactionName = interactionNames[selectedInteractableObject.interactionName];
                interactButton.gameObject.GetComponent<Button>().interactable = true;

                if (!selectedInteractableObject.isUI)
                    distanceCoroutine = StartCoroutine(CheckLeoInteractableDistance());
            }
            else 
            {
                interactionName = interactionNames[selectedInteractableObject.interactionName];
                interactButton.gameObject.GetComponent<Button>().interactable = false;
                Invoke("UnselectInteractableObject", 1);
            }

            buttonText.text = interactableNames[selectedInteractableObject.interactableName] + " (" + interactionName + ")";
            interactButton.Play("ShowInteractButton");
        }

        if (selectedObject.GetComponent<InteractableObject>().selectEvent != null)
            StartCoroutine(selectedObject.GetComponent<InteractableObject>().selectEvent);
    }
    private int tempNum = 0;
    public IEnumerator CheckLeoInteractableDistance() 
    {
        Debug.Log("Distance Checking...");
        float distance = 0;
        Transform SITPos = selectedInteractableObject.GetComponent<Transform>();
        while (distance < GeneralVariables.maxInteractionDistance)
        {
            distance = Vector2.Distance(leo.tf.position, SITPos.position);
            yield return new WaitForSeconds(0.1f);
        }
        UnselectInteractableObject();
    }
    public void UnselectInteractableObject() 
    {
        if (distanceCoroutine != null)
            StopCoroutine(distanceCoroutine);
        if (selectedInteractableObject.unselectEvent != null)
            StartCoroutine(selectedInteractableObject.unselectEvent);
        selectedInteractableObject = null;
        interactButton.Play("CloseInteractButton");
    }
}
