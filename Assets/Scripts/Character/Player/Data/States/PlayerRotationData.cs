// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using System;
using UnityEngine;

[Serializable]
public class PlayerRotationData
{
    [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
}
