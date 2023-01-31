using UnityEngine;

public class MovementMathUtillity
{
    // 대상의 수평 가속도 반환
    public Vector3 GetHorizontalVelocity(Vector3 velocity)
    {
        Vector3 result = velocity;
        result.y = 0.0f;
        return result;
    }

    // 대상의 수직 가속도 반환
    public Vector3 GetVerticalVelocity(Vector3 velocity)
    {
        Vector3 result = velocity;
        result.x = result.z = 0.0f;
        return result;
    }

    // 아크 탄젠트를 이용한 이동하고자 하는 방향의 각도값 반환 ( 0 ~ 360 사이 반환 )
    public float GetTargetAtanAngle(Vector3 direction)
    {
        float result = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (result < 0.0f)
        {
            result += 360.0f;
        }

        return result;
    }

    // 회전 값 합연산 ( 0 ~ 360 사이 반환 )
    public void AddRotationAngle(ref float original, float value)
    {
        float temp = original + value;
        if (temp > 360.0f)
        {
            temp -= 360.0f;
        }
        original = temp;
    }

    // 목표지점 방향값 반환
    public Vector3 GetTargetRotation(float angle)
    {
        return Quaternion.Euler(0.0f, angle, 0.0f) * Vector3.forward;
    }

    // 이동중인지 반환
    public bool IsMoving(Vector3 velocity, float minValue = 0.1f)
    {
        Vector3 temp = GetHorizontalVelocity(velocity);
        Vector2 movement = new Vector2(velocity.x, velocity.z);
        return movement.magnitude > minValue;
    }

    // 상향이동 여부 반환
    public bool IsMovingUp(Vector3 velocity, float minValue = 0.1f)
    {
        return GetVerticalVelocity(velocity).y > minValue;
    }

    // 하향이동 여부 반환
    public bool IsMovingDown(Vector3 velocity, float minValue = 0.1f)
    {
        return GetVerticalVelocity(velocity).y < -minValue;
    }
}
