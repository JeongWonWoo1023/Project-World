using System;
using System.Collections.Generic;
using UnityEngine;

public enum RegenType
{
    Batch, Individual
}

public class FieldItemGroup : MonoBehaviour
{
    [field: SerializeField] public GameObject ItemPrefab { get; private set; }
    [field: SerializeField] public RegenType RegenType { get; private set; }
    [field: SerializeField] public FieldItemPoint[] SpawnPoints { get; private set; }
    [field: SerializeField] public TimeManager TimeManager { get; private set; }

    private FieldItemGroupData data;
    private bool _isStartCoolTime;
    public bool IsStartCoolTIme
    {
        get => _isStartCoolTime;
        set
        {
            _isStartCoolTime = value;
            if (IsStartCoolTIme)
            {
                TimeManager.ApplyCoolTime(null);
                data.coolEnd = TimeManager.End.ToString("yyyyMMddHHmmss");
                data.isGet.Clear();
                foreach(FieldItemPoint point in SpawnPoints)
                {
                    data.isGet.Add(point.IsGet);
                }
                if (FieldItemManager.Instance.SaveData.ContainsData(data))
                {
                    Debug.Log("해당 그룹의 데이터가 이미 존재합니다");
                }
                FieldItemManager.Instance.Save();
            }
        }
    }

    private string _childName = "Point";

    public void Initialize(FieldItemGroupData saveData)
    {
        // 아이템 생성 트랜스폼 초기화
        SpawnPoints = GetComponentsInChildren<FieldItemPoint>();
        data = saveData;
        TimeManager.End = DateTime.ParseExact(data.coolEnd, "yyyyMMddHHmmss",
            System.Globalization.CultureInfo.InvariantCulture);
        for (int i = 0; i < transform.childCount; ++i)
        {
            SpawnPoints[i].gameObject.name = $"{_childName}_{i}";
            if (data.isGet.Count < transform.childCount)
            {
                data.isGet.Add(SpawnPoints[i].IsGet);
                continue;
            }
            SpawnPoints[i].IsGet = TimeManager.IsEndCoolTime() ? false : data.isGet[i];
        }
    }
}

[Serializable]
public class FieldItemGroupData
{
    [SerializeField] public List<bool> isGet = new List<bool>();
    public string coolEnd = default(DateTime).ToString("yyyyMMddHHmmss");
}
