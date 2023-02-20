using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    private RectTransform _parant;
    private Transform _traking;

    public void Initialize(RectTransform perant, Transform trakingRect)
    {
        _parant = perant;
        _traking = trakingRect;
        transform.SetParent(_parant);
    }

    private void OnDisable()
    {
        ObjectPool.Instance.PushPool(gameObject);
        CancelInvoke();
    }

    private void Update()
    {
        Vector3 direction = _traking.position - Camera.main.transform.position;
        if(Vector3.Dot(Camera.main.transform.forward, direction) > 0.0f)
        {
            transform.position = Camera.main.WorldToScreenPoint(_traking.position);
        }
    }
}
