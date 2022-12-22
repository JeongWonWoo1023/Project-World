// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using System;
using UnityEngine;

[Serializable]
public class PlayerGroundedData
{
    [field: SerializeField][field: Range(0.0f, 25.0f)] public float BaseMoveSpeed { get; private set; } = 5.0f;
    [field: SerializeField] public AnimationCurve SlopeSpeedAngle { get; private set; }
    [field: SerializeField] public PlayerRotationData BaseRotation { get; private set; }
    [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
    [field: SerializeField] public PlayerRunData RunData { get; private set; }
}
[Serializable]
public class PlayerWalkData
{
    [field: SerializeField][field: Range(0.0f, 1.0f)] public float MoveSpeedCoef { get; private set; } = 0.225f;
}
[Serializable]
public class PlayerRunData
{
    [field: SerializeField][field: Range(1.0f, 2.0f)] public float MoveSpeedCoef { get; private set; } = 1.0f;
}

[Serializable]
public class PlayerLayerData
{
    [field: SerializeField] public LayerMask GroundMask { get; private set; }
}

[Serializable]
public class PlayerRotationData
{
    [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
}
