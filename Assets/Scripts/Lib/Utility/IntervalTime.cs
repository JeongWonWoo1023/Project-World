using System;
using UnityEngine;

public enum ContentType
{
    Day, Weak
}


[Serializable]
public struct Interval
{
    [SerializeField, Range(0, 24)] public int hour;
    [SerializeField, Range(0, 60)] public int minute;
    [SerializeField, Range(0, 60)] public int second;
}

public class IntervalTime
{
    [SerializeField] private Interval interval;
    [SerializeField] private ContentType contentType;
    public DateTime startTime;

    public bool IsCoolTime()
    {
        TimeSpan result = DateTime.Now - startTime;
        switch(contentType)
        {
            case ContentType.Day:
                return CalDate(result,0);
            case ContentType.Weak:
                return CalDate(result, 6);
            default:
                return true;
        }
    }

    private bool CalDate(TimeSpan result, int day)
    {
        if (result.Days == day)
        {
            if (result.Hours < 24)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (result.Days > day)
        {
            return true;
        }
        else
        {
            Debug.LogError("시간 데이터 에러");
            return true;
        }
    }
}
