using System;
using UnityEngine;

[Serializable]
public class ConnectUIData
{
    [field: SerializeField] public Gauge HPBar { get; private set; }
    [field: SerializeField] public Gauge MPBar { get; private set; }
    [field: SerializeField] public Gauge EXPBar { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text[] Level { get; private set; }
    [field: SerializeField] [field: Tooltip("플레이어는 바인딩 하지 마시오")] public EnemyStatusUI EnemyUIPrefab { get; private set; }

    public void SetEnemyUIComponent(EnemyStatusUI ui)
    {
        HPBar = ui.HPBar;
        if (Level.Length == 0)
        {
            Level = new TMPro.TMP_Text[1];
        }
        Level[0] = ui.Level;
    }
}
