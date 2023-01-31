using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySaveData : SaveData
{
    public List<ItemInfo> itemInfo = new List<ItemInfo>();
    public List<ItemUpgradeData> itemStatus = new List<ItemUpgradeData>();
    public List<int> itemCount = new List<int>();
    public int installedWeaponIndex;
    public int gold;
}

[Serializable]
public class ItemUpgradeData
{
    public ItemStatusData[] status;
    public ItemUpgradeData(ItemStatusData[] input)
    {
        status = input;
    }
}
