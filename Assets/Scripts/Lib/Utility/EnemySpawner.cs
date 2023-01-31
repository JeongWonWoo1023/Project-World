using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [field: SerializeField] public Enemy EnemyPrefab { get; private set; }

    [SerializeField] private IntervalTime interval;
    [SerializeField] private ContentType contentInterval = ContentType.Day;
    [SerializeField] private EnemyMovementType moveType = EnemyMovementType.Stand;
    [SerializeField] private Vector3[] routinePath;
    private Enemy clone; // 생성할 클론
    private bool isDead; // 해당 위치 오브젝트 사망여부

    // 활성화
    public void Enable()
    {
        if (isDead && interval.IsCoolTime())
        {
            return;
        }

        clone = ObjectPool.Instance.PopObject(EnemyPrefab.gameObject.name, transform.position, transform.rotation).GetComponent<Enemy>();
        clone.RoutinePath = routinePath;
        clone.Data.MovementData.Type = moveType;

        clone.stateMachine.ChangeState(clone.stateMachine.Spawn);
    }

    // 비활성화
    public void Disable()
    {
        if(isDead)
        {
            return;
        }

        if(clone.stateMachine.Current.IsDead)
        {
            interval.startTime = DateTime.Now;
            isDead = true;
        }
        clone.gameObject.SetActive(false);
    }
}
