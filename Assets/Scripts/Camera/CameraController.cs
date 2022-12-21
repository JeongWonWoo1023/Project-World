// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

using Cinemachine;
using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, Range(0.0f, 10.0f), Tooltip("기본 카메라 거리")] private float defaultDistance = 6.0f;
    [SerializeField, Range(0.0f, 10.0f), Tooltip("최소 카메라 거리")] private float minDistance = 1.0f;
    [SerializeField, Range(0.0f, 10.0f), Tooltip("최대 카메라 거리")] private float maxDiatance = 6.0f;

    [SerializeField, Range(0.0f, 10.0f)] private float lerpCoef = 4.0f; // 보간 가중치
    [SerializeField, Range(0.0f, 10.0f)] private float wheelCoef = 1.0f; // 마우스 휠 값 가중치

    private CinemachineFramingTransposer framingTransposer = null; // 카메라 이동에 관련한 값에 접근 가능한 클래스
    private CinemachineInputProvider inputProvider = null; // 카메라 입력 관리자

    private float currentDistance; // 현재 거리

    private void Awake()
    {
        framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        inputProvider = GetComponent<CinemachineInputProvider>();

        currentDistance = defaultDistance; // 기본 거리로 초기화
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        float zoomValue = inputProvider.GetAxisValue(2) * wheelCoef; // 정의됭 마우스 휠값과 가중치의 곱
        currentDistance = Mathf.Clamp(currentDistance + zoomValue, minDistance, maxDiatance); // 거리 제한

        float distTemp = framingTransposer.m_CameraDistance; // 현재 카메라의 거리와 같다면 리턴하기위해 캐싱

        if (distTemp == currentDistance) return;

        float lerpValue = Mathf.Lerp(distTemp, currentDistance, lerpCoef * Time.deltaTime); // 카메라 이동 보간

        framingTransposer.m_CameraDistance = lerpValue; // 줌 이동 거리값 적용
    }
}
