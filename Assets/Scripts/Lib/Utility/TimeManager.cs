using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct TimeData
{
    [SerializeField] public int hour;
    [SerializeField, Range(0, 59)] public int minute;
    [SerializeField, Range(0, 59)] public int second;
}

[Serializable]
public class TimeManager
{
    [field: SerializeField] public TimeData CoolTime { get; private set; }

    public DateTime Standard { get; private set; }
    public DateTime End { get; set; }

    public IEnumerator ApplyCoolTime(Action startAction = null, Action processAction = null, Action endAction = null)
    {
        // 쿨타임 적용 코루틴
        startAction?.Invoke();
        Standard = DateTime.Now;
        End = Standard.Add(new TimeSpan(CoolTime.hour, CoolTime.minute, CoolTime.second));
        while(DateTime.Compare(DateTime.Now, End) <= 0)
        {
            processAction?.Invoke();
            yield return null;
        }
        endAction?.Invoke();
    }

    public void ApplyCoolTime(Action startAction = null)
    {
        // 쿨타임 적용 메소드
        startAction?.Invoke();
        Standard = DateTime.Now;
        End = Standard.Add(new TimeSpan(CoolTime.hour, CoolTime.minute, CoolTime.second));
    }

    public bool IsEndCoolTime(Action endAction = null)
    {
        // 쿨타임 종료여부 반환
        if(End == default)
        {
            return true;
        }

        if(DateTime.Compare(DateTime.Now,End) > 0)
        {
            endAction?.Invoke();
            return true;
        }
        return false;
    }

    public TimeSpan GetRemainingTime()
    {
        // 남은 쿨타임 반환
        TimeSpan result = new TimeSpan(0, 0, 0);
        if (End == default)
        {
            return result;
        }
        result = DateTime.Now - End;
        return result;
    }
}
