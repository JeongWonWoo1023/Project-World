using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStrongAttackState : PlayerBattleState
{
    private int attackCount;
    public PlayerStrongAttackState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Anim.SetTrigger("TrigStrongSttack");
    }

    public override void Exit()
    {
        base.Exit();
        attackCount = 0;
    }

    public override void OnAnimationAttackEvent()
    {
        base.OnAnimationAttackEvent();
        UpdateTargetRotation(GetInputDirection());
        stateMachine.Player.Rigid.MoveRotation(Quaternion.Euler(stateMachine.Current.CurrentTargetRotation));
    }

    public override void OnAnimationEnter()
    {
        base.OnAnimationEnter();
        stateMachine.Player.SkiilDamageCoef = stateMachine.Player.StrongAttack.Damages[attackCount];

        Transform hand = stateMachine.Player.Hand.transform;
        Quaternion rotation = Quaternion.LookRotation(hand.up, hand.forward);
        Effect effect = ObjectPool.Instance.PopObject<Effect>(stateMachine.Player.StrongAttack.Effect.name,
            stateMachine.Player.transform.position + Vector3.up,
            rotation);

        SoundManager.Instance.soundEffectSource.PlayOneShot(stateMachine.Player.StrongAttack.Sounds[attackCount++]);

        effect.particle.Play();
        stateMachine.Player.AttackTarget(stateMachine.Player.StrongAttack.Effect.range.radius); // 공격
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.ChangeState(stateMachine.Idle);
    }

}
