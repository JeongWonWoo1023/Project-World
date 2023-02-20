using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorSensorRequire : MonoBehaviour
{
    protected T GetComponenetFromLayer<T>(Collider other, string layerName)
    {
        if (!CompareLayer(other, layerName))
        {
            return default(T);
        }
        return other.GetComponent<T>();
    }

    private bool CompareLayer(Collider other, string targetLayerName)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(targetLayerName))
        {
            return true;
        }
        return false;
    }
}
