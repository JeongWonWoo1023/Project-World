using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Developer : Jeong Won Woo
// Create : 2022. 12. 06.
// Update : 2022. 12. 06.

public interface IPlayerMovement : IMovement
{
    // 월드 이동벡터 초기화
    void SetMovement(in Vector3 worldDirection, bool isRunning);

    // 점프 시도
    bool TryJump();

    // 넉백
    void KnockBack(in Vector3 force, float time);
}

public interface IEnemyMovement : IMovement
{

}

public interface IMovement
{
    // 이동 여부 반환
    bool IsMoving();

    // 지면 접지 여부 반환
    bool IsGrounded();

    // 지면과의 거리 반환
    float GetDistanceFormGround();

    // 이동 중지
    void StopMove();
}

