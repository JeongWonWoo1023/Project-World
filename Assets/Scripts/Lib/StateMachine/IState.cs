using UnityEngine;
// Developer : Jeong Won Woo
// Create : 2022. 12. 21.
// Update : 2022. 12. 21.

public interface IState : ITrigger
{
    public void Enter(); // 상태 진입 시 호출
    public void Exit(); // 상태 전환 직전 호출
    public void HandleInput(); // 입력 처리
    public void Process(); // 프레임 프로세스
    public void PhysicalProcess(); // 물리 프로세스
    public void OnAnimationEnter();
    public void OnAnimationExit();
    public void OnAnimationTransition();

}

public interface ITrigger
{
    public void OnTriggerEnter(Collider collider);
    public void OnTriggerStay(Collider collider);
    public void OnTriggerExit(Collider collider);
}

