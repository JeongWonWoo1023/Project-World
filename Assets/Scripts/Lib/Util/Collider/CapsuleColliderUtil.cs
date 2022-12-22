using System;
using UnityEngine;

[Serializable]
public class CapsuleColliderUtil
{
    public CapsuleColliderData ColliderData { get; private set; }
    [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }
    [field: SerializeField] public SlopeData SlopeData { get; private set; }

    public void Initialize(GameObject obj)
    {
        if (ColliderData != null) return;
        CalColliderData(obj);
        ColliderData = new CapsuleColliderData();
        ColliderData.Initialize(obj);
    }

    public void CalCollierDimension()
    {
        SetCapsuleColliderRadius(DefaultColliderData.Radius);
        SetCapsuleColliderHeight(DefaultColliderData.Height * (1.0f - SlopeData.StepSlopeHeight));

        ResetColliderCenter();

        float halfHeight = ColliderData.Collider.height * 0.5f;
        if (halfHeight < ColliderData.Collider.radius)
        {
            SetCapsuleColliderRadius(halfHeight);
        }

        ColliderData.SetColliderData();
    }

    public void ResetColliderCenter()
    {
        float difference = DefaultColliderData.Height - ColliderData.Collider.height;
        Vector3 newColliderCenter = new Vector3(0.0f, DefaultColliderData.CenterY + (difference * 0.5f), 0.0f);
        ColliderData.Collider.center = newColliderCenter;
    }

    public void SetCapsuleColliderRadius(float radius)
    {
        ColliderData.Collider.radius = radius;
    }

    public void SetCapsuleColliderHeight(float height)
    {
        ColliderData.Collider.height = height;
    }

    // 캡슐 콜라이더 컴포넌트 설정
    public void CalColliderData(GameObject obj)
    {
        CapsuleCollider collider = obj.GetComponent<CapsuleCollider>();
        if (collider == null)
        {
            collider = obj.AddComponent<CapsuleCollider>();

            float maxHeight = -1.0f;

            SkinnedMeshRenderer[] skinMeshs = obj.GetComponentsInChildren<SkinnedMeshRenderer>(); // 스킨 메시 랜더러 컴포넌트 모두 불러오기

            // 컴포넌트가 하나라도 있는 경우
            if (skinMeshs.Length > 0)
            {
                foreach (SkinnedMeshRenderer mesh in skinMeshs)
                {
                    foreach (Vector3 vertex in mesh.sharedMesh.vertices)
                    {
                        if (maxHeight < vertex.y)
                        {
                            maxHeight = vertex.y;
                        }
                    }
                }
            }
            // 스킨 메시 랜더러가 없을 경우
            else
            {
                MeshFilter[] meshFIlters = obj.GetComponentsInChildren<MeshFilter>(); // 메시 필터 컴포넌트 모두 불러오기

                // 컴포넌트가 하나라도 있는 경우
                if (meshFIlters.Length > 0)
                {
                    foreach (MeshFilter mesh in meshFIlters)
                    {
                        foreach (Vector3 vertex in mesh.mesh.vertices)
                        {
                            if (maxHeight < vertex.y)
                            {
                                maxHeight = vertex.y;
                            }
                        }
                    }
                }
            }
            if (maxHeight <= 0)
            {
                maxHeight = 1.0f;
            }
            DefaultColliderData.SetDefaultData(maxHeight, (Vector3.up * (maxHeight * 0.5f)).y, 0.2f);
        }
    }
}
