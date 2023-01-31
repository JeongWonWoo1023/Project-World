using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNormalAttackState : PlayerBattleState
{
    #region 필드 & 프로퍼티
    private int attackCount;
    private bool isInput;
    #endregion

    #region 생성자
    public PlayerNormalAttackState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }
    #endregion

    #region IState 인터페이스 메소드
    public override void Enter()
    {
        base.Enter();
        attackCount = 0;
        stateMachine.Player.SkiilDamageCoef = 0.0f;
        isInput = false;
        AddInputAction();
        if (stateMachine.Player.Rigid.velocity != Vector3.zero)
        {
            stateMachine.Player.Rigid.velocity = Vector3.zero;
        }
        SetAnimationValueFromHash(stateMachine.Player.AnimationData.NormalComboCountParameterHash, attackCount);
        StartAnimation(stateMachine.Player.AnimationData.NormalAttackParameterHash);
    }

    public override void Exit()
    {
        base.Exit();
        RemoveInputAction();
        StopAnimation(stateMachine.Player.AnimationData.NormalAttackParameterHash);
    }

    // 판정
    public override void OnAnimationEnter()
    {
        base.OnAnimationEnter();

        // 최대 공격 횟수 까지 ( 임시 테스트용 )
        // 공격 횟수, 데미지 계수 등 데이터 필요
        if (attackCount < 5)
        {
            // 최초 공격 시
            if (attackCount == 0)
            {
                stateMachine.Player.SkiilDamageCoef = 1.3f; // 스킬 퍼뎀 초기화
            }
            else
            {
                stateMachine.Player.SkiilDamageCoef *= 1.05f; // 가중치 부여
            }
            stateMachine.Player.AttackTarget(); // 공격
        }
    }

    // 전이
    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        if(isInput)
        {
            SetAnimationValueFromHash(stateMachine.Player.AnimationData.NormalComboCountParameterHash, ++attackCount);
            isInput = false;
        }
    }

    // 종료
    public override void OnAnimationExit()
    {
        base.OnAnimationExit();

        SetAnimationValueFromHash(stateMachine.Player.AnimationData.NormalComboCountParameterHash, attackCount);
        StopAnimation(stateMachine.Player.AnimationData.NormalAttackParameterHash);
        // 상태 전환
        stateMachine.ChangeState(stateMachine.Idle);
    }
    #endregion

    #region 재사용 메소드
    protected override void AddInputAction()
    {
        base.AddInputAction();
        stateMachine.Player.Input.InGameActions.NormalAttack.performed += OnNormalAtack;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
        stateMachine.Player.Input.InGameActions.NormalAttack.performed -= OnNormalAtack;
    }
    #endregion

    #region 입력 메소드
    protected override void OnNormalAtack(InputAction.CallbackContext context)
    {
        if(isInput || UIManager.Instance.IsPause)
        {
            return;
        }
        isInput = true;
    }
    #endregion
}
