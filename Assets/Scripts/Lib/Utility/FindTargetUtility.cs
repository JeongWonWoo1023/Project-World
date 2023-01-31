using UnityEngine;

public class FindTargetUtility : MonoBehaviour
{
    [field: SerializeField] public bool DebugMode { get; private set; } = false;
    [field: SerializeField][field: Range(0f, 360f)] public float HorizontalAngle { get; private set; } = 0.0f;
    [field: SerializeField][field: Range(0f, 360f)] public float VerticalAngle { get; private set; } = 0.0f;
    [field: SerializeField] public float MaxDistance { get; private set; } = 1.0f;
    [field: SerializeField] public SphereCollider FindCollider { get; private set; }
    [field: SerializeField] public LayerMask TargetMask { get; private set; }
    [field: SerializeField] public LayerMask ObstacleMask { get; private set; }

    public Vector3 GizmoCenter { get; private set; } = Vector3.zero;

    // 기즈모에 따른 콜라이더 범위 초기화
    private void OnValidate()
    {
        if(FindCollider.radius != MaxDistance)
        {
            FindCollider.radius = MaxDistance;
        }
    }

    // 기즈모 그리기
    private void OnDrawGizmos()
    {
        if (!DebugMode) return;
        GizmoCenter = transform.position + Vector3.up * FindCollider.center.y;
        Gizmos.DrawWireSphere(GizmoCenter, MaxDistance);

        float angle = transform.eulerAngles.y;
        Vector3 right = AngleToHorizontalDirection(transform.eulerAngles.y + HorizontalAngle * 0.5f);
        Vector3 left = AngleToHorizontalDirection(transform.eulerAngles.y - HorizontalAngle * 0.5f);
        Vector3 up = AngleToVerticalDirection(transform.eulerAngles.y + VerticalAngle * 0.5f);
        Vector3 down = AngleToVerticalDirection(transform.eulerAngles.y - VerticalAngle * 0.5f);
        Vector3 forward = AngleToHorizontalDirection(angle);

        Debug.DrawRay(GizmoCenter, right * MaxDistance, Color.red);
        Debug.DrawRay(GizmoCenter, left * MaxDistance, Color.red);
        Debug.DrawRay(GizmoCenter, up * MaxDistance, Color.red);
        Debug.DrawRay(GizmoCenter, down * MaxDistance, Color.red);
        Debug.DrawRay(GizmoCenter, forward * MaxDistance, Color.cyan);
    }

    // 각도에 따른 수평 방향값 반환
    private Vector3 AngleToHorizontalDirection(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian),0.0f,Mathf.Cos(radian));
    }

    // 각도에 따른 수직 방향값 반환
    private Vector3 AngleToVerticalDirection(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(0.0f, Mathf.Sin(radian), Mathf.Cos(radian));
    }
}
