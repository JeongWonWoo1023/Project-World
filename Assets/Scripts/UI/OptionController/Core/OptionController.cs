using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OptionController : MonoBehaviour
{
    public enum OptionCategory
    {
        None, Sound, Graphic
    }

    public OptionCategory category;
    [SerializeField] protected string saveKey;

    protected abstract void ApplyEvent();
}
