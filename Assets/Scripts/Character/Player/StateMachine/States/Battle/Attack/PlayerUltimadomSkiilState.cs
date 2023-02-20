using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUltimadomSkiilState : PlayerBattleState
{
    private int attackCount;
    private float range;
    public PlayerUltimadomSkiilState(PlayerStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (stateMachine.Player.Rigid.velocity != Vector3.zero)
        {
            stateMachine.Player.Rigid.velocity = Vector3.zero;
        }
        stateMachine.Player.StartCoroutine(CoolTimeStart());
        Effect effect = ObjectPool.Instance.PopObject<Effect>(stateMachine.Player.Ultimadom.Effect.name,
            stateMachine.Player.transform.position,
            stateMachine.Player.transform.rotation);

        SoundManager.Instance.soundEffectSource.PlayOneShot(stateMachine.Player.Ultimadom.Sounds[attackCount]);

        attackCount = 0;
        range = stateMachine.Player.Ultimadom.Effect.range.radius;
        effect.particle.Play();
        stateMachine.Player.SkiilDamageCoef = stateMachine.Player.Ultimadom.Damages[attackCount++];
        stateMachine.Player.AttackTarget(range, stateMachine.Player.Ultimadom.Cost); // 공격
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void OnAnimationEnter()
    {
        base.OnAnimationEnter();
        stateMachine.Player.StartCoroutine(AttackProcess());
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.ChangeState(stateMachine.Idle);
    }

    private IEnumerator AttackProcess()
    {
        while(attackCount < stateMachine.Player.Ultimadom.HitCount)
        {
            stateMachine.Player.SkiilDamageCoef = stateMachine.Player.Ultimadom.Damages[attackCount];
            stateMachine.Player.AttackTarget(range); // 공격
            SoundManager.Instance.soundEffectSource.PlayOneShot(stateMachine.Player.Ultimadom.Sounds[attackCount++]);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator CoolTimeStart(float coolTime = 30.0f)
    {
        stateMachine.Current.UltimadomCoolTime = coolTime;
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while(stateMachine.Current.UltimadomCoolTime > 0.0f)
        {
            stateMachine.Current.UltimadomCoolTime -= Time.deltaTime;
            yield return delay;
        }
        stateMachine.Current.UltimadomCoolTime = 0.0f;
    }
}
