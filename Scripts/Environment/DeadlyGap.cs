using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyGap : MonoBehaviour
{
    public bool delay;
    public bool once;
    public float delayTime;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Leo")
        {
            GetComponent<BoxCollider2D>().enabled = false;
            if (delay || !once)
            {
                Invoke("OnDiedLeo", delayTime);
                delay = false;
            }
            else OnDiedLeo();
        }
    }

    private void OnDiedLeo() 
    {
        StartCoroutine(onDiedLeo());
    }

    private IEnumerator onDiedLeo() 
    {
        Animation panoramaPassingImage = GameObject.Find("PanoramaPassingImage").GetComponent<Animation>();
        Transform leo = GameObject.Find("Leo").GetComponent<Transform>();

        panoramaPassingImage.Play("FrontImage_Closing");
        yield return new WaitForSeconds(2);

        leo.position = PPR_Manager.lastStartPosition.position;
        Camera.main.GetComponent<Transform>().position = new Vector3(leo.position.x, 0, -10);
        GetComponent<BoxCollider2D>().enabled = true;
        panoramaPassingImage.Play("FrontImage_OnChangedPanorama");
    }
}
