using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaleItemList : MonoBehaviour
{
    public Transform slotParent;

    [SerializeField] private SaleItemData saleData;
    [SerializeField] private SaleItemSaveData saveData = new SaleItemSaveData();

    private string slotResourcePath = "Prefabs/UI/Slot";

    private ShoppingBasket basket;
    private ItemDescription description;

    public class SaleItemSaveData : SaveData
    {
        [field: SerializeField] public int[] Redundancy { get; private set; }

        public void ResetCount(int index)
        {
            Redundancy[index] = 0;
        }
    }

    public void InitializeComponent(ShoppingBasket basket, ItemDescription description)
    {
        this.basket = basket;
        this.description = description;
    }

    public void Initialize()
    {
        if(saleData.Item.Length == 0)
        {
            return;
        }
        for (int i = 0; i < saleData.Item.Length; ++i)
        {
            SaleItemSlot slot = Instantiate(Resources.Load<GameObject>($"{slotResourcePath}/SaleItemSlot"), slotParent).GetComponent<SaleItemSlot>();
            slot.Initialize(saleData.Item[i], saleData.Price[i], saleData.LimitSaleCount[i]);
            slot.InitializeEvent(() => description.Initialize(saleData.Item[i]),() => basket.AddItem(slot.Item, saleData.Price[i]),() => basket.RemoveItem(slot.Item, saleData.Price[i]));
        }
    }
}
