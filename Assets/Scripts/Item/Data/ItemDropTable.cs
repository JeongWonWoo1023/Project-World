using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DropTable", menuName = "Item/DropTable")]
public class ItemDropTable : ScriptableObject
{
    [field: SerializeField] public Item[] DropItem { get; private set; }
    [field: SerializeField] public int[] DropCount { get; private set; }
    [field: SerializeField] public int[] DropRatio { get; private set; }
}
