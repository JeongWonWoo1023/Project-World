using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AttackType
{
    Physical, Magical
}

public abstract class BattleSystem : MonoBehaviour, IBattle
{
    [Serializable]
    public class Components
    {
        [Tooltip("물리공격형 캐릭터의 경우 무기를 마법공격형 캐릭터의 경우 범위 오브젝트 바인딩")]
        public GameObject rangeObj;

        [HideInInspector] public SphereCollider collSphere;
        [HideInInspector] public BoxCollider collBox;
        [HideInInspector] public Vector3 boxSize;
        [HideInInspector] public float radius;
    }

    [Serializable]
    public class BattleState
    {
        public bool isAttacking;
        public bool isDead;
    }

    [SerializeField] private Components _compo = new Components();
    [SerializeField] private BattleState _state = new BattleState();

    public Components Compo => _compo;
    public BattleState State => _state;

    protected virtual void Start()
    {
        InitComponent();
    }

    public virtual void InitComponent()
    {
        Compo.rangeObj.TryGetComponent(out Compo.collSphere);
        Compo.rangeObj.TryGetComponent(out Compo.collBox);
        if(Compo.collSphere != null)
        {
            Compo.radius = Compo.collSphere.radius;
        }
        else if (Compo.collBox != null)
        {
            Compo.boxSize = Compo.collBox.size * 0.5f;
        }
    }

    public abstract void OnDamage(int value);
}
