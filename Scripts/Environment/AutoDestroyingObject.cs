using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyingObject : MonoBehaviour
{
    public float destroyTime;

    private void Start() 
    {
        Invoke("Destroy", destroyTime);
        GetComponent<Animation>().Play("ADO_Destroy1");
    }

    private void Destroy() 
    {
        Destroy(gameObject);
    }
}
