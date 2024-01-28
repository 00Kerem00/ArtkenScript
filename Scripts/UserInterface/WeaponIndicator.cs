using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIndicator : MonoBehaviour
{
    public string[] itemNames;

    public Animation anim;
    public Image selectedWeaponImg;
    public Transform selectedWeaponAvailability;
    public Text selectedWeaponTxt;
    public Leo leo;

    public List<Leo.Inventory.Item> weapons;
    public Leo.Inventory.Item selectedWeapon;
    private int pointer = 0;

    private void Start() 
    {
        itemNames = TextManager.GetTextArray(@"Texts\" + GeneralVariables.language + @"\ItemNames");
    }

    public void ChangeWeapon(string weaponName)
    {
        Debug.Log("Ulaa!!");
        UpdateWeapons();
        pointer = -1;
        Debug.Log("Lan!!");
        foreach (Leo.Inventory.Item item in weapons)
        {
            if (item != null && item.name == weaponName)
            {
                ChangeWeapon(); break;
            }
            pointer++;
        }
    }
    public void ChangeWeapon() 
    {
        UpdateWeapons();

        pointer++;
        if (pointer == weapons.Count)
        {
            pointer = 0;
            if (weapons.Count != 1)
                HideIndicator();
            leo.SwitchWeapon("None");
        }
        else if (pointer == 1)
        {
            selectedWeapon = weapons[pointer];
            selectedWeaponTxt.text = itemNames[selectedWeapon.textID];
            selectedWeaponImg.sprite = Resources.Load(@"UserInterface\WeaponIndicator\Weapon_" + weapons[pointer].name, typeof(Sprite)) as Sprite;
            leo.SwitchWeapon(selectedWeapon.name);
            anim.Play("ShowWeaponIndicator");
        }
        else 
        {
            selectedWeapon = weapons[pointer];
            Debug.Log("Hoppp");
            if (pointer == 1)
                ShowIndicator();
            leo.SwitchWeapon(selectedWeapon.name);
            StartCoroutine(changeWeapon());
        }
    }
    private IEnumerator changeWeapon() 
    {
        anim.Play("HideWeaponIndicator");
        yield return new WaitForSeconds(0.25f);

        selectedWeaponAvailability.localScale = new Vector3(selectedWeapon.availability / 100f, 1, 1);        
        selectedWeaponTxt.text = itemNames[selectedWeapon.textID];
        selectedWeaponImg.sprite = Resources.Load(@"UserInterface\WeaponIndicator\Weapon_" + weapons[pointer].name, typeof(Sprite)) as Sprite;
        leo.SwitchWeapon(selectedWeapon.name);

        anim.Play("ShowWeaponIndicator");
    }

    private void UpdateWeapons() 
    {
        weapons = new List<Leo.Inventory.Item>();
        weapons.Add(null);

        foreach (Leo.Inventory.Item item in leo.inventory.items) 
        {
            if (GeneralVariables.IsWeapon(item.name))
                weapons.Add(item);
        }
    }

    public void ShowIndicator() 
    {
        anim.Play("ShowWeaponIndicator");
    }

    public void HideIndicator() 
    {
        anim.Play("HideWeaponIndicator");
    }
    public void OnClickIndiactor() 
    {
        ChangeWeapon();
    }
}
