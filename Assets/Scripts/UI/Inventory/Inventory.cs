using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : Popup
{
    [field: Header("텍스트")]
    [field: SerializeField] public TMPro.TMP_Text CategoryName { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text BagWeigh { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text Gold { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ItemName { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ItemType { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ItemTotalStatusName { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ItemTotalStatusValue { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ItemEffectDescription { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text ItemDescription { get; private set; }

    [field: Header("버튼")]
    [field: SerializeField] public Button Exit { get; private set; }
    [field: SerializeField] public Button DetailStatus { get; private set; }
    [field: SerializeField] public Button[] Categorys { get; private set; }

    [field: Header("이미지")]
    [field: SerializeField] public Image ItemIcon { get; private set; }

    [field: Header("컴포넌트")]
    [field: SerializeField] public Transform SlotList { get; private set; }
    [field: SerializeField] public Transform Rank { get; private set; }

    [field: Header("설정")]
    [field: SerializeField] public int MaxWeight { get; private set; } = 2000;

    public float TotalBagWeight { get; private set; }

    private List<ItemSlot> slots = new List<ItemSlot>();
    private string slotResourcePath = "Prefabs/UI/Slot";

    private ItemCategory _currentPocusCategory = ItemCategory.Weapon;

    private void Awake()
    {
        Initialize();
    }

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        // 인벤토리 초기화
        BindButtonEvent();
        InitializeAllSlot();
        ActivateSlot(_currentPocusCategory);
        UpdateCategoryName();

        CloseAction = () =>
        {
            // 창이 닫힌경우 로직
            UIManager.Instance.IsOpneInventory = false;
        };
    }

    private void BindButtonEvent()
    {
        // 버튼 이벤트 바인딩
        for (int i = 0; i < Enum.GetValues(typeof(ItemCategory)).Length; i++)
        {
            if (IsButtonNull(Categorys[i]))
            {
                continue;
            }
            // AddListener의 클로저 문제 해결을 위한 인덱스 캐싱
            int cachingIndex = i;
            Categorys[cachingIndex].onClick.AddListener(() =>
            {
                // 카테고리 버튼 이벤트 로직
                _currentPocusCategory = (ItemCategory)cachingIndex;
                UpdateCategoryName();
                ActivateSlot(_currentPocusCategory);
            });
        }
    }

    private void UpdateCategoryName()
    {
        // 카테고리 이름 갱신
        string name = string.Empty;
        switch (_currentPocusCategory)
        {
            case ItemCategory.Weapon:
                name = "장비";
                break;
            case ItemCategory.Usable:
                name = "소비";
                break;
            case ItemCategory.Booty:
                name = "전리품";
                break;
            case ItemCategory.Extra:
                name = "기타";
                break;
        }
        CategoryName.text = name;
    }

    public void AddItem(Item item, int count)
    {
        // 아이템 추가
        ItemSlot targetSlot = FindSlot(item);
        targetSlot.ItemCount += count;
        TotalBagWeight += GetItemWeight(targetSlot.Item.Info.Weight, targetSlot.ItemCount);
        AddSlotEvent(targetSlot);
        UpdateBagWeight((int)TotalBagWeight, MaxWeight);
    }

    public void RemoveItem(Item item, int count)
    {
        // 아이템 제거
        ItemSlot targetSlot = FindSlot(item);
        targetSlot.ItemCount -= count;
        TotalBagWeight -= GetItemWeight(targetSlot.Item.Info.Weight, targetSlot.ItemCount);
        UpdateBagWeight((int)TotalBagWeight, MaxWeight);
    }

    public ItemSlot FindSlot(Item item)
    {
        // 아이템 탐색 후 아이템이 있는 슬롯 반환
        foreach (ItemSlot slot in slots)
        {
            if (!slot.Item.Info.Name.Equals(item.Info.Name))
            {
                return slot;
            }
        }
        return AddSlot(item);
    }

    public void InitializeAllSlot()
    {
        // 모든 슬롯의 아이템 무게 탐색
        InventorySaveData data = LoadInventory();

        if (TotalBagWeight != 0)
        {
            TotalBagWeight = 0;
        }

        for(int i = 0; data != null && i < data.itemInfo.Count && data.itemInfo.Count != 0;++i)
        {
            AddItem(data.itemInfo[i].ItemObject.GetComponent<Item>(), data.itemCount[i]);
        }
        UpdateBagWeight((int)TotalBagWeight, MaxWeight);
    }

    public void AddSlotEvent(ItemSlot slot)
    {
        // 슬롯 클릭 이벤트 등록
        slot.Button.onClick.AddListener(() =>
        {
            // 슬롯 클릭 이벤트 로직
            ItemName.text = slot.Item.Info.Name;
            ItemType.text = slot.Item.Info.Type;
            ItemIcon.sprite = slot.Icon.sprite;
            ItemEffectDescription.text = slot.Item.Info.EffectDescription;
            ItemDescription.text = slot.Item.Info.ItemDescription;
            UpdateItemRank(slot);
        });
    }

    private void UpdateItemRank(ItemSlot slot)
    {
        // 아이템 랭크 갱신
        for (int i = 0; i < Rank.childCount; ++i)
        {
            if (i <= (int)slot.Item.Info.Rank)
            {
                Rank.GetChild(i).gameObject.SetActive(true);
                continue;
            }
            Rank.GetChild(i).gameObject.SetActive(false);
        }
    }

    public List<ItemSlot> GetSlotFromCategory(ItemCategory category)
    {
        // 특정 카테고리의 슬롯 탐색 후 슬롯 리스트 반환
        List<ItemSlot> result = new List<ItemSlot>();
        foreach(ItemSlot slot in slots)
        {
            if(slot.Item.Info.Category == category)
            {
                result.Add(slot);
            }
        }
        return result;
    }

    public void ActivateSlot(ItemCategory category)
    {
        // 특정 카테고리의 슬롯을 제외한 슬롯 비활성화
        int slotCount = 0;
        foreach (ItemSlot slot in slots)
        {
            if (slot.Item.Info.Category == category)
            {
                slot.gameObject.SetActive(true);
                if (slotCount++ == 0)
                {
                    slot.Button.onClick.Invoke();
                }
                continue;
            }
            slot.gameObject.SetActive(false);
        }
    }

    public Item FindItem(Item item)
    {
        // 아이템 탐색 후 해당 아이템 반환 없을 경우 null 반환
        foreach (ItemSlot slot in slots)
        {
            if (!slot.Item.Info.Name.Equals(item.Info.Name))
            {
                return slot.Item;
            }
        }
        return null;
    }

    private ItemSlot AddSlot(Item item)
    {
        // 새 슬롯 추가
        ItemSlot slot = Instantiate(Resources.Load<GameObject>($"{slotResourcePath}/ItemSlot"), SlotList).GetComponent<ItemSlot>();
        slot.Icon.sprite = item.Info.Icon;
        slot.Item = item;
        AddSlotEvent(slot);
        slots.Add(slot);
        return slot;
    }

    private float GetItemWeight(float weight,int count)
    {
        // 아이템 무게 반환
        return (float)Math.Truncate((weight * count) * 100) / 100;
    }

    private void UpdateBagWeight(int current, int max)
    {
        // 배낭 총 무게 갱신
        BagWeigh.text = $"배낭 용량 {current}/{max}";
        SaveInventory();
    }

    private void SaveInventory()
    {
        // 인벤토리 세이브
        InventorySaveData data = new InventorySaveData();
        foreach (ItemSlot slot in slots)
        {
            data.itemInfo.Add(slot.Item.Info);
            data.itemStatus.Add(new ItemUpgradeData(slot.Item.Status));
            data.itemCount.Add(slot.ItemCount);
        }
        data.gold = 0; // 임시값 ( 게임 중앙 매니저에서 가져와야 함 )
        DataManager.Instance.SaveData(data, "Data/Save/Inventory", typeof(InventorySaveData).Name);
    }

    private InventorySaveData LoadInventory()
    {
        // 인벤토리 로드
        InventorySaveData result = DataManager.Instance.LoadData<InventorySaveData>("Data/Save/Inventory",typeof(InventorySaveData).Name);
        if(result == null)
        {
            return null;
        }
        return result;
    }
}
