using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    public Animation anim;
    public Leo leo;
    public Image attackButton;
    public Transform itemList;
    public RectTransform list;

    public List<GameObject> itemButtons = new List<GameObject>();
    public string[] weaponNamesOfItems = { "Gun", "Zippo", "Axe", "Dynamite" };

    public void OpenInventoryPanel() 
    {
        UpdateItems();
        Debug.Log("Open Inventory Panel");
        anim.Play("OpenInventoryPanel");
        leo.currentController.GetComponent<Animation>().Play("HideWeaponIndicator");
        attackButton.gameObject.GetComponent<Button>().interactable = false;
    }
    public void CloseInventoryPanel() 
    {
        anim.Play("CloseInventoryPanel");
        leo.currentController.GetComponent<Animation>().Play("ShowWeaponIndicator");
        attackButton.gameObject.GetComponent<Button>().interactable = true;
    }

    public void UpdateItems() 
    {
        int itemCountOfLeo = leo.inventory.items.Count;
        int minimumCount;

        if (itemCountOfLeo < weaponNamesOfItems.Length)
            minimumCount = itemCountOfLeo;
        else
            minimumCount = weaponNamesOfItems.Length;

        for (int i = 0; i < minimumCount; i++)
            if (weaponNamesOfItems[i] != leo.inventory.items[i].name)
                SetItemOfItemButton(i, leo.inventory.items[i].name);

        if (itemCountOfLeo > itemButtons.Count)
            for (int i = itemButtons.Count; i < itemCountOfLeo; i++)
                itemButtons.Add(CreateNewItemButton(i, leo.inventory.items[i].name));
        else if (itemCountOfLeo < itemButtons.Count)
            RemoveUnneededItemButtons();

//        int listBottom = itemCountOfLeo * 100 - 400;
//        list.offsetMin = new Vector2(0, listBottom);
//        Debug.Log("Count Of Leo Items: " + leo.inventory.items.Count + ", Count Of Panel Items: " + weaponNamesOfItems.Length + ", Button Count: " + itemButtons.Count);
    }
    public GameObject CreateNewItemButton(int itemNumber, string itemName) 
    {
        GameObject newItemButton = Instantiate(Resources.Load<GameObject>(@"UserInterface\InventoryPanel\ItemButton"));
        newItemButton.name = "Item_" + itemNumber;        
        Button button = newItemButton.GetComponent<Button>();
        newItemButton.GetComponent<Transform>().SetParent(itemList);
        newItemButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(@"UserInterface\InventoryPanel\" + itemName);

        RectTransform rectTransform = newItemButton.GetComponent<RectTransform>();
        int buttonPosition = itemNumber * -100 - 80;
        rectTransform.localScale = new Vector2(0.8f, 0.8f);
        rectTransform.sizeDelta = new Vector2(100, 100);
        rectTransform.anchoredPosition = new Vector2(15, buttonPosition);

        System.Array.Resize(ref weaponNamesOfItems, weaponNamesOfItems.Length + 1);
        weaponNamesOfItems[weaponNamesOfItems.Length - 1] = itemName;
    //    button.onClick = null;
        button.onClick.AddListener(delegate { OnClickItemButton(itemNumber); });
        
        return newItemButton;
    }
    public void RemoveUnneededItemButtons() 
    {
        string[] newWeaponNames = new string[leo.inventory.items.Count];
        for (int i = 0; i < newWeaponNames.Length; i++)
            newWeaponNames[i] = weaponNamesOfItems[i];
        weaponNamesOfItems = newWeaponNames;

        for (int i = leo.inventory.items.Count; i < itemButtons.Count; i++) 
            Destroy(itemButtons[i]);
        for (int i = itemButtons.Count - 1; i >= leo.inventory.items.Count; i--)
            itemButtons.RemoveAt(i);

            Debug.Log("Leftover Buttons Removed");
    }
    public void SetItemOfItemButton(int itemNumber, string itemName) 
    {
        itemButtons[itemNumber].GetComponent<Image>().sprite = Resources.Load<Sprite>(@"UserInterface\InventoryPanel\" + itemName);
        weaponNamesOfItems[itemNumber] = itemName;
    }
    
    public void OnClickItemButton(int itemNumber) 
    {
        CloseInventoryPanel();
        attackButton.sprite = Resources.Load<Sprite>(@"UserInterface\InventoryPanel\" + weaponNamesOfItems[itemNumber]);
        leo.SwitchWeapon(weaponNamesOfItems[itemNumber]);
    }
    public void ClickNewButton() { Debug.Log("click"); }
}
