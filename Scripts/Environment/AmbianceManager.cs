using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceManager : MonoBehaviour
{
    private static AmbianceManager instance;
    public static AmbianceManager GetInstance() { if (instance == null) instance = GameObject.Find("AmbianceManager").GetComponent<AmbianceManager>(); return instance; }
    public static void SetAmbiance(string panoramaName) { GetInstance().setAmbiance(panoramaName); }

    public AudioSource ambiance;
    public string currentSound;

    public void setAmbiance(string panoramaName)
    {
        switch (GeneralVariables.scene)
        {
            case GeneralVariables.Scene.Underground_0: SetSoundOfUnderground_0(panoramaName); break;
        }
    }

    public void SetSoundOfUnderground_0(string panoramaName) 
    {
        switch (panoramaName)
        {
            case "Underground_0_Panorama_0": SetAmbianceSound("Night"); break;
            case "Underground_0_Panorama_1": SetAmbianceSound("Night"); break;
            case "Underground_0_Panorama_2": SetAmbianceSound("U0UpStorageAmbiance"); break;
            case "Underground_0_Panorama_3": SetAmbianceSound("Sewage"); break;
            case "Underground_0_Panorama_4": SetAmbianceSound("Sewage"); break;
            case "Underground_0_Panorama_5": SetAmbianceSound("ArtkenDiggings"); break;
            case "Underground_0_Panorama_6": SetAmbianceSound("ArtkenDiggings"); break;
            case "Underground_0_Panorama_7": SetAmbianceSound("U0OfficeAmbiance"); break;
            case "Underground_0_Panorama_8": SetAmbianceSound("U0OfficeAmbiance"); break;
            case "Underground_0_Panorama_9": SetAmbianceSound("FactoryAmbiance"); break;
            case "Underground_0_Panorama_10": SetAmbianceSound("ArtkenDiggings"); break;
            case "Underground_0_Panorama_11": SetAmbianceSound("ArtkenDiggings"); break;
        }
    }
    public void SetAmbianceSound(string clipName) 
    {
        if (clipName != currentSound) 
        {
            ambiance.Stop();
            ambiance.PlayOneShot(Resources.Load<AudioClip>(@"Audio\Environment\" + clipName));
            currentSound = clipName;
        }
    }
}
