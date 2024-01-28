using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float range = 40;
    private Vector3 hitPoint;
    private Transform tf;
    private Animation anim;
    public UnityEngine.EventSystems.MoveDirection direction;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();

        tf = GetComponent<Transform>();
        tf.SetParent(null);
        tf.rotation = Quaternion.Euler(0, 0, 0);

        Vector2 direction;
        if (this.direction == UnityEngine.EventSystems.MoveDirection.Left)
            direction = Vector2.left;
        else
            direction = Vector2.right;

        if (direction == Vector2.left)
            range *= -1;

        RaycastHit2D hit = Physics2D.Raycast(tf.position, direction, range);
        if (hit && !hit.collider.isTrigger && hit.collider.gameObject.name != "Leo")
        {
            hitPoint = hit.point;
            Debug.Log(hit.collider.gameObject.name);

            if (hit.collider.gameObject.tag == "Antagonist")
                target = hit.collider.gameObject;
        }
        else
            hitPoint = tf.position + new Vector3(range, 0, 0);

        StartCoroutine(MoveToHitPoint());
    }

    private IEnumerator MoveToHitPoint() 
    {
        while (Vector2.Distance(tf.position, hitPoint) > 0.6)
        {
            tf.position = Vector2.MoveTowards(tf.position, hitPoint, 0.5f);
            yield return new WaitForEndOfFrame();
        }
        anim.Play("BulletDestroy");
        HurtTarget();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    private void HurtTarget() 
    {
        if (target != null) 
        {
            target.GetComponent<Guard>().Hurt(direction, 40);
        }
    }
}
