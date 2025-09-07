using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro; // For TextMeshPro

public class SliderAngleDisplay : MonoBehaviour
{
    [SerializeField] private PinchSlider slider;
    [SerializeField] private TMP_Text angleLabel;  
    [SerializeField] private TMP_Text minAngleLabel;   
    [SerializeField] private TMP_Text maxAngleLabel;  
    [SerializeField] private float minAngle = -30f;
    [SerializeField] private float maxAngle = 30f;

    private void Start()
    {
        if (slider != null)
        {
            minAngleLabel.text = $"{maxAngle:F0}°";
            maxAngleLabel.text = $"{minAngle:F0}°";
            float oldValue = slider.SliderValue;
            slider.OnValueUpdated.AddListener(UpdateAngleLabel);
            UpdateAngleLabel(new SliderEventData(oldValue, slider.SliderValue, null, slider)); // initialize
        }
    }

    private void UpdateAngleLabel(SliderEventData data)
    {
        float angle = Mathf.Lerp(minAngle, maxAngle, data.NewValue);
        if (angleLabel != null)
        {
            angleLabel.text = $"{angle:F1}°"; // one decimal place
        }
    }
}
