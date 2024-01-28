using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidewall : MonoBehaviour
{
    public Transform LeftsideWall, RightsideWall;

    public float minPosOfPanorama, maxPosOfPanorama;
    public float panoramaCenter;
    public float depth;

    private float cameraMinPos, cameraMaxPos;
    private float cameraRange;
    private float sidewallLayerMinPos, sidewallLayerMaxPos;
    private float rightSWDistanceFromSWMaxPos, leftSWDistanceFromSWMinPos;

    private void Start() 
    {
        cameraMaxPos = maxPosOfPanorama - GetCameraHalfWide();
        cameraMinPos = minPosOfPanorama + GetCameraHalfWide();

        sidewallLayerMaxPos = (cameraMaxPos - panoramaCenter) / depth * -1; // (101)
        sidewallLayerMinPos = (panoramaCenter - cameraMaxPos) / depth * -1;

        sidewallLayerMaxPos += cameraMaxPos;
        sidewallLayerMinPos += cameraMinPos;

        rightSWDistanceFromSWMaxPos = maxPosOfPanorama - sidewallLayerMaxPos;
        leftSWDistanceFromSWMinPos = sidewallLayerMinPos - minPosOfPanorama;

        if (RightsideWall != null)
            RightsideWall.position = new Vector3(panoramaCenter + rightSWDistanceFromSWMaxPos, RightsideWall.position.y, RightsideWall.position.z);
        if (LeftsideWall != null)
            LeftsideWall.position = new Vector3(panoramaCenter - leftSWDistanceFromSWMinPos, LeftsideWall.position.y, LeftsideWall.position.z);
    }

    private float GetCameraHalfWide() 
    {
        float height = Screen.height, width = Screen.width;
        return (width / height * Camera.main.orthographicSize);
    }
}
