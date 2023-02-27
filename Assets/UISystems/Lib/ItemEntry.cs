using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ItemEntry : MonoBehaviour
{
    public Transform slotParent;
    public ItemDescription description;
    protected List<ItemSlot> slots = new List<ItemSlot>();
    protected UnityAction slotButtonEvent;
    private string slotResourcePath = "Prefabs/UI/Slot";

    public virtual void AddItem(Item item, int price, int count = 1)
    {
        // 아이템 추가
        ItemSlot targetSlot = FindSlot(item);
        targetSlot.ItemCount += count;
    }

    public virtual void RemoveItem(Item item, int price, int count = 1)
    {
        // 아이템 제거
        ItemSlot targetSlot = FindSlot(item);
        targetSlot.ItemCount -= count;
        RemoveSlot(targetSlot);
    }

    public ItemSlot FindSlot(Item item)
    {
        // 아이템 탐색 후 아이템이 있는 슬롯 반환
        foreach (ItemSlot slot in slots)
        {
            if (slot.Item.Info.Name.Equals(item.Info.Name))
            {
                return slot;
            }
        }
        return AddSlot(item);
    }

    public Item FindItem(Item item)
    {
        // 아이템 탐색 후 해당 아이템 반환 없을 경우 null 반환
        foreach (ItemSlot slot in slots)
        {
            if (slot.Item.Info.Name.Equals(item.Info.Name))
            {
                return slot.Item;
            }
        }
        return null;
    }

    public ItemSlot AddSlot(Item item)
    {
        ItemSlot slot = Instantiate(Resources.Load<GameObject>($"{slotResourcePath}/ItemSlot"), slotParent).GetComponent<ItemSlot>();
        slot.Icon.sprite = item.Info.Icon;
        slot.Item = item;
        AddSlotEvent(slot);
        slots.Add(slot);
        return slot;
    }

    public void RemoveSlot(ItemSlot slot)
    {
        if(slot.ItemCount > 0)
        {
            return;
        }
        slot.ItemCount = 0;
        Destroy(slot.gameObject);
    }

    public void RemoveAllSlot()
    {
        if(slots.Count == 0)
        {
            return;
        }
        foreach(ItemSlot slot in slots)
        {
            Destroy(slot.gameObject);
        }
    }

    protected virtual void AddSlotEvent(ItemSlot slot)
    {
        // 슬롯 클릭 이벤트 등록
        ItemInfo info = slot.Item.Info;
        slot.Button.onClick.AddListener(() => description.Initialize(slot.Item));
    }

    protected virtual void Initialize() { }
}
