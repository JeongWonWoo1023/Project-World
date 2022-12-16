using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattle
{
    void OnDamage(int value); // 데미지
}

public interface ISkill
{
    void OnSkill(int cost); // 스킬 사용 이벤트 메소드
}

