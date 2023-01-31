using UnityEngine;

public class EnemyAnimationEventTrigger : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    public void TriggerOnMovementStateAnimationEnter()
    {
        if(IsInAnimationTrasition())
        {
            return;
        }
        enemy.OnMovementStateAnimationEnter();
    }

    public void TriggerOnMovementStateAnimationExit()
    {
        if (IsInAnimationTrasition())
        {
            return;
        }
        enemy.OnMovementStateAnimationExit();
    }

    public void TriggerOnMovementStateAnimationTransition()
    {
        enemy.OnMovementStateAnimationTransition();
    }

    private bool IsInAnimationTrasition(int index = 0)
    {
        return enemy.Anim.IsInTransition(index);
    }
}
