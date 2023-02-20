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

    public override void OnAnimationAttackEvent()
    {
        base.OnAnimationAttackEvent();
        isInput = false;
        UpdateTargetRotation(GetInputDirection());
        stateMachine.Player.Rigid.MoveRotation(Quaternion.Euler(stateMachine.Current.CurrentTargetRotation));
    }

    // 판정
    public override void OnAnimationEnter()
    {
        base.OnAnimationEnter();

        // 최대 공격 횟수 까지
        // 공격 횟수, 데미지 계수 등 데이터 필요

        stateMachine.Player.SkiilDamageCoef = stateMachine.Player.NormalAttack.Damages[attackCount];

        Transform hand = stateMachine.Player.Hand.transform;
        Quaternion rotation = Quaternion.LookRotation(hand.up,hand.forward);
        Effect effect = ObjectPool.Instance.PopObject<Effect>(stateMachine.Player.NormalAttack.Effect.name,
            stateMachine.Player.transform.position + Vector3.up,
            rotation);
        SoundManager.Instance.soundEffectSource.PlayOneShot(stateMachine.Player.NormalAttack.Sounds[attackCount]);
        effect.particle.Play();
        stateMachine.Player.AttackTarget(stateMachine.Player.NormalAttack.Effect.range.radius); // 공격
    }

    // 전이
    public override void OnAnimationTransition()
    {
        base.OnAnimationTransition();
        if (isInput)
        {
            SetAnimationValueFromHash(stateMachine.Player.AnimationData.NormalComboCountParameterHash, ++attackCount);
        }
    }

    // 종료
    public override void OnAnimationExit()
    {
        base.OnAnimationExit();

        StopAnimation(stateMachine.Player.AnimationData.NormalAttackParameterHash);
        // 상태 전환
        stateMachine.ChangeState(stateMachine.Idle);
    }
    #endregion

    #region 재사용 메소드
    protected override void AddInputAction()
    {
        base.AddInputAction();
        stateMachine.Player.Input.InGameActions.NormalAttack.started += OnNormalAtackAction;
        stateMachine.Player.Input.InGameActions.NormalAttack.performed += OnMovementPerformed;
        stateMachine.Player.Input.InGameActions.StrongAttack.performed += OnStrongAttack;
        stateMachine.Player.Input.InGameActions.UltimadomSkiil.performed += OnUltimadom;
    }

    protected override void RemoveInputAction()
    {
        base.RemoveInputAction();
        stateMachine.Player.Input.InGameActions.NormalAttack.started -= OnNormalAtackAction;
        stateMachine.Player.Input.InGameActions.NormalAttack.performed -= OnMovementPerformed;
        stateMachine.Player.Input.InGameActions.StrongAttack.performed -= OnStrongAttack;
        stateMachine.Player.Input.InGameActions.UltimadomSkiil.performed -= OnUltimadom;
    }
    #endregion

    #region 입력 메소드
    protected override void OnNormalAtackAction(InputAction.CallbackContext context)
    {
        isInput = true;
    }

    protected override void OnMovementPerformed(InputAction.CallbackContext context)
    {

    }

    protected override void OnStrongAttack(InputAction.CallbackContext obj)
    {
        stateMachine.ChangeState(stateMachine.StrongAttack);
    }

    protected override void OnUltimadom(InputAction.CallbackContext context)
    {
        base.OnUltimadom(context);
    }
    #endregion
}
