using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class P_GroundedData
{
    [field: SerializeField][field: Range(0f, 25f)] public float BaseMovementSpeed { get; private set; } = 5f;

    [field: SerializeField][field: Range(0.0f, 25.0f)] public float GroundSensorRayDistance { get; private set; } = 5.0f;

    [field: SerializeField] public List<PlayerCameraRecenteringData> SidewayCameraRecenteringData { get; private set; }
    [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardCameraRecenteringData { get; private set; }

    [field: SerializeField] public AnimationCurve SlopeSpeedCoef { get; private set; }

    [field: SerializeField] public RotationData RotationData { get; private set; }

    [field: SerializeField] public P_IdleData IdleData { get; private set; }

    [field: SerializeField] public P_WalkData WalkData { get; private set; }
    [field: SerializeField] public P_RunData RunData { get; private set; }
    [field: SerializeField] public P_SprintData SprintData { get; private set; }
    [field: SerializeField] public P_DashData DashData { get; private set; }

    [field: SerializeField] public P_StopData StopData { get; private set; }

    [field: SerializeField] public P_RollingData RollingData { get; private set; }

    [field: SerializeField] public LayerMask GroundMask { get; private set; }

    public bool ContainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }
    public bool IsGrounded(int layer)
    {
        return ContainsLayer(GroundMask, layer);
    }
}

[Serializable]
public class RotationData
{
    [field: SerializeField] public Vector3 ReachTargetRotation { get; private set; }
}

[Serializable]
public class P_IdleData
{
    [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardCameraRecenteringData { get; private set; }
}

[Serializable]
public class P_WalkData
{
    [field: SerializeField][field: Range(0.0f, 1.0f)] public float SpeedCoef { get; private set; } = 0.225f;

    [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardCameraRecenteringData { get; private set; }
}

[Serializable]
public class P_RunData
{
    [field: SerializeField][field: Range(1.0f, 2.0f)] public float SpeedCoef { get; private set; } = 1.0f;
}

[Serializable]
public class P_SprintData
{
    [field: SerializeField][field: Range(1.0f, 3.0f)] public float SpeedCoef { get; private set; } = 1.7f;

    [field: SerializeField][field: Range(0.0f, 5.0f)] public float SprintToRunTime { get; private set; } = 1.0f;
    [field: SerializeField][field: Range(0.0f, 2.0f)] public float RunToWalkTime { get; private set; } = 0.5f;
}

[Serializable]
public class P_DashData
{
    [field: SerializeField][field: Range(1.0f, 3.0f)] public float SpeedCoef { get; private set; } = 2.0f;
    [field: SerializeField] public RotationData RotationData { get; private set; }


    [field: SerializeField][field: Range(0.0f, 2.0f)] public float ConsideredTime { get; private set; } = 1.0f;
    [field: SerializeField][field: Range(1, 10)] public int UseLimitCount { get; private set; } = 2;
    [field: SerializeField][field: Range(0.0f, 5.0f)] public float CollTime { get; private set; } = 1.75f;
}

[Serializable]
public class P_StopData
{
    [field: SerializeField][field: Range(0.0f, 15.0f)] public float WalkDecelerationForce { get; private set; } = 5.0f;
    [field: SerializeField][field: Range(0.0f, 15.0f)] public float RunDecelerationForce { get; private set; } = 6.5f;
    [field: SerializeField][field: Range(0.0f, 15.0f)] public float SprintDecelerationForce { get; private set; } = 5.0f;
}

[Serializable]
public class P_RollingData
{
    [field: SerializeField][field: Range(0.0f, 3.0f)] public float SpeedCoef { get; private set; } = 1.0f;
}

[Serializable]
public class PlayerCameraRecenteringData
{
    [field: SerializeField][field: Range(0.0f, 360.0f)] public float MinimumAngle { get; private set; }
    [field: SerializeField][field: Range(0.0f, 360.0f)] public float MaximumAngle { get; private set; }
    [field: SerializeField][field: Range(-1.0f, 20.0f)] public float WaitTime { get; private set; }
    [field: SerializeField][field: Range(-1.0f, 20.0f)] public float RecenteringTime { get; private set; }

    public bool IsWithinRange(float angle)
    {
        return angle >= MinimumAngle && angle <= MaximumAngle;
    }
}

[Serializable]
public class PlayerAnimationData
{
    [Header("Group")]
    [SerializeField] private string paramGrounded = "isGrounded";
    [SerializeField] private string paramMoving = "isMoving";
    [SerializeField] private string paramStopping = "isStop";
    [SerializeField] private string paramLanding = "isLanding";
    [SerializeField] private string paramAirborne = "isAir";
    [SerializeField] private string paramBattle = "isBattle";

    [Header("Grounded")]
    [SerializeField] private string paramIdle = "isIdle";
    [SerializeField] private string paramDash = "isDash";
    [SerializeField] private string paramWalk = "isWalking";
    [SerializeField] private string paramRun = "isRunning";
    [SerializeField] private string paramSprinting = "isSprinting";
    [SerializeField] private string paramMediumStop = "isMediumStop";
    [SerializeField] private string paramHardStop = "isHardStop";
    [SerializeField] private string paramRoll = "isRolling";
    [SerializeField] private string paramHardLanding = "isHardLanding";

    [Header("Airborne")]
    [SerializeField] private string paramFall = "isFalling";

    [Header("Battle")]
    [SerializeField] private string paramNormalAttack = "isNormalAttack";
    [SerializeField] private string paramNormalComboCount = "NormalComboCount";

    public int GroundedParameterHash { get; private set; }
    public int MovingParameterHash { get; private set; }
    public int StoppingParameterHash { get; private set; }
    public int LandingParameterHash { get; private set; }
    public int AirborneParameterHash { get; private set; }
    public int BattleParameterHash { get; private set; }

    public int IdleParameterHash { get; private set; }
    public int DashParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int SprintParameterHash { get; private set; }
    public int MediumStopParameterHash { get; private set; }
    public int HardStopParameterHash { get; private set; }
    public int RollParameterHash { get; private set; }
    public int HardLandParameterHash { get; private set; }

    public int FallParameterHash { get; private set; }

    public int NormalAttackParameterHash { get; private set; }
    public int NormalComboCountParameterHash { get; private set; }

    public void Initialize()
    {
        GroundedParameterHash = Animator.StringToHash(paramGrounded);
        MovingParameterHash = Animator.StringToHash(paramMoving);
        StoppingParameterHash = Animator.StringToHash(paramStopping);
        LandingParameterHash = Animator.StringToHash(paramLanding);
        AirborneParameterHash = Animator.StringToHash(paramAirborne);
        BattleParameterHash = Animator.StringToHash(paramBattle);

        IdleParameterHash = Animator.StringToHash(paramIdle);
        DashParameterHash = Animator.StringToHash(paramDash);
        WalkParameterHash = Animator.StringToHash(paramWalk);
        RunParameterHash = Animator.StringToHash(paramRun);
        SprintParameterHash = Animator.StringToHash(paramSprinting);
        MediumStopParameterHash = Animator.StringToHash(paramMediumStop);
        HardStopParameterHash = Animator.StringToHash(paramHardStop);
        RollParameterHash = Animator.StringToHash(paramRoll);
        HardLandParameterHash = Animator.StringToHash(paramHardLanding);

        FallParameterHash = Animator.StringToHash(paramFall);

        NormalAttackParameterHash = Animator.StringToHash(paramNormalAttack);
        NormalComboCountParameterHash = Animator.StringToHash(paramNormalComboCount);
    }
}