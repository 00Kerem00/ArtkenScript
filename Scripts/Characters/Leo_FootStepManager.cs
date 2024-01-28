using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leo_FootStepManager : MonoBehaviour
{
    public AudioSource leoFootStepSource;
    public AudioClip[] footStepSounds;
    public string currentGround;
    public Transform walkSighter;

    public void OnFootStep() 
    {
        CheckGround();
        int index = Random.Range(0, footStepSounds.Length);
        leoFootStepSource.PlayOneShot(footStepSounds[index]);
    }

    public void CheckGround() 
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(walkSighter.position + new Vector3(0, 1), Vector2.down, 1);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.tag == "SoundedGround")
            {
                string floorType = hit.collider.gameObject.GetComponent<DataHolder>().data[0];
                if (currentGround != floorType)
                {
                    currentGround = floorType;
                    ChangeFootStepSounds(currentGround);
                }
            }
        }
    }
    public void ChangeFootStepSounds(string groundTag)
    {
        Debug.Log(groundTag);
        switch (groundTag) 
        {
            case "GrassFloor": footStepSounds = new AudioClip[3];
                footStepSounds[0] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\GrassStep_0");
                footStepSounds[1] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\GrassStep_1");
                footStepSounds[2] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\GrassStep_2"); break;
            case "ConcreteFloor": footStepSounds = new AudioClip[3];
                footStepSounds[0] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ConcreteStep_0");
                footStepSounds[1] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ConcreteStep_1");
                footStepSounds[2] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ConcreteStep_2"); break;
            case "SewageFloor": footStepSounds = new AudioClip[4];
                footStepSounds[0] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\SewageStep_0");
                footStepSounds[1] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\SewageStep_1");
                footStepSounds[2] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\SewageStep_2");
                footStepSounds[3] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\SewageStep_3"); break;
            case "ArtkenDiggingFloor": footStepSounds = new AudioClip[4];
                footStepSounds[0] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ArtkenDiggingStep_0");
                footStepSounds[1] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ArtkenDiggingStep_1");
                footStepSounds[2] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ArtkenDiggingStep_2");
                footStepSounds[3] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\ArtkenDiggingStep_3"); break;
            case "U0Office": footStepSounds = new AudioClip[4];
                footStepSounds[0] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\U0OfficeStep_0");
                footStepSounds[1] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\U0OfficeStep_1");
                footStepSounds[2] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\U0OfficeStep_2");
                footStepSounds[3] = Resources.Load<AudioClip>(@"Audio\Character\FootSteps\U0OfficeStep_3"); break;
        }
    }
}
