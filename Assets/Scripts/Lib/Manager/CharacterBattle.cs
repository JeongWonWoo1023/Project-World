using System;
using UnityEngine;

public class CharacterBattle : MonoBehaviour, IBattle
{
    [HideInInspector] public StatusData status; // 능력치
    [field: SerializeField] public ConnectUIData ConnectUI { get; private set; } // 연동 UI
    public Transform EnemyUITrakingTrans { get; set; }
    private int _level;
    private int _currentHP;
    private int _currentMP;
    private int _currentEXP;

    private bool _isDead;

    private float _skiilDamageCoef;

    private EnemyStatusUI _enemyStatusUI = null;

    // 레벨 프로퍼티
    public int Level
    {
        get => _level;
        set
        {
            if (Level == value)
            {
                Debug.Log("대입 할 레벨과 현재 레벨이 같습니다");
                return;
            }
            _level = value;
            foreach (TMPro.TMP_Text eliment in ConnectUI.Level)
            {
                if(eliment == null)
                {
                    continue;
                }
                eliment.text = $"Lv. {Level}";
            }
        }
    }

    // 현재 HP 프로퍼티 ( UI 연동 )
    public int CurrentHP
    {
        get => _currentHP;
        set
        {
            _currentHP = value;
            if (ConnectUI.HPBar == null)
            {
                // HP가 없는경우 리턴
                return;
            }
            ConnectUI.HPBar.FrontSlider.value = GetRatio(status.Health.MaxHp, CurrentHP);

            if (ConnectUI.HPBar.ValueInfo == null)
            {
                return;
            }
            ConnectUI.HPBar.ValueInfo.text = $"{CurrentHP} / {status.Health.MaxHp} ( {(GetRatio(status.Health.MaxHp, CurrentHP) * 100.0f).ToString("F2")}% )";
        }
    }

    // 현재 MP 프로퍼티 ( UI 연동 )
    public int CurrentMP
    {
        get => _currentMP;
        set
        {
            _currentMP = value;
            if(ConnectUI.MPBar == null)
            {
                // MP가 없는경우 리턴
                return;
            }
            ConnectUI.MPBar.FrontSlider.value = GetRatio(status.Health.MaxMp, CurrentMP);

            if(ConnectUI.MPBar.ValueInfo == null)
            {
                return;
            }
            ConnectUI.MPBar.ValueInfo.text = $"{CurrentMP} / {status.Health.MaxMp} ( {(GetRatio(status.Health.MaxMp, CurrentMP) * 100.0f).ToString("F2")}% )";
        }
    }

    // 현재 EXP 프로퍼티 ( UI 연동 )
    public int CurrentEXP
    {
        get => _currentEXP;
        set
        {
            _currentEXP = value;
            if (ConnectUI.EXPBar == null)
            {
                // MP가 없는경우 리턴
                return;
            }
            ConnectUI.EXPBar.BackSlider.value = GetRatio(status.Experience.Point, CurrentEXP);

            if (ConnectUI.EXPBar.ValueInfo == null)
            {
                return;
            }
            ConnectUI.EXPBar.ValueInfo.text = $"{CurrentEXP} / {status.Experience.Point} ( {(GetRatio(status.Experience.Point, CurrentEXP) * 100.0f).ToString("F2")}% )";
        }
    }

    // 사망여부 값 프로퍼티 ( UI 연동 )
    public bool IsDead
    {
        get => _isDead;
        set
        {
            _isDead = value;
        }
    }

    // 스킬 데미지 계수
    public float SkiilDamageCoef
    {
        get => _skiilDamageCoef;
        set
        {
            _skiilDamageCoef = value <= 0.0f ? 0.0f : (float)Math.Truncate(value * 100) / 100;
        }
    }

    public bool IsHit { get; set; }

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private SphereCollider weapon;

