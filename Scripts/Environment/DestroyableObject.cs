using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    public float health;
    public Animation animation;
    public Transform healthIndicator;
    public int pieceCount;
    public string pieceName;
    public string collectableName;
    public int collectableTextID;
    public bool characterCollision;

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        Debug.Log(collision.collider.name + ", " + collision.collider.tag);
        if (collision.collider.tag == "Damaging") 
        {
            healthIndicator.gameObject.GetComponent<SpriteRenderer>().enabled = true;
            Damage(collision);
        }
    }

    private void Damage(Collision2D collision) 
    {
        health -= collision.collider.GetComponent<Damaging>().damageUnit;
        animation.Play("DestroyableObjectDamage");
        healthIndicator.localScale = new Vector3(health / 100, 0.05f, 1);

        if (health <= 0)
            Explode();
    }

    private void Explode() 
    {
        Vector3 position = GetComponent<Transform>().position;

        animation.Play("ADO_Destroy");

        GameObject woodenPiece = Instantiate(Resources.Load(@"Prefabs\" + pieceName, typeof(GameObject)), position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)))) as GameObject;
        woodenPiece.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5, 5) * 10, Random.Range(1, 5) * 10));

        for (int i = 1; i < pieceCount; i++) 
        {
            GameObject newWoodenPiece = Instantiate(woodenPiece, position, Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))));
            Vector2 force = new Vector2(Random.Range(-5, 5) * 400, Random.Range(1, 5) * 400);
            Debug.Log(force);
            newWoodenPiece.GetComponent<Rigidbody2D>().AddForce(force);
        }
        if (collectableName != "")
            Collectable.SpawnCollectable(GetComponent<Transform>().position, new Leo.Inventory.Item(collectableName, 1, 100, collectableTextID));

        Destroy(gameObject);
    }

    private void Start() 
    {
        if (!characterCollision)
            Physics2D.IgnoreCollision(GetComponents<Collider2D>()[0], GameObject.Find("Leo").GetComponent<BoxCollider2D>());
    }
}
