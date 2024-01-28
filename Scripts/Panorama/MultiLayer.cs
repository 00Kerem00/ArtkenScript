using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLayer : MonoBehaviour
{
    public Transform camera;

    public Vector3 currentStartPosition;
    public Panorama defaultPanorama;
    public Panorama currentPanorama 
    {
        set
        {
            if (_currentPanorama != null)
            {
                AmbianceManager.SetAmbiance(value.gameObject.name);
                currentPanorama.DeactivateIL_Manager();

                if (_currentPanorama.lightLevel > value.lightLevel)
                    value.halfDark.Play("HalfDark");
                else if (_currentPanorama.lightLevel < value.lightLevel)
                    value.halfDark.Play("HalfLight");

                UnableOldPanorama(_currentPanorama);
                if (_currentPanorama.panoramaExitEvent != null)
                    StartCoroutine(_currentPanorama.panoramaExitEvent);
            }

            _currentPanorama = value;
            if (value.panoramaEnterEvent != null) StartCoroutine(value.panoramaEnterEvent);
            OnChangedPanorama();
        }
        get { if (_currentPanorama != null) return _currentPanorama; else return defaultPanorama; } 
    }
    private Panorama _currentPanorama;

    private int layerCount;
    private Transform[] layerLocations;
    private float[] layerDepths;
    private Transform staticLayer;
    private float panoramaCenter;

    public Animation frontLayer;

    private void Start()    
    {
        currentPanorama = defaultPanorama;
    }

    private void LateUpdate() 
    {
        // Sets layer positions according to ratio of depth and distance between camera position and center.
        for (int i = 0; i < layerCount; i++)
        {
            float xPos, zPos;          
            xPos = (camera.localPosition.x - panoramaCenter) / layerDepths[i] * -1;
            zPos = layerLocations[i].localPosition.z;
            layerLocations[i].localPosition = new Vector3 (xPos, -5, zPos);
        }
    }

    private void OnChangedPanorama() 
    {
        // Sets limit of camera movement.
        camera.gameObject.GetComponent<CameraFollow>().minLoc = currentPanorama.minCamLoc;
        camera.gameObject.GetComponent<CameraFollow>().maxLoc = currentPanorama.maxCamLoc;
        camera.gameObject.GetComponent<CameraFollow>().SetCameraPath();
        camera.gameObject.GetComponent<Transform>().position = new Vector3(currentStartPosition.x, currentStartPosition.y, -10);

        // Creates arrays in length that the layer count of current panorama.
        layerCount = currentPanorama.layers.Length;
        layerLocations = new Transform[layerCount];
        layerDepths = new float[layerCount];    

        // Equalizes values of new arrays as values of current panorama. And sets parent of layers as camera.
        for (int i = 0; i < layerCount; i++)
        {
            layerLocations[i] = currentPanorama.layers[i].gameObject.GetComponent<Transform>();
            layerLocations[i].SetParent(camera);
            layerDepths[i] = currentPanorama.layers[i].depth;
        }

        // Sets center point of camera path as position of the current panorama.
        staticLayer = currentPanorama.staticLayer;
        panoramaCenter = staticLayer.position.x;
        camera.gameObject.GetComponent<CameraFollow>().currentPanoramaYPos = currentPanorama.yPos;

        // Activates inter layer manager of the current panorama.
        currentPanorama.ActivateIL_Manager();

        //Plays animation of the front image.
        frontLayer.Play("FrontImage_OnChangedPanorama");

        Panorama.EnablePanorama(currentPanorama);
    }
    private void UnableOldPanorama(Panorama oldPanorama) 
    {
        Panorama.UnablePanorama(oldPanorama);
    }
}
