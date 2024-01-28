using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvesMovingObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public InvestigatingObject investigatingObject;

    public void OnPointerDown(PointerEventData eventData)
    {
        investigatingObject.touchOffset = Camera.main.ScreenPointToRay(Input.mousePosition).origin - GetComponent<RectTransform>().position;
        investigatingObject.touchOffset = new Vector2(investigatingObject.touchOffset.x, investigatingObject.touchOffset.y);
        investigatingObject.selectedMovingObject = gameObject.GetComponent<RectTransform>();
        investigatingObject.enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        investigatingObject.enabled = false;
    }
}
