using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Transform tf;
    public float smooth;
    public float smoothScale;
    public float extraSmoothScale;
    public float currentPanoramaYPos = 0;

    public float minLoc = -15;
    private float _minLoc;
    public float maxLoc = 15;
    private float _maxLoc;

    private float extraSmoothMinPos, extraSmoothMaxPos;

    void FixedUpdate()
    {
        CheckExtraSmooth();
        Vector2 desiredPosition = target.position;
        float xPos = Vector2.Lerp(tf.position, desiredPosition, smooth).x;
        tf.position = new Vector3(LimitedXPosition(xPos), currentPanoramaYPos, -10);
    }

    public void SetCameraPath() 
    {
        float width = Screen.width, height = Screen.height;
        float offset = (width / height * Camera.main.orthographicSize);

        _minLoc = minLoc + offset;
        _maxLoc = maxLoc - offset;

        extraSmoothMinPos = _minLoc + 2;
        extraSmoothMaxPos = _maxLoc - 2;
    }
    private void CheckExtraSmooth() 
    {
//        Debug.Log("campos: " + tf.position.x + ", minpos: " + extraSmoothMinPos + ", maxpos: " + extraSmoothMaxPos);
        if (tf.position.x < extraSmoothMinPos || tf.position.x > extraSmoothMaxPos)
            smooth = extraSmoothScale;
        else
            smooth = smoothScale;
    }
    private float LimitedXPosition(float xPosition) 
    {
        if (xPosition < _minLoc) xPosition = _minLoc;
        else if (xPosition > _maxLoc) xPosition = _maxLoc;
        return xPosition;
    }
}
