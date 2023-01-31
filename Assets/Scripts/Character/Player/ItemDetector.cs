using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemDetector : MonoBehaviour
{
    [field: SerializeField] public Inventory Inventory { get; private set; } // 인벤토리

    public List<Item> ItemList { get; private set; } // 플레이어 근처에 있는 아이템 큐

    private void Awake()
    {
        ItemList = new List<Item>();
    }

    public void ViewItemUI()
    {
        // 아이템 오브젝트에 가까이 가면 아이템 UI출력

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            return;
        }
        Item item = other.GetComponent<Item>();
        if(item.IsInstalled)
        {
            return;
        }

        ItemList.Add(item);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Item") || ItemList.Count == 0)
        {
            return;
        }
        Item installItem = other.GetComponent<Item>();
        if (installItem.IsInstalled)
        {
            return;
        }
        foreach (Item item in ItemList)
        {
            if(other.GetComponent<Item>() == item)
            {
                ItemList.Remove(item);
            }
        }
    }

    public void GetItem()
    {
        // 아이템 능동 지급
        if (ItemList.Count == 0)
        {
            return;
        }

        Item item = ItemList[0];
        if (item.Info.DropValueRange.x == item.Info.DropValueRange.y)
        {
            Inventory.AddItem(item, (int)item.Info.DropValueRange.x);
        }
        else
        {
            int count = Random.Range((int)item.Info.DropValueRange.x, (int)item.Info.DropValueRange.y + 1);
            Inventory.AddItem(item, count);
        }
        item.gameObject.SetActive(false);
        ItemList.Remove(item);
    }

    public void GetItem(Item item, int count = 1)
    {
        // 아이템 수동 지급
        Inventory.AddItem(item, count);
    }
}
