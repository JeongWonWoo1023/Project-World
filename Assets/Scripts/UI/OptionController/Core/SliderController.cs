using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SliderController : OptionController
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected TMPro.TMP_Text valueText;
}
