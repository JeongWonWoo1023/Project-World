using System;
using UnityEngine;

[Serializable]
public class P_AirborneData
{
    [field: SerializeField] public P_JumpData JumpData { get; private set; }
    [field: SerializeField] public P_FallData FallData { get; private set; }
}

[Serializable]
public class P_JumpData
{
    [field: SerializeField] public RotationData RotationData { get; private set; }
    [field: SerializeField][field: Range(0.0f, 5.0f)] public float JumpToGroundRayDistance { get; private set; } = 2.0f;

    [field: SerializeField] public AnimationCurve OnSlopeUpCoef { get; private set; }
    [field: SerializeField] public AnimationCurve OnSlopeDownCoef { get; private set; }

    [field: SerializeField] public Vector3 StandJumpForce { get; private set; }
    [field: SerializeField] public Vector3 WalkJumpForce { get; private set; }
    [field: SerializeField] public Vector3 RunJumpForce { get; private set; }
    [field: SerializeField] public Vector3 SprintJumpForce { get; private set; }

    [field: SerializeField][field: Range(0.0f, 10.0f)] public float DecelerationForce { get; private set; } = 1.5f;
}

[Serializable]
public class P_FallData
{
    [field: SerializeField][field: Range(1.0f, 15.0f)] public float FallSpeedLimit { get; private set; } = 15.0f;
    [field: SerializeField][field: Range(0.0f, 100.0f)] public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3.0f;
}
