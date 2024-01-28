using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animation anim;
    public string itemName;
    public int itemTextID;
    public int availability;
    private Transform tf;
    public IEnumerator onCollect;

    public static Collectable SpawnCollectable(Vector2 position, Leo.Inventory.Item item) 
    {
        Collectable collectable = Instantiate(Resources.Load("Collectable\\Collectable", typeof(GameObject)) as GameObject, position, Quaternion.Euler(new Vector3(0, 0, 0))).GetComponent<Collectable>();
        collectable.spriteRenderer.sprite = Resources.Load(@"Items\" + item.name, typeof(Sprite)) as Sprite;
        collectable.itemName = item.name;
        collectable.itemTextID = item.textID;
        collectable.availability = item.availability;

        return collectable;
    }
    public static Collectable SpawnCollectable(Vector2 position, Leo.Inventory.Item item, IEnumerator onCollect) 
    {
        Collectable collectable = Instantiate(Resources.Load("Collectable\\Collectable", typeof(GameObject)) as GameObject, position, Quaternion.Euler(new Vector3(0, 0, 0))).GetComponent<Collectable>();
        collectable.spriteRenderer.sprite = Resources.Load(@"Items\" + item.name, typeof(Sprite)) as Sprite;
        collectable.itemName = item.name;
        collectable.itemTextID = item.textID;
        collectable.availability = item.availability;
        collectable.onCollect = onCollect;

        return collectable;
    }

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo") 
        {
            anim.Play("Collectable_Destroy");
            GetComponents<BoxCollider2D>()[1].enabled = false;
            TextMeshMessenger.CreateTextMeshMessenger("+1 " + GameObject.Find("WeaponIndicator").GetComponent<WeaponIndicator>().itemNames[itemTextID], tf.position + new Vector3(0, 5, 0));
            GameObject.Find("Leo").GetComponent<Leo>().inventory.AddItem(new Leo.Inventory.Item(itemName, 1, availability, itemTextID));
            Invoke("Destroy", 0.5f);

            if (onCollect != null)
                StartCoroutine(onCollect);
        }
    }

    private void Destroy() 
    {
        Destroy(gameObject); 
    }

    private void Start() 
    {
        tf = GetComponent<Transform>();
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Leo").GetComponent<BoxCollider2D>());
    }
}
