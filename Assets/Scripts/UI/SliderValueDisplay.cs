using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(UpdateValue);
        UpdateValue(slider.value);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(UpdateValue);
    }

    private void UpdateValue(float val)
    {
        text.text = Mathf.RoundToInt(val).ToString();
    }
}
