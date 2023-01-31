using System;
using UnityEngine;

[Serializable]
public class LayerData
{
    [field: SerializeField] public LayerMask GroundMask { get; private set; }
    public bool ConTainsLayer(LayerMask layerMask, int layer)
    {
        return (1 << layer & layerMask) != 0;
    }
    public bool IsGrounded(int layer)
    {
        return ConTainsLayer(GroundMask, layer);
    }
}
