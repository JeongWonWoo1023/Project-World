using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNameUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text nameText;

    private RectTransform _parant;
    private Transform _traking;

    public void Initialize(RectTransform perant, Transform trakingRect, string name)
    {
        _parant = perant;
        _traking = trakingRect;
        nameText.text = name;
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
        if (Vector3.Dot(Camera.main.transform.forward, direction) > 0.0f)
        {
            transform.position = Camera.main.WorldToScreenPoint(_traking.position);
        }
    }
}
