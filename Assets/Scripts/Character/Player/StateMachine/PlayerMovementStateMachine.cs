// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

public class PlayerMovementStateMachine : StateMachine
{
    public PlayerController Player { get; } // 플레이어 메인 스크립트 인스턴스
    public PlayerStateData StateData { get; } // 상태별 데이터 인스턴스
    public PlayerIdlingState IdlingState { get; } // 가만히 있는 상태 인스턴스
    public PlayerWalkingState WalkingState { get; } // 걷는 상태 인스턴스
    public PlayerRunningState RunningState { get; } // 뛰는 상태 인스턴스
    public PlayerSprintingState SprintingState { get; } // 스프린트 상태 인스턴스

    public PlayerMovementStateMachine(PlayerController playerController)
    {
        Player = playerController;

        StateData = new PlayerStateData();
        IdlingState = new PlayerIdlingState(this);
        WalkingState = new PlayerWalkingState(this);
        RunningState = new PlayerRunningState(this);
        SprintingState = new PlayerSprintingState(this);
    }
}
