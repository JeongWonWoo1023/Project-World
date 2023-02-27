using System.Collections.Generic;
using UnityEngine;

public class NearObjectDetector : DetectorSensorRequire
{
    [field: SerializeField] public Inventory Inventory { get; private set; } // 인벤토리

    public List<Item> ItemList { get; private set; } = new List<Item>(); // 플레이어 근처에 있는 아이템 리스트
    public List<NPC> NPCList { get; private set; } = new List<NPC>(); // 플레이어 근처에 있는 NPC 리스트

    public void ViewItemUI()
    {
        // 아이템 오브젝트에 가까이 가면 아이템 UI출력

    }

    private void OnTriggerEnter(Collider other)
    {
        ItemEnter(other);
        NPCEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ItemExit(other);
        NPCExit(other);
    }

    private void ItemEnter(Collider other)
    {
        Item item = GetComponenetFromLayer<Item>(other, "Item");
        if (IsItemNull(item))
        {
            return;
        }
        ItemList.Add(item);
    }

    private void NPCEnter(Collider other)
    {
        NPC npc = GetComponenetFromLayer<NPC>(other, "NPC");
        if (npc == null)
        {
            return;
        }
        NPCList.Add(npc);
    }

    private void ItemExit(Collider other)
    {
        Item item = GetComponenetFromLayer<Item>(other, "Item");
        if (IsItemNull(item))
        {
            return;
        }
        foreach (Item eliment in ItemList)
        {
            if (item.Equals(eliment))
            {
                ItemList.Remove(eliment);
            }
        }
    }

    private void NPCExit(Collider other)
    {
        NPC npc = GetComponenetFromLayer<NPC>(other, "NPC");
        if (npc == null)
        {
            return;
        }
        foreach (NPC eliment in NPCList)
        {
            if (npc.Equals(eliment))
            {
                NPCList.Remove(eliment);
            }
        }
    }

    private bool IsItemNull(Item item)
    {
        if (item == null)
        {
            return true;
        }
        if (item.IsInstalled)
        {
            return true;
        }
        return false;
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
        if(item.SpawnPoint != null)
        {
            item.SpawnPoint.IsGet = true;
        }
        ItemList.Remove(item);
    }

    public void GetItem(Item item, int count = 1)
    {
        // 아이템 수동 지급
        Inventory.AddItem(item, count);
    }
}
