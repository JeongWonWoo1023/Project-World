using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItemManager : Singleton<FieldItemManager>
{
    [field: SerializeField] public FieldItemGroup[] FieldItemGroups { get; private set; }

    public ItemGroupSaveData SaveData { get; private set; }

    private void Awake()
    {
        SaveData = Load();
        if (SaveData == null)
        {
            SaveData = new ItemGroupSaveData();
        }
        InitializeComponent();
    }

    [ContextMenu("InitializeComponent")]
    public void InitializeComponent()
    {
        // 컴포넌트 초기화
        FieldItemGroups = GetComponentsInChildren<FieldItemGroup>();
        for(int i = 0; i < FieldItemGroups.Length; ++i)
        {
            if (SaveData.groupDatas.Count < FieldItemGroups.Length)
            {
                SaveData.groupDatas.Add(new FieldItemGroupData());
            }
            FieldItemGroups[i].Initialize(SaveData.groupDatas[i]);
        }
        Save();
    }

    public void Save()
    {
        DataManager.Instance.SaveJson(SaveData, typeof(ItemGroupSaveData).Name);
    }

    public ItemGroupSaveData Load()
    {
        return DataManager.Instance.LoadJson<ItemGroupSaveData>(typeof(ItemGroupSaveData).Name);
    }
}

[Serializable]
public class ItemGroupSaveData : SaveData
{
    [SerializeField] public List<FieldItemGroupData> groupDatas = new List<FieldItemGroupData>();

    public bool ContainsData(FieldItemGroupData data)
    {
        for (int i = 0; i < groupDatas.Count; ++i)
        {
            if (groupDatas[i].Equals(data))
            {
                groupDatas[i] = data;
                return true;
            }
        }
        groupDatas.Add(data);
        return false;
    }
}
