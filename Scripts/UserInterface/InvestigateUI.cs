using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigateUI : MonoBehaviour
{
    public GameObject investigateUI;
    public Animation foregroundAnimation;
    public InteractionManager interactionManager;
    private GameObject investigateObject;
    private GameObject[] investigatingParts;
    public UnityEngine.UI.Image interactionSelectButton;
    public GameObject indicators;
    public IEnumerator hideEvent;
    public List<int> partQueue;
    public AudioSource audioSource;

    public void LoadInvestigateObject(string investigatingObjectName) 
    {
        investigateObject = Instantiate(Resources.Load(@"Prefabs\InvestigateObjects\" + investigatingObjectName, typeof(GameObject)) as GameObject, investigateUI.GetComponent<Transform>());
        investigatingParts = investigateObject.GetComponent<InvestigatingObject>().investigatingParts;
    }
    private void CloseInvestigatingObject() 
    {
        InvestigatingObject component = investigateObject.GetComponent<InvestigatingObject>();
        if (component.closeSound != null)
            audioSource.PlayOneShot(component.closeSound);
        if (component.closeEvent != null)
            StartCoroutine(component.closeEvent);
        component.SetInvestigatingObjectStatesInDatabase();
        Destroy(investigateObject);
    }

    public void ShowInvestigateUI(string investigatingObjectName) 
    {
        partQueue = new List<int>();
        partQueue.Add(0);
        SetOtherUIStates(false);
        LoadInvestigateObject(investigatingObjectName);
        foregroundAnimation.Play("FrontImage_OnChangedPanorama");
        investigateUI.SetActive(true);
    }

    public void ShowInvestigateUI(string investigatingObjectName, float delayTime)
    {
        StartCoroutine(showInvestigateUI(investigatingObjectName, delayTime));
    }

    private IEnumerator showInvestigateUI(string investigatingObjectName, float delayTime) 
    {
        foregroundAnimation.gameObject.GetComponent<UnityEngine.UI.Image>().color = Color.black;
        partQueue = new List<int>();
        partQueue.Add(0);
        SetOtherUIStates(false);
        LoadInvestigateObject(investigatingObjectName);
        yield return new WaitForSeconds(delayTime);
        
        foregroundAnimation.Play("FrontImage_OnChangedPanorama");
        investigateUI.SetActive(true);
    }
    public void HideInvestigateUI() 
    {
        SetOtherUIStates(true);
        foregroundAnimation.Play("FrontImage_OnChangedPanorama");
        investigateUI.SetActive(false);
        CloseInvestigatingObject();

        if (hideEvent != null)
            StartCoroutine(hideEvent);
    }
    public void SetOtherUIStates(bool value) 
    {
        indicators.SetActive(value);
        GameObject.Find("Leo").GetComponent<Leo>().currentController.SetActive(value);
        interactionSelectButton.enabled = value;
    }
    public void OnClickDeepInvestigate(GameObject interactable) 
    {
        interactionManager.SelectInteractableObject(interactable);
    }
    public void BackPart() 
    {
        if (partQueue.Count == 1) { HideInvestigateUI(); return; }

        partQueue.RemoveAt(partQueue.Count - 1);

        foregroundAnimation.Play("FrontImage_OnChangedPanorama");
        for (int i = 0; i < investigatingParts.Length; i++)
        {
            investigatingParts[i].SetActive(i == partQueue.ToArray()[partQueue.Count - 1]);
        }
        foreach (int i in partQueue)
            Debug.Log(i);
    }
    public void DeepInvestigate(int investigatingPartNumber) 
    {
        foregroundAnimation.Play("FrontImage_OnChangedPanorama");
        for (int i = 0; i < investigatingParts.Length; i++) 
        {
            investigatingParts[i].SetActive(i == investigatingPartNumber);
        }

        partQueue.Add(investigatingPartNumber);
    }

    private void Start() { partQueue = new List<int>(); partQueue.Add(0); }
}
