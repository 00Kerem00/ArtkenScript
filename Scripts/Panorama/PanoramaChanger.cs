using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanoramaChanger : MonoBehaviour
{
    public Panorama nextPanorama;
    public Transform leo, nextPanoramaStartLocation;

    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.name == "Leo") 
        {
            Camera.main.GetComponent<MultiLayer>().currentStartPosition = nextPanoramaStartLocation.position;
            Camera.main.GetComponent<MultiLayer>().currentPanorama = nextPanorama;
            PPR_Manager.lastStartPosition = nextPanoramaStartLocation;
            StaticCoroutine.DoCoroutine(PortLeoToStartPosition());
        }
    }

    private IEnumerator PortLeoToStartPosition()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        leo.position = nextPanoramaStartLocation.position;
    }
}
