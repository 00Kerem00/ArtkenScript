using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panorama : MonoBehaviour
{
    public float yPos;
    public Layer[] layers;
    public Transform staticLayer;
    public float minCamLoc, maxCamLoc;
    public GameObject panoramaChangers;
    public Animation halfDark;
    public int lightLevel;
    public GameObject[] lightSprites;
    public SpriteMask[] lightMasks;

    public IEnumerator panoramaEnterEvent, panoramaExitEvent;

    public InterLayerManager interLayerManager;

    public void DeactivateIL_Manager() 
    {
        if (interLayerManager != null)
            interLayerManager.CloseIL_Manager();
    }
    public void ActivateIL_Manager() 
    {
        if (interLayerManager != null) interLayerManager.ActiveIL_Manager();
    }

    #region Statics
    /// <summary>
    /// Sets parent of layers as panorama, equalizes positions as 0, 0, 0 and deactivates the panorama changers.
    /// </summary>
    /// <param name="panorama"></param>
    public static void UnablePanorama(Panorama panorama) 
    {
        int layerCount = panorama.layers.Length;
        Transform tfOldPanorama = panorama.gameObject.GetComponent<Transform>();

        for (int i = 0; i < layerCount; i++)
        {
            Transform tf = panorama.layers[i].GetComponent<Transform>();
            tf.SetParent(tfOldPanorama);
            tf.localPosition = new Vector3(0, 0, tf.localPosition.z);
        }

        foreach (GameObject ls in panorama.lightSprites)
            ls.SetActive(false);
        foreach (SpriteMask lm in panorama.lightMasks)
            lm.enabled = false;

//        panorama.panoramaChangers.SetActive(false);
    }

    /// <summary>
    /// Activates panorama changers of panorama
    /// </summary>
    /// <param name="panorama"></param>
    public static void EnablePanorama(Panorama panorama) 
    {
        foreach (GameObject ls in panorama.lightSprites)
            ls.SetActive(true);
        foreach (SpriteMask lm in panorama.lightMasks)
            lm.enabled = true;

//        panorama.panoramaChangers.SetActive(true);
    }
    #endregion
}
