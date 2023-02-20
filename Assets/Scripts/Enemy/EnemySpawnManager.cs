using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : Singleton<EnemySpawnManager>
{
    [field: SerializeField] public EnemySpawnGroup[] EnemyGroups { get; private set; }

    public EnemyGroupSaveData SaveData { get; private set; }

    private string _objectName = "SpawnGroup";

    private void Awake()
    {
        SaveData = Load();
        if(SaveData == null)
        {
            SaveData = new EnemyGroupSaveData();
        }
        InitializeComponent();
    }

    [ContextMenu("InitializeComponent")]
    public void InitializeComponent()
    {
        EnemyGroups = GetComponentsInChildren<EnemySpawnGroup>();
        for (int i = 0; i < EnemyGroups.Length; ++i)
        {
            if(SaveData.groupDatas.Count < EnemyGroups.Length)
            {
                SaveData.groupDatas.Add(new EnemyGroupData());
            }
            EnemyGroups[i].Initialize(SaveData.groupDatas[i]);
            EnemyGroups[i].gameObject.name = $"{_objectName}_{i}";
        }
        Save();
    }

    public void Save()
    {
        DataManager.Instance.SaveJson(SaveData, typeof(EnemyGroupSaveData).Name);
    }

    public EnemyGroupSaveData Load()
    {
        return DataManager.Instance.LoadJson<EnemyGroupSaveData>(typeof(EnemyGroupSaveData).Name);
    }
}

[Serializable]
public class EnemyGroupSaveData : SaveData
{
    [SerializeField] public List<EnemyGroupData> groupDatas = new List<EnemyGroupData>();

    public bool ContainsData(EnemyGroupData data)
    {
        for(int i = 0; i < groupDatas.Count;++i)
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