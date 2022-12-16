using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform modelRoot;

    private Vector3 deltaPos = Vector3.zero;
    private Quaternion deltaRot = Quaternion.identity;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        playerRoot.Translate(deltaPos, Space.World);
        deltaPos = Vector3.zero;
        modelRoot.rotation *= deltaRot;
        deltaRot = Quaternion.identity;
    }

    private void OnAnimatorMove()
    {
        deltaPos += anim.deltaPosition;
        deltaRot *= anim.deltaRotation;
    }
}
