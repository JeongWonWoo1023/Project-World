using UnityEngine;

[CreateAssetMenu(fileName = "SaleItem", menuName = "Shop/SaleItem")]
public class SaleItemData : ScriptableObject
{
    [SerializeField] private int productCount;
    [field: SerializeField] public Item[] Item { get; private set; }
    [field: SerializeField] public int[] LimitSaleCount { get; private set; }
    [field: SerializeField] public int[] Price { get; private set; }

    private void OnValidate()
    {
        Item = new Item[productCount];
        LimitSaleCount = new int[productCount];
        Price = new int[productCount];
    }
}
