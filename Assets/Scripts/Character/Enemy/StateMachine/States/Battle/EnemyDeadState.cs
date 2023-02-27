using UnityEngine;

public class EnemyDeadState : EnemyBattleState
{
    public EnemyDeadState(EnemyStateMachine _stateMachine) : base(_stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;
        stateMachine.Enemy.SpawnPoint.IsSubjugation = true;
        QuestManager.Instance.Hunt(stateMachine.Enemy.name);
        SetAnimationIntValue("animation", (int)EnemyAnimationType.Die);
    }

    public override void OnAnimationExit()
    {
        base.OnAnimationExit();
        stateMachine.Current.IsDead = true;
        for(int i = 0; i < stateMachine.Enemy.DropTable.DropItem.Length; ++i)
        {
            if(stateMachine.Enemy.DropTable.DropRatio[i] >= Random.Range(0,101))
            {
                Item drop = ObjectPool.Instance.PopObject<Item>(stateMachine.Enemy.DropTable.DropItem[i].name,
                    stateMachine.Enemy.transform.position + Vector3.up, stateMachine.Enemy.transform.rotation);
                drop.Count = stateMachine.Enemy.DropTable.DropCount[i];
            }
        }
        UIManager.Instance.Inventory.Gold += stateMachine.Enemy.DropTable.DropGold;
        stateMachine.Enemy.gameObject.SetActive(false);
    }
}
