using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TransportableObject : MonoBehaviour
{
    public Leo leo;
    public Transform tf;
    public Transform leftActionPosition, rightActionPosition;
    private Transform toBeUsedActionPosition { get { if (sideOfLeo == MoveDirection.Right) return rightActionPosition; else return leftActionPosition; } }
    private Transform transportableObjectDefaultParent;
    private float leoDefaultSpeed;
    private bool pulling { get { return (sideOfLeo == leo.currentDirection); } }
    private bool lastPullingState = false;
    public bool holding = false;
    public MoveDirection sideOfLeo { get { if (leo.tf.position.x < tf.position.x) return MoveDirection.Left; else return MoveDirection.Right; } }
    public BoxCollider2D limit;
    private bool leoIsMoving = false;

    public void Interact() 
    {
        StartCoroutine(interact());
    }
    private IEnumerator interact() 
    {
        leoIsMoving = true;
        StartCoroutine(moveToInteract());

        yield return new WaitWhile(delegate { return leoIsMoving; });

        HoldObject();

        yield return new WaitForSeconds(0.33f);
        SetLeoStates();
        SetTransportableObjectStates();
        StartCoroutine(CheckPulling());
    }
    private IEnumerator moveToInteract() 
    {
        leo.animator.SetBool("Running", true);
        MoveDirection sideOfTransportable;
        if(leo.tf.position.x < tf.position.x)
            sideOfTransportable = MoveDirection.Right;
        else
            sideOfTransportable = MoveDirection.Left;

        while (Mathf.Abs(tf.position.x - leo.tf.position.x) > 1.1f) 
        {
            leo.Move(sideOfTransportable);
            yield return new WaitForEndOfFrame();
        }

        leo.animator.SetBool("Running", false);
        leoIsMoving = false;
    }

    public void HoldObject() 
    {
        GetComponent<InteractableObject>().interactionName = 6;
        leo.animator.SetTrigger("InteractTransportableObject");
        leo.maxSpeed = 0;
        holding = true;

    }

    public void ReleaseObject() 
    {
        GetComponent<InteractableObject>().interactionName = 5;
        leo.animator.SetTrigger("ReleaseTransportableObject");
        holding = false;

        SetLeoDefaultStates();
        SetTransportableObjectDefaultStates();
    }

    public void SetLeoStates() 
    {
        leo.maxSpeed = 2;
        leo.tf.position = toBeUsedActionPosition.position;
        leo.animator.gameObject.GetComponent<Transform>().SetParent(leo.tf);
    }
    public void SetTransportableObjectStates() 
    {
        tf.SetParent(leo.tf);
    }

    public void SetLeoDefaultStates() 
    {
        leo.maxSpeed = leoDefaultSpeed;
        if (sideOfLeo == MoveDirection.Right)
            leo.Rotate(MoveDirection.Left);
        else
            leo.Rotate(MoveDirection.Right);

        leo.animator.gameObject.GetComponent<Transform>().SetParent(leo.directablePart);
    }
    public void SetTransportableObjectDefaultStates() 
    {
        tf.SetParent(transportableObjectDefaultParent);
    }

    private IEnumerator CheckPulling() 
    {
        while (holding) 
        {
            if (lastPullingState != pulling) 
            {
                lastPullingState = !lastPullingState;
                leo.animator.SetBool("Pulling", lastPullingState);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void Start() 
    {
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), leo.gameObject.GetComponent<BoxCollider2D>());
        if (limit != null)
            Physics2D.IgnoreCollision(limit, leo.gameObject.GetComponent<BoxCollider2D>());
        leoDefaultSpeed = leo.maxSpeed;
        transportableObjectDefaultParent = tf.parent;
    }
}
