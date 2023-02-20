using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSkill", menuName = "Skill/PlayerSkill")]
public class SkiilData : ScriptableObject
{
    [field: SerializeField] public Effect Effect { get; private set; }
    [field: SerializeField] public int HitCount { get; private set; }
    [field: SerializeField] public float[] Damages { get; private set; }
    [field: SerializeField] public AudioClip[] Sounds { get; private set; }
    [field: SerializeField] public int Cost { get; private set; }
}
