using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DropTable", menuName = "Item/DropTable")]
public class ItemDropTable : ScriptableObject
{
    [field: SerializeField] public Item[] DropItem { get; private set; }
    [field: SerializeField] public int[] DropCount { get; private set; }
    [field: SerializeField] public int[] DropRatio { get; private set; }
    [field: SerializeField] public int DropGold { get; private set; }
}
