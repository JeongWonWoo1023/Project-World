using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Defualt", menuName = "Charactors/Enemy/Defualt")]
public class EnemyScriptableData : ScriptableObject
{
    [field: SerializeField] public StatusData DefualtStatus { get; private set; }
    [field: SerializeField] public EnemyMovementData MovementData { get; private set; }
    [field: SerializeField] public LayerMask TargetMask { get; private set; }
}
