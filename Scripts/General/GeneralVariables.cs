using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralVariables : MonoBehaviour
{
    public enum Language { TR, EN }
    public static Language language = Language.TR;
    public enum Scene { MainMenu, Factory, Underground_0, Underground_1, RailwayStation }
    public static Scene scene = Scene.Factory;

    public static Guard[] GetAllGuards() 
    {
        Guard[] result;
        GameObject[] guards = GameObject.FindGameObjectsWithTag("Antagonist");
        
        result = new Guard[guards.Length];
        for (int i = 0; i < guards.Length; i++)
            result[i] = guards[i].GetComponent<Guard>();

        return result;
    }
    public static int GetDamageUnitOfWeapon(string weaponName) 
    {
        switch (weaponName) 
        {
            case "Axe": return 15;
            default: Debug.LogError("Weapon Name Is Invalid! Name: " + weaponName); return 0;
        }
    }
    public static string[] weapons = { "Arrow", "Zippo", "Axe", "Dynamite", "Gun", "MazeBox" };
    public static WeaponTransformValues GetTransformValuesOfWeapon(string weaponName)
    {
        switch (weaponName)
        {
            case "Arrow": return new WeaponTransformValues(new Vector3(2.23f, 0.16f, 0), new Vector3(1, 1, 1), 190.05f);
            case "Zippo": return new WeaponTransformValues(new Vector3(2.23f, 0.16f, 0), new Vector3(1, 1, 1), 190.05f);
            case "Axe": return new WeaponTransformValues(new Vector3(2.23f, 0.16f, 0), new Vector3(1, 1, 1), 190.05f);
            case "Dynamite": return new WeaponTransformValues(new Vector3(2.18f, 0.11f, 0), new Vector3(0.55f, 0.55f, 1), 113.85f);
            case "Gun": return new WeaponTransformValues(new Vector3(2.79f, 0.36f, 0), new Vector3(0.23f, 0.23f, 1), 3.37f);
            case "Crowbar": return new WeaponTransformValues(new Vector3(2.08f, 0.67f, 0), new Vector3(1, 1, 1), 335.934f);
            default: return new WeaponTransformValues(new Vector3(2.23f, 0.16f, 0), new Vector3(1, 1, 1), 190.05f);
        }
    }
    private static string[] throwWeapons = { "Arrow" };
    private static string[] lightSourceWeapons = { "Zippo" };
    private static string[] specialItems = { "Dynamite", "MazeBox", "AlarmControl" };
    public static Leo.AttackType GetAttackTypeOfWeapon(string weaponName)
    {
        if (weaponName == "None")
            return Leo.AttackType.None;
        foreach (string weapon in throwWeapons) { if (weaponName == weapon) return Leo.AttackType.Throw; }
        foreach (string weapon in lightSourceWeapons) { if (weaponName == weapon) return Leo.AttackType.UseZippo; }
        foreach (string weapon in specialItems) { if (weaponName == weapon) return Leo.AttackType.SpecialUsage; }
        if (weaponName == "Gun") { return Leo.AttackType.Shoot; }
        return Leo.AttackType.Hit;
    }
    public static bool IsWeapon(string itemName) { foreach (string weapon in weapons) if (itemName == weapon) return true; return false; }

    public static float maxInteractionDistance = 5;

    public static RaycastHit2D[] ReverseHitArray(RaycastHit2D[] hits) 
    {
        List<RaycastHit2D> result = new List<RaycastHit2D>();
        foreach (RaycastHit2D hit in hits)
            result.Add(hit);
        result.Reverse();
        return result.ToArray();
    }

    public class WeaponTransformValues 
    {
        public WeaponTransformValues(Vector3 position, Vector3 scale, float zRotation)
        {
            this.position = position;
            this.scale = scale;
            this.zRotation = zRotation;
        }

        public Vector3 position;
        public Vector3 scale;
        public float zRotation;
    }

    public static string[] groundTags = { "GrassFloor", "ConcreteFloor", "SewageFloor", "ArtkenDiggingFloor" };
    public static bool IsVoicedGround(string groundName) 
    {
        foreach (string tag in groundTags)
            if (groundName == tag)
                return true;
        return false;
    }
}
