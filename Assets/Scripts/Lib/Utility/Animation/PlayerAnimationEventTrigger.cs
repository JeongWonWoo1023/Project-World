using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEventTrigger : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = transform.parent.GetComponent<Player>();
    }

    public void TriggerOnMovementStateAnimationEnter()
    {
        if(IsInAnimationTrasition())
        {
            return;
        }
        player.OnMovementStateAnimationEnter();
    }

    public void TriggerOnMovementStateAnimationExit()
    {
        if (IsInAnimationTrasition())
        {
            return;
        }
        player.OnMovementStateAnimationExit();
    }

    public void TriggerOnMovementStateAnimationTransition()
    {
        player.OnMovementStateAnimationTransition();
    }

    private bool IsInAnimationTrasition(int index = 0)
    {
        return player.Anim.IsInTransition(index);
    }
}
