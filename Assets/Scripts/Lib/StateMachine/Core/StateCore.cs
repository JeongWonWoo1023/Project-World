using UnityEditorInternal;
using UnityEngine;

public class StateCore : IState
{
    #region IState 인터페이스 메소드
    public virtual void Enter()
    {
        Debug.Log($"Current Enemy State : {GetType().Name}"); // 해당 상태명 출력
    }

    public virtual void Exit()
    {

    }

    public virtual void Process()
    {

    }

    public virtual void PhysicalProcess()
    {

    }

    public virtual void HandleInput()
    {

    }

    public virtual void OnAnimationEnter()
    {

    }

    public virtual void OnAnimationExit()
    {

    }

    public virtual void OnAnimationTransition()
    {

    }

    public virtual void OnTriggerEnter(Collider collider)
    {

    }

    public virtual void OnTriggerStay(Collider collider)
    {

    }

    public virtual void OnTriggerExit(Collider collider)
    {

    }
    #endregion

    #region 재사용 메소드
    // 모션 시작
    protected virtual void StartAnimation(int hash)
    {

    }

    // 모션 종료
    protected virtual void StopAnimation(int hash)
    {

    }

    // int값에 의한 모션 시작
    protected virtual void SetAnimationValueFromHash(int hash, int value)
    {

    }
    #endregion
}
