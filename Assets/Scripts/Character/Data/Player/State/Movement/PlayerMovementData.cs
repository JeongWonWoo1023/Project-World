using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Player", menuName = "Charactors/Player")]
public class PlayerMovementData
{
    [field: SerializeField] public P_GroundedData GroundedData { get; private set; }
    [field: SerializeField] public P_AirborneData AirborneData { get; private set; }

}
