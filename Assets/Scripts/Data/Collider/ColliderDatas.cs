using System;
using UnityEngine;

public class CapsuleColliderData
{
    public CapsuleCollider Collider { get; private set; }
    public Vector3 localCenter { get; private set; }

    public void Initialize(GameObject obj)
    {
        if (Collider != null) return;
        Collider = obj.GetComponent<CapsuleCollider>();
        SetColliderData();
    }

    public void SetColliderData()
    {
        localCenter = Collider.center;
    }
}

[Serializable]
public class DefaultColliderData
{
    [field: SerializeField] public float Height { get; private set; } = 1.5f;
    [field: SerializeField] public float CenterY { get; private set; } = 0.9f;
    [field: SerializeField] public float Radius { get; private set; } = 0.2f;

    public void SetDefaultData(float _height, float _centerY, float _radius)
    {
        Height = _height;
        CenterY = _centerY;
        Radius = _radius;
    }
}

[Serializable]
public class SlopeData
{
    [field: SerializeField][field: Range(0.0f, 1.0f)] public float StepSlopeHeight { get; private set; } = 0.25f;
    [field: SerializeField][field: Range(0.0f, 5.0f)] public float RayDIstance { get; private set; } = 2.0f;
    [field: SerializeField][field: Range(0.0f, 50.0f)] public float StepReachForce { get; private set; } = 25.0f;
}

