using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonUpDownManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool interactable = true;
    public IEnumerator OnDown;
    public IEnumerator OnUp;

    public void OnPointerDown(PointerEventData eventData) 
    {
        if (interactable)
            StartCoroutine(OnDown);
    }

    public void OnPointerUp(PointerEventData eventData) 
    {
        if (interactable)
            StartCoroutine(OnUp);
    }
}