    public void InitializeStatus(StatusData data)
    {
        status = new StatusData();
        status = data;

        Level = status.Experience.Level;
        CurrentHP = data.Health.MaxHp;
        CurrentMP = data.Health.MaxMp;
        CurrentEXP = 0;
    }

    public void UpdateStatus(StatusData data,int level,int hp, int mp, int exp)
    {
        status = data;
        Level = level;
        CurrentHP = hp;
        CurrentMP = mp;
        CurrentEXP = exp;
    }

    public float GetAttackRange()
    {
        return weapon.radius;
    }

    #region IBattle 인터페이스 메소드
    public void AttackTarget()
    {
        Vector3 center = weapon.transform.position; // 무기 원점으로 변경 필요
        Collider[] targets = Physics.OverlapSphere(center, weapon.radius, targetMask, QueryTriggerInteraction.Ignore); // 무기 공격 범위 적용 필요

        foreach (Collider target in targets)
        {
            IBattle targetBattle = target.GetComponent<IBattle>();

            float defence = targetBattle.GetDefence(status.AttackType); // 공격 유형에 따라 방어력 가져오기
            // 방어력 가져오기 실패 시
            if (defence < 0)
            {
                return;
            }
            targetBattle.OnDamage(CalDamage(defence));
            if(targetBattle.IsDead)
            {
                CurrentEXP += targetBattle.GetEXP();
            }
        }
    }

    public float GetDefence(AttackType type)
    {
        switch (type)
        {
            case AttackType.Physical:
                return status.Defence.DefualtPhysical + (status.Experience.Level * status.Defence.PhysicalCoef);
            case AttackType.Magical:
                return status.Defence.DefualtMagical + (status.Experience.Level * status.Defence.MagicalCoef);
            default:
                return -1;
        }
    }

    public int GetEXP()
    {
        return status.Experience.Point;
    }

    public void OnDamage(int damage)
    {
        SetEnemyUI();
        CurrentHP -= damage;
        IsHit = true;

        // 체력이 0 미만으로 내려갔을 경우
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            _enemyStatusUI.gameObject.SetActive(false);
            IsDead = true;
        }
    }

    private void SetEnemyUI()
    {
        if (_enemyStatusUI != null || ConnectUI.EnemyUIPrefab == null)
        {
            return;
        }
        _enemyStatusUI = ObjectPool.Instance.PopObject<EnemyStatusUI>(ConnectUI.EnemyUIPrefab.gameObject.name,
                Vector3.zero);
        _enemyStatusUI.Initialize(UIManager.Instance.EnemyUIParantTrans, EnemyUITrakingTrans);
        ConnectUI.SetEnemyUIComponent(_enemyStatusUI);
    }
    #endregion

    // 데미지 계산
    private int CalDamage(float defenceValue)
    {
        float result = 0.0f;
        float attackOrigin = status.Attack.Defualt + (status.Experience.Level * status.Attack.Coef);
        switch (status.AttackType)
        {
            // 1차 계산식 : 공격력 - 방어율
            case AttackType.Physical:
                result = attackOrigin - (defenceValue - (defenceValue * status.Attack.PhysicalPenetration));
                break;
            case AttackType.Magical:
                result = attackOrigin - (defenceValue - (defenceValue * status.Attack.MagicalPenetration));
                break;
        }

        // 2차 계산 식 : 1차 계산식 결과 * 치명타 데미지
        // 크리티컬 발동
        if (GetCritical())
        {
            result *= status.Attack.CriticalDamageCoef;
        }

        // 3차 계산 식 : 스킬 퍼센트 데미지 적용
        result *= SkiilDamageCoef;
        return (int)result;
    }

    // 크리티컬 여부 반환
    private bool GetCritical()
    {
        float value = (float)Math.Truncate((UnityEngine.Random.Range(0.0f, 1.0f) * 1000) / 1000); // 소수 셋째자리 미만 자르기
        return status.Attack.CriticalProbability <= value;
    }

    private float GetRatio(float max, float current)
    {
        return current / max;
    }
}
