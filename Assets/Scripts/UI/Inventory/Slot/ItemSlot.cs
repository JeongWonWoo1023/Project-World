using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [field: Header("슬롯 구성요소")]
    [field: SerializeField] public Image Icon { get; private set; }
    [field: SerializeField] public Button Button { get; private set; }
    [field: SerializeField] public TMPro.TMP_Text CountText { get; private set; }
    [field: SerializeField] public Item Item { get; set; }
    [field: SerializeField] public int Index { get; set; }

    private int _itemCount;
    public int ItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            if(ItemCount <= 0)
            {
                _itemCount = 0;
                gameObject.SetActive(false);
                return;
            }
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            if(ItemCount < 2)
            {
                CountText.text = string.Empty;
                return;
            }
            CountText.text = ItemCount.ToString();
        }
    }
}
