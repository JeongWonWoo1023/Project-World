using UnityEngine;
using UnityEngine.Events;

public interface IItem
{
    void UseItem();
    void DropItem(Vector3 dropPos, Quaternion dorpRot);
    void UpgradeItem();
}

public class Item : MonoBehaviour, IItem
{
    public UnityAction activeAction; // 액티브 효과
    public UnityAction passiveAction; // 패시브 효과

    protected IItem itemInterface;

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

    public void Use() => itemInterface?.UseItem();
    public void Drop(Vector3 dropPos, Quaternion dorpRot) => itemInterface?.DropItem(dropPos, dorpRot);
    public void Upgrade() => itemInterface?.UpgradeItem();

    public virtual void UseItem()
    {
        // 아이템 사용 및 장착
        if (!Info.IsUsable)
        {
            Debug.Log($"{Info.Name} : 사용 할 수 없는 아이템입니다");
            return;
        }
    }

    public virtual void DropItem(Vector3 dropPos, Quaternion dorpRot)
    {
        // 아이템 드랍
        if (!Info.IsDropable)
        {
            Debug.Log($"{Info.Name} : 필드에 드랍되지 않는 아이템입니다");
            return;
        }
        if (Info.ItemObject == null)
        {
            Debug.Log($"{Info.Name} : 드랍 오브젝트를 바인딩 해주세요");
            return;
        }
        GameObject dropObj = ObjectPool.Instance.PopObject(Info.ItemObject.name, dropPos, dorpRot);
        // 드랍 연출 ( 물리 로직 구현 필요 )
    }

    public virtual void UpgradeItem()
    {
        // 아이템 업그레이드
        if (!Info.IsUpgradable)
        {
            Debug.Log($"{Info.Name} : 업그레이드 할 수 없는 아이템입니다");
            return;
        }
        if (UpgradeRequireItem[0] == null || UpgradeRequireItem.Length == 0)
        {
            Debug.Log($"{Info.Name} : 업그레이드 요구 아이템을 바인딩 해주세요");
            return;
        }
    }
}
