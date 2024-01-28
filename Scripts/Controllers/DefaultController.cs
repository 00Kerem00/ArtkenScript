using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultController : MonoBehaviour
{
    public Leo leo;
    public Animator animator;

    public Animation leftMoveArrow, rightMoveArrow, jumpArrow;

    public bool moveLeftIsHolding;
    public bool moveRightIsHolding;

    public ButtonUpDownManager Btn_MoveLeft;
    public ButtonUpDownManager Btn_MoveRight;
    public ButtonUpDownManager Btn_Attack;
    public InventoryPanel inventoryPanel;

    private void Update() 
    {
        if (moveLeftIsHolding)
            leo.Move(MoveDirection.Left);
        if (moveRightIsHolding)
            leo.Move(MoveDirection.Right);
    }

    private void Start() 
    {
        Btn_MoveLeft.OnDown = OnDown_MoveLeft();
        Btn_MoveLeft.OnUp = OnUp_MoveLeft();

        Btn_MoveRight.OnDown = OnDown_MoveRight();
        Btn_MoveRight.OnUp = OnUp_MoveRight();

        Btn_Attack.OnDown = OnDown_AttackButton();
        Btn_Attack.OnUp = OnUp_AttackButton();
    }

    public IEnumerator OnDown_MoveLeft() 
    {
        leftMoveArrow.Play("ImgLeftMoveArrow");

        animator.SetBool("Running", true);
        moveLeftIsHolding = true;
        yield return null;
        Btn_MoveLeft.OnDown = OnDown_MoveLeft();
    }
    public IEnumerator OnUp_MoveLeft() 
    {
        leftMoveArrow.Play("ImgLeftMoveArrowReverse");

        animator.SetBool("Running", false);
        moveLeftIsHolding = false;
        yield return null;
        Btn_MoveLeft.OnUp = OnUp_MoveLeft();
    }

    public IEnumerator OnDown_MoveRight()
    {
        rightMoveArrow.Play("ImgRightMoveArrow");

        animator.SetBool("Running", true);
        moveRightIsHolding = true;
        yield return null;
        Btn_MoveRight.OnDown = OnDown_MoveRight();
    }
    public IEnumerator OnUp_MoveRight()
    {
        rightMoveArrow.Play("ImgRightMoveArrowReverse");

        animator.SetBool("Running", false);
        moveRightIsHolding = false;
        yield return null;
        Btn_MoveRight.OnUp = OnUp_MoveRight();
    }

    public IEnumerator OnDown_AttackButton() 
    {
        waitForOpenInventoryPanel = StartCoroutine(WaitForOpenInventoryPanel());
        Btn_Attack.OnDown = OnDown_AttackButton();
        yield return null;
    }
    public IEnumerator OnUp_AttackButton() 
    {
        StopCoroutine(waitForOpenInventoryPanel);
        waitForOpenInventoryPanel = null;
        Btn_Attack.OnUp = OnUp_AttackButton();
        yield return null;
    }
    private IEnumerator WaitForOpenInventoryPanel() 
    {
        yield return new WaitForSeconds(0.5f);
        inventoryPanel.OpenInventoryPanel();
    }
    private Coroutine waitForOpenInventoryPanel;

    public void OnClick_Jump() 
    {
        jumpArrow.Play("ImgJumpArrow");
        leo.Jump();
    }

    public void OnClick_Attack() 
    {
        leo.Attack();
    }
}
