using UnityEngine;

// 공통 실시간 데이터
public class CharacterCurrentData
{
    public float MovementSpeedCoef { get; set; } = 1.0f; // 현재 이동속도 계수
    public float MovementOnSlopeSpeed { get; set; } = 1.0f; // 현재 경사로 이동속도
    public float MovementDecelerationForce { get; set; } = 1.0f; // 현재 감속도

    private Vector3 _currentTargetRotation;
    private Vector3 _timeToReachTargetRotation;
    private Vector3 _dampedTargetRotationCurrentValocity;
    private Vector3 _dampedTargetRotationPassedTime;

    public ref Vector3 CurrentTargetRotation
    {
        get
        {
            return ref _currentTargetRotation;
        }
    }
    public ref Vector3 TimeToReachTargetRotation
    {
        get
        {
            return ref _timeToReachTargetRotation;
        }
    }
    public ref Vector3 DampedTargetRotationCurrentValocity
    {
        get
        {
            return ref _dampedTargetRotationCurrentValocity;
        }
    }
    public ref Vector3 DampedTargetRotationPassedTime
    {
        get
        {
            return ref _dampedTargetRotationPassedTime;
        }
    }

    public bool IsAttak { get; set; } // 공격 중
    public bool IsDead { get; set; } // 사망
    public bool IsKnockback { get; set; } // 넉백
}
