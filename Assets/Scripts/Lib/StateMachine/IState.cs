using System.Collections;
// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

public interface IState
{
    public void Enter();
    public void Exit();
    public void HandleInput();
    public void Process();
    public void PhysicalProcess();
}
