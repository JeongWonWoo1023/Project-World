using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    public ParticleSystem particle;
    public SphereCollider range;

    private void OnDisable()
    {
        ObjectPool.Instance.PushPool(gameObject);
        CancelInvoke();
    }

    private void OnEnable()
    {
        StartCoroutine(Disable());
    }

    private IEnumerator Disable()
    {
        yield return new WaitForSeconds(particle.duration);
        gameObject.SetActive(false);
    }
}
