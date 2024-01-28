using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterLayerManager : MonoBehaviour
{
    public InterLayer[] interLayers;

    private void LateUpdate() 
    {
        foreach (InterLayer il in interLayers)
        {
            float xScale = (il.point1.position.x - il.point0.position.x) / il.maxPointDistance;
            if (xScale > 0)
                il.tf.localScale = new Vector3(xScale, 1, 1);
            else
                il.tf.localScale = new Vector3(0, 1, 1);        
        }
    }

    public void ActiveIL_Manager() { gameObject.GetComponent<InterLayerManager>().enabled = true; }
    public void CloseIL_Manager() { gameObject.GetComponent<InterLayerManager>().enabled = false; }
}
