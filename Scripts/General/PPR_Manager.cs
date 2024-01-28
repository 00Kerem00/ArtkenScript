using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PPR_Manager : MonoBehaviour
{
    public static Transform lastStartPosition;

    public static void OnDiedLeo(Collider2D trigger) 
    {
        StaticCoroutine.DoCoroutine(onDiedLeo(trigger));
    }
    private static IEnumerator onDiedLeo(Collider2D trigger) 
    {
        Animation panoramaPassingImage = GameObject.Find("PanoramaPassingImage").GetComponent<Animation>();
        trigger.enabled = false;

        yield return new WaitForSeconds(2);

        trigger.enabled = true;
        panoramaPassingImage.Play("FrontImage_Closing");
        GameObject.Find("Leo").GetComponent<Transform>().position = lastStartPosition.position;
        panoramaPassingImage.Play("FrontImage_OnChangedPanorama");
    }
}
