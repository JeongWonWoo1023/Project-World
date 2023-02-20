using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Item/Reward")]
public class ItemRewardScriptable : ScriptableObject
{
    [field: SerializeField] public Item[] RewardItem { get; private set; }
    [field: SerializeField] public int RewardGold { get; private set; }
    [field: SerializeField] public int RewardEXP { get; private set; }
}
