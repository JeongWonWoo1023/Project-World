public class PlayerStateMachine : StateMachine
{
    #region 필드 & 프로퍼티
    public Player Player { get; } // 플레이어 메인 컨트롤러
    public PlayerCurrentData Current { get; } // 현재 데이터

    public P_IdlingState Idle { get; } // 기본
    public P_WalkingState Walk { get; } // 걷기
    public P_RunningState Run { get; } // 달리기
    public P_SprintingState Sprint { get; } // 스프린트
    public P_DashState Dash { get; } // 대쉬

    public P_WalkStoppingState WalkToStop { get; } // 걷는 중 정지
    public P_RunStoppingState RunToStop { get; } // 달리기 중 정지
    public P_SprintStoppingState SprintToStop { get; } // 스프린트 중 정지

    public P_JumpingState Jump { get; } // 점프
    public P_FallingState Fall { get; } // 공중에 뜬 상태

    public P_LightLandingState LightLanding { get; } // 낮은 높이에서의 착지
    public P_RollingState Rolling { get; } // 착지 후 구르기
    public P_HardLandindState HardLanding { get; } // 높은 높이에서의 착지

    public PlayerNormalAttackState NormalAttack { get; } // 기본공격
    #endregion

    #region 생성자
    public PlayerStateMachine(Player player)
    {
        Player = player;
        Current = new PlayerCurrentData();

        Idle = new P_IdlingState(this);
        Walk = new P_WalkingState(this);
        Run = new P_RunningState(this);
        Sprint = new P_SprintingState(this);
        Dash = new P_DashState(this);

        WalkToStop = new P_WalkStoppingState(this);
        RunToStop = new P_RunStoppingState(this);
        SprintToStop = new P_SprintStoppingState(this);

        Jump = new P_JumpingState(this);
        Fall = new P_FallingState(this);

        LightLanding = new P_LightLandingState(this);
        Rolling = new P_RollingState(this);
        HardLanding = new P_HardLandindState(this);

        NormalAttack = new PlayerNormalAttackState(this);
    }
    #endregion

}
