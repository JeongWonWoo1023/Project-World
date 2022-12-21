using System.Collections;
using System.Collections.Generic;
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
}
