using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    public Leo leo;
    public GameObject defaultController;
    public Transform aimArrow;
    public Transform tfAimButton;
    private Vector3 tfAimButtonDefaultPosition;
    public Transform leftHandPosition;
    public Transform leftHandMaxForcePosition, leftHandMinForcePosition;
    public SpriteRenderer srAimArrow;
    public ButtonUpDownManager aimButton;

    public void OnClickCancel() 
    {
        leo.animator.SetTrigger("CancelThrow");
        leo.SwitchController(defaultController);
    }

    private void Update() 
    {
        SetAimButtonLocation();

        aimArrow.rotation = Quaternion.Euler(new Vector3(0, 0, GetAimAngle()));
        float scale = GetAimForce();
        aimArrow.localScale = new Vector3(scale, scale, 1);
    }
    private void SetAimButtonLocation() 
    {
        tfAimButton.position = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
    }
    private float GetAimAngle() 
    {
        float x, y;
        float result;
        Vector2 a;
        a = tfAimButtonDefaultPosition - tfAimButton.localPosition;
        x = a.x; y = a.y;

        result = Mathf.Atan(x / y) * Mathf.Rad2Deg + 90;
        if (tfAimButton.localPosition.y < tfAimButtonDefaultPosition.y) result += 180;
        return result *= -1;
    }
    private float GetAimForce() 
    {
        return Vector2.Distance(tfAimButtonDefaultPosition, tfAimButton.localPosition) / 60;
    }
    private void ThrowArrow(Vector2 throwForce) 
    {
        leo.animator.SetTrigger("Throw");
        GameObject newArrow = Instantiate(Resources.Load(@"Prefabs\Arrow", typeof(GameObject)), leo.weapon.gameObject.GetComponent<Transform>().position, Quaternion.Euler(new Vector3(0, 0, GetAimAngle() - 90))) as GameObject;
        Physics2D.IgnoreCollision(newArrow.GetComponent<BoxCollider2D>(), leo.gameObject.GetComponent<BoxCollider2D>());
        newArrow.GetComponent<Rigidbody2D>().AddForce(throwForce *= -1);
    }


    private void Awake() 
    {
        Vector2 a = new Vector2(0, 0), b = new Vector2(1, 1);
        float x = b.x - a.x;
        float y = b.y - a.y;
        Debug.Log(Mathf.Atan(x / y) * Mathf.Rad2Deg);

        tfAimButtonDefaultPosition = tfAimButton.localPosition;

        aimButton.OnDown = OnDownAimButton();
        aimButton.OnUp = OnUpAimButton();
    }

    public IEnumerator OnDownAimButton() 
    {
        aimArrow.gameObject.SetActive(true);
        this.enabled = true;

        yield return null;
        aimButton.OnDown = OnDownAimButton();
    }
    public IEnumerator OnUpAimButton() 
    {
        ThrowArrow((tfAimButton.localPosition - tfAimButtonDefaultPosition) * GetAimForce() * 2);
        aimArrow.gameObject.SetActive(false);
        this.enabled = false;
        tfAimButton.localPosition = tfAimButtonDefaultPosition;
        aimArrow.localScale = new Vector3(0, 0, 1);

        yield return null;
        aimButton.OnUp = OnUpAimButton();
    }
}
