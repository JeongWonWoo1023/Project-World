using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    void OnAttack(); // 공격 이벤트 메소드
    void OnDamage(int value); // 데미지
    void OnDead(); // 사망
}

public interface ISkill
{
    void OnSkill(int cost); // 스킬 사용 이벤트 메소드
}

