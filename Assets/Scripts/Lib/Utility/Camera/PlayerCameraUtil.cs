using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerCameraUtil
{
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }

    [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0.0f;
    [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 4.0f;

    private CinemachinePOV cinemachinePOV;

    public void Initialize()
    {
        cinemachinePOV = VirtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    public void EnableRecentering(float waitTime = -1.0f,float recenteringTime = -1.0f, float baseMovementSpeed = 1.0f,float movementSpeed = 1.0f)
    {
        cinemachinePOV.m_HorizontalRecentering.m_enabled = true;

        cinemachinePOV.m_HorizontalRecentering.CancelRecentering();

        if(waitTime == -1.0f)
        {
            waitTime = DefaultHorizontalWaitTime;
        }

        if (recenteringTime == -1.0f)
        {
            recenteringTime = DefaultHorizontalRecenteringTime;
        }

        recenteringTime = recenteringTime * baseMovementSpeed / movementSpeed;

        cinemachinePOV.m_HorizontalRecentering.m_WaitTime = waitTime;
        cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime = recenteringTime;
    }

    public void DisableRecentering()
    {
        cinemachinePOV.m_HorizontalRecentering.m_enabled = false;
    }
}
