using UnityEngine;

public class EnemyStatusUI : MonoBehaviour
{
    [field: SerializeField] public Gauge HPBar { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text Level { get; private set; }

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
        transform.position = Camera.main.WorldToScreenPoint(_traking.position);
    }
}
