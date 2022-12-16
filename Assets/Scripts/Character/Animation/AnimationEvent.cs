using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    public UnityEvent onAttackAnim;

    public void OnAttackAnim()
    {
        onAttackAnim?.Invoke();
    }
}
