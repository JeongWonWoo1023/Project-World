using System.Collections.Generic;
using UnityEngine;

// 플레이어 실시간 데이터
public class PlayerCurrentData : CharacterCurrentData
{
    public Vector2 MovementInput { get; set; } // 이동 입력 값

    public List<PlayerCameraRecenteringData> SidewayCameraRecenteringData { get; set; }
    public List<PlayerCameraRecenteringData> BackwardCameraRecenteringData { get; set; }

    public bool IsWalk { get; set; }
    public bool IsSprint { get; set; }
    public bool IsDash { get; set; }
    public bool IsJump { get; set; }

    public Vector3 JumpForce { get; set; } // 현재 점프력

    public RotationData RotationData { get; set; } // 현재 방향 데이터
}

