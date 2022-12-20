using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BehaviorTree.NodeCreator;

public class EnemyBehavior : EnemyRequire
{
    private float groundDistance;

    private Vector3 direction = Vector3.zero;
    private float distance = 0.0f;
    private int currentPath = 0; // 현재 목표지점

    private void Awake()
    {
        InitializeComponenet();
        SetBehavior();
    }
    private void Update()
    {
        Compo.behaviorRoot.Run();
    }
    protected override void SetBehavior()
    {
        Compo.behaviorRoot = 
            Selection(
                // 이동하는 조건에 따라 적 개체가 이동 (조건, 참 수행, 거짓 수행)
                DualConTesk(() => { return State.isHit; },
                () => {
                    distance = Vector3.Distance(movePath[currentPath], transform.position);
                    if (Mathf.Approximately(distance,0.0f))
                    {
                        ++currentPath;
                    }
                    else
                    {
                        direction = (movePath[currentPath] - transform.position).normalized;
                        Compo.movement.SetMovement(direction, State.isRunning);
                    }
                    State.isMoving = true;
                },
                () =>
                {
                    Compo.movement.StopMove();
                    State.isMoving = false;
                })
            );
    }
}
