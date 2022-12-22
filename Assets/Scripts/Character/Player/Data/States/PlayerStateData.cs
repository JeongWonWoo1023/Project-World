// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using UnityEngine;

public class PlayerStateData
{
    public Vector2 MovementInput { get; set; }
    public float MoveSpeedCoef { get; set; } = 1.0f;
    public float MovementOnSlopeSpeed { get; set; } = 1.0f;
    public bool IsWalk { get; set; }

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
}
