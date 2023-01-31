using System;

using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Player", menuName = "Charactors/Player")]
public class PlayerScriptableData : ScriptableObject
{
    [field: SerializeField] public PlayerMovementData Movement { get; private set; }
    [field: SerializeField] public StatusData DefualtStatus { get; private set; }
}
