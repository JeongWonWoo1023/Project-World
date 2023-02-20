using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;

    public void ChangeState(IState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void HandleInput() => currentState?.HandleInput();
    public void Process() => currentState?.Process();
    public void PhysicalProcess() => currentState?.PhysicalProcess();
    public void OnAnimationEnterEvent() => currentState?.OnAnimationEnter();
    public void OnAnimationExitEvent() => currentState?.OnAnimationExit();
    public void OnAnimationTransitionEvent() => currentState?.OnAnimationTransition();
    public void OnAnimationAttackEvent() => currentState?.OnAnimationAttackEvent();
    public void OnTriggerEnter(Collider collider) => currentState?.OnTriggerEnter(collider);
    public void OnTriggerStay(Collider collider) => currentState?.OnTriggerStay(collider);
    public void OnTriggerExit(Collider collider) => currentState?.OnTriggerExit(collider);
}
