using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItemPoint : MonoBehaviour
{
    [field: SerializeField] public Item Item { get; set; }

    private FieldItemGroup group;
    private bool _isGet;
    public bool IsGet
    {
        get => _isGet; 
        set
        {
            _isGet = value;
            if (IsGet)
            {
                Item.gameObject.SetActive(false);
                if (IsGet)
                {
                    group.IsStartCoolTIme = true;
                }
            }
        }
    }

    private void Start()
    {
        group = GetComponentInParent<FieldItemGroup>();
    }

    public void Enable()
    {
        // 아이템 불러오기
        if(IsGet)
        {
            return;
        }
        Item = ObjectPool.Instance.PopObject<Item>(group.ItemPrefab.name, transform.position, transform.rotation);
        Item.SpawnPoint = this;
    }

    public void Disable()
    {
        // 아이템 비활성화
        Item.gameObject.SetActive(false);
    }

    public Item GetItem()
    {
        // 아이템 습득
        IsGet = true;
        return Item;
    }
}
