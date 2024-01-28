using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static bool databaseChecked = false;

    #region InvestigatingObject
    public static bool[] GetDeactivatingObjectStates(string investigatingObject) 
    {
        CheckDatabase();

        List<bool> result = new List<bool>();
        string path = Application.persistentDataPath + "\\InvestigatingObjects\\" + investigatingObject + "\\DeactivatingObjectStates.txt";
        StreamReader reader = new StreamReader(path);

        string item;
        while ((item = reader.ReadLine()) != null) 
        {
            result.Add(bool.Parse(item));
        }

        reader.Close();
        return result.ToArray();
    }
    public static void SetDeactivatingObjectStates(string investigatingObject, bool[] values) 
    {
        CheckDatabase();

        string path = Application.persistentDataPath + "\\InvestigatingObjects\\" + investigatingObject + "\\DeactivatingObjectStates.txt";
        StreamWriter writer = new StreamWriter(path);
        foreach (bool value in values)
            writer.WriteLine(value);
        writer.Close();
    }

    public static Vector2[] GetMovingObjectPositions(string investigateObject) 
    {
        CheckDatabase();

        List<Vector2> result = new List<Vector2>();
        string path = Application.persistentDataPath + "\\InvestigatingObjects\\" + investigateObject + "\\MovingObjectPositions.txt";
        StreamReader reader = new StreamReader(path);

        float x, y;
        string item;
        while ((item = reader.ReadLine()) != null) 
        {
            x = float.Parse(item);
            y = float.Parse(reader.ReadLine());

            result.Add(new Vector2(x, y));
        }

        reader.Close();
        return result.ToArray();
    }
    public static void SetMovingObjectPositions(string investigatingObject, Vector2[] values) 
    {
        CheckDatabase();

        string path = Application.persistentDataPath + "\\InvestigatingObjects\\" + investigatingObject + "\\MovingObjectPositions.txt";
        StreamWriter writer = new StreamWriter(path);
        foreach (Vector2 value in values) 
        {
            writer.WriteLine(value.x);
            writer.WriteLine(value.y);
        }
        writer.Close();
    }

    public static string[] GetTextsOfInvestigationObject(string investigatingObject) 
    {
        CheckDatabase();

        List<string> result = new List<string>();
        string path = Application.persistentDataPath + "\\InvestigatingObjects\\" + investigatingObject + "\\Texts.txt";
        StreamReader reader = new StreamReader(path);

        string item;
        while ((item = reader.ReadLine()) != null) 
        {
            result.Add(item);
        }

        reader.Close();
        return result.ToArray();
    }
    public static void SetTextsOfInvestigatingObject(string investigatingObject, string[] values) 
    {
        CheckDatabase();

        string path = Application.persistentDataPath + "\\InvestigatingObjects\\" + investigatingObject + "\\Texts.txt";
        StreamWriter writer = new StreamWriter(path);
        foreach (string s in values)
            writer.WriteLine(s);
        writer.Close();
    }
    #endregion
    public static Leo.Inventory GetLeoInventory() 
    {
        CheckDatabase();
        
        Leo.Inventory result = new Leo.Inventory();
        List<Leo.Inventory.Item> items = new List<Leo.Inventory.Item>();
        string path = Application.persistentDataPath + "\\LeoInventory.txt";
        StreamReader reader = new StreamReader(path);

        string text;
        Leo.Inventory.Item item;
        while ((text = reader.ReadLine()) != null) 
        {
            item = new Leo.Inventory.Item(text, int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()), int.Parse(reader.ReadLine()));
            items.Add(item);
        }

        result.items = items;
        reader.Close();
        return result;
    }
    public static void SetLeoInventory(Leo.Inventory inventory) 
    {
        CheckDatabase();

        string path = Application.persistentDataPath + "\\LeoInventory.txt";
        StreamWriter writer = new StreamWriter(path);

        foreach(Leo.Inventory.Item item in inventory.items)
        {
            writer.WriteLine(item.name);
            writer.WriteLine(item.count);
            writer.WriteLine(item.availability);
            writer.WriteLine(item.textID);
        }
        writer.Close();
    }
    public static void CheckDatabase() 
    {
        if (!databaseChecked)
        {
            Debug.Log("a");
            CheckInvestigatingObjectDatabase("InvestigatingTable");
            CheckInvestigatingObjectDatabase("InvestigatingEncryptedDoorUI");
            CheckInvestigatingObjectDatabase("InvestigatingFireplace");
            CheckInvestigatingObjectDatabase("InvestigatingTable_U0");
            CheckInvestigatingObjectDatabase("U0Cabinet_0");
            CheckInvestigatingObjectDatabase("U0Cabinet_1");
            CheckInvestigatingObjectDatabase("Inves_MazeBox");
            CheckInvestigatingObjectDatabase("Inves_FenceDoor");
            CheckInvestigatingObjectDatabase("InvestigatingSpinningTable");
            
            if (!File.Exists(Application.persistentDataPath + "\\LeoInventory.txt"))
                File.Create(Application.persistentDataPath + "\\LeoInventory.txt");
            databaseChecked = true;
            Debug.Log("Databes Checked");
        }        
    }
    private static void CheckInvestigatingObjectDatabase(string objectName) 
    {
        if (!File.Exists(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName + "\\DeactivatingObjectStates.txt"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName);
            File.Create(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName + "\\DeactivatingObjectStates.txt");
        }
        if (!File.Exists(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName + "\\MovingObjectPositions.txt"))
            File.Create(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName + "\\MovingObjectPositions.txt");

        if (!File.Exists(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName + "\\Texts.txt"))
            File.Create(Application.persistentDataPath + "\\InvestigatingObjects\\" + objectName + "\\Texts.txt");

    }

    public static void DeleteDatabase() 
    {
        if (File.Exists(Application.persistentDataPath + "\\LeoInventory.txt"))
            File.Delete(Application.persistentDataPath + "\\LeoInventory.txt");
        if (Directory.Exists(Application.persistentDataPath + "\\InvestigatingObjects"))
            Directory.Delete(Application.persistentDataPath + "\\InvestigatingObjects", true);
    }
}
