using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [field: SerializeField] public Enemy EnemyPrefab { get; private set; }
    [field: SerializeField] public EnemyMovementType MoveType { get; private set; } = EnemyMovementType.Stand;
    [field: SerializeField] public Vector3[] MovementPath { get; private set; }
    private EnemySpawnGroup _group;
    public EnemySpawnGroup Group
    {
        get
        {
            if(_group == null)
            {
                _group = GetComponentInParent<EnemySpawnGroup>();
            }
            return _group;
        }
    }

    [SerializeField] private bool _isSubjugation;
    public bool IsSubjugation
    {
        get => _isSubjugation;
        set
        {
            _isSubjugation = value;
            if(IsSubjugation)
            {
                Group.IsStartCoolTIme = true;
            }
        }
    }

    private Enemy _clone;

    public void Enable()
    {
        // 몬스터 스폰
        if(IsSubjugation)
        {
            return;
        }
        _clone = ObjectPool.Instance.PopObject<Enemy>(EnemyPrefab.gameObject.name, transform.position, transform.rotation);
        if (MoveType == EnemyMovementType.SelectPath)
        {
            _clone.RoutinePath = MovementPath;
        }
        _clone.Data.MovementData.Type = MoveType;
        _clone.SpawnPoint = this;
        _clone.stateMachine.ChangeState(_clone.stateMachine.Spawn);
    }

    public void Disable()
    {
        // 렌더 거리보다 멀어질 경우 몬스터 비활성화
        _clone.gameObject.SetActive(false);
    }
}
