using System;
using UnityEngine;

public enum ItemCategory
{
    Weapon, Usable, Booty, Extra
}

public enum ItemRank
{
    Normal, Rare, Epic, Legendary, Transcendence
}

[Serializable]
public struct ItemInfo
{
    [field: SerializeField] public ItemCategory Category { get; private set; } // 아이템 카테고리 ( 장비, 소비, 전리품, 기타 )
    [field: SerializeField] public Sprite Icon { get; private set; } // 아이템 아이콘
    [field: SerializeField] public GameObject ItemObject { get; private set; }  // 아이템 오브젝트
    [field: SerializeField] public TimeData RegenCoolTime { get; private set; }  // 리젠 쿨타임

    [field: SerializeField] public bool IsUsable { get; private set; }  // 아이템 사용 & 장착 가능 여부
    [field: SerializeField] public bool IsDropable { get; private set; }  // 필드 드랍 여부
    [field: SerializeField] public bool IsUpgradable { get; private set; }  // 업그레이드 가능 여부

    [field: SerializeField] public string Name { get; private set; }  // 아이템 이름
    [field: SerializeField] public string Type { get; private set; }  // 아이템 타입 ( 한손검, 회복물약, 버프 음식 등등 )
    [field: SerializeField] public string ItemDescription { get; private set; }  // 아이템 설명
    [field: SerializeField] public string EffectDescription { get; private set; }  // 아이템 효과 설명

    [field: SerializeField] public ItemRank Rank { get; private set; }  // 아이템 등급
    [field: SerializeField] public int Level { get; private set; }  // 아이템 업그레이드 레벨

    [field: SerializeField] public Vector2 DropValueRange { get; private set; }  // 아이템 드랍 시 개수 범위

    [field: SerializeField] public float Weight { get; private set; }  // 중량
}
