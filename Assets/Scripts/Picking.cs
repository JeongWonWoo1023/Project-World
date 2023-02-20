using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Picking : MonoBehaviour
{
    public float radius;
    public bool isDetect;
    public SphereCollider collider;

    private void OnValidate()
    {
        collider.radius = radius;
    }

    private void OnDisable()
    {
        ObjectPool.Instance.PushPool(gameObject);
        CancelInvoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isDetect = true;
        }
    }
}
