using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    public UnityAction activeAction; // 액티브 효과
    public UnityAction passiveAction; // 패시브 효과
    [field: SerializeField] public FieldItemPoint SpawnPoint { get; set; }

    [field: SerializeField] public bool IsInstalled { get; set; } = false;
    [field: SerializeField] public Item[] UpgradeRequireItem { get; private set; }  // 업그레이드 요구 아이템

    [SerializeField] private ItemInfo _info;
    [SerializeField] protected ItemStatusData[] _status;

    private int _count;

    public ItemInfo Info
    {
        get => _info;
        set => _info = value;
    }

    public ItemStatusData[] Status
    {
        get => _status;
        set => _status = value;
    }

    public int Count
    {
        get => _count;
        set => _count = value <= 0 ? 0 : value;
    }

    private void OnDisable()
    {
        ObjectPool.Instance.PushPool(gameObject);
        CancelInvoke();
    }

    public virtual void UseItem()
    {
        // 아이템 사용 및 장착
        if (!Info.IsUsable)
        {
            Debug.Log($"{Info.Name} : 사용 할 수 없는 아이템입니다");
            return;
        }
    }
}
