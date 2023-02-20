using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnGroup : MonoBehaviour
{
    [field: SerializeField] public EnemySpawnPoint[] SpawnPoints { get; private set; }
    [field: SerializeField] public TimeManager TimeManager { get; private set; }

    private EnemyGroupData data;
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
                data.isSubjugation.Clear();
                foreach (EnemySpawnPoint point in SpawnPoints)
                {
                    data.isSubjugation.Add(point.IsSubjugation);
                }
                EnemySpawnManager.Instance.SaveData.ContainsData(data);
                EnemySpawnManager.Instance.Save();
            }
        }
    }

    private string _childName = "Point";

    public void Initialize(EnemyGroupData saveData)
    {
        // 적 생성 트랜스폼 초기화
        SpawnPoints = new EnemySpawnPoint[transform.childCount];

        data = saveData;
        TimeManager.End = DateTime.ParseExact(data.coolEnd, "yyyyMMddHHmmss",
            System.Globalization.CultureInfo.InvariantCulture);
        SpawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        for (int i = 0; i < transform.childCount; ++i)
        {
            SpawnPoints[i].gameObject.name = $"{_childName}_{i}";
            if (data.isSubjugation.Count < transform.childCount)
            {
                data.isSubjugation.Add(SpawnPoints[i].IsSubjugation);
                continue;
            }
            SpawnPoints[i].IsSubjugation = TimeManager.IsEndCoolTime() ? false : data.isSubjugation[i];
        }
    }
}

[Serializable]
public class EnemyGroupData
{
    [SerializeField] public List<bool> isSubjugation = new List<bool>();
    public string coolEnd = default(DateTime).ToString("yyyyMMddHHmmss");
}

