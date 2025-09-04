using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI; // Needed for PinchSlider
using System.Collections;

public class ExoMovements : MonoBehaviour
{
    [Header("Joint References")]
    [SerializeField] private GameObject joint2DynamicElements;
    [SerializeField] private GameObject joint3DynamicElements;
    [SerializeField] private GameObject joint4DynamicElements;
    [SerializeField] private GameObject joint5DynamicElements;

    [Header("MRTK Sliders")]
    [SerializeField] private PinchSlider joint2Slider;
    [SerializeField] private PinchSlider joint3Slider;
    [SerializeField] private PinchSlider joint4Slider;
    [SerializeField] private PinchSlider joint5Slider;

    private Quaternion basePosJoint2;
    private Quaternion basePosJoint3;
    private Quaternion basePosJoint4;
    private Quaternion basePosJoint5;

    [Header("Rotation Settings")]
    [Tooltip("Maximum Z rotation in degrees")]
    public float maxRotation = 45f;
    [Tooltip("Minimum Z rotation in degrees")]
    public float minRotation = -45f;

    [Header("Reset Settings")]
    [Tooltip("How long each joint takes to reset (seconds)")]
    public float resetDuration = 1f;
    [Tooltip("Pause between resetting joints")]
    public float pauseBetweenJoints = 0.3f;

    private bool isResetting = false;

    void Start()
    {
        // Save the base positions
        basePosJoint2 = joint2DynamicElements.transform.localRotation;
        basePosJoint3 = joint3DynamicElements.transform.localRotation;
        basePosJoint4 = joint4DynamicElements.transform.localRotation;
        basePosJoint5 = joint5DynamicElements.transform.localRotation;

        // Hook up slider events
        if (joint2Slider != null) joint2Slider.OnValueUpdated.AddListener(UpdateJoint2Rotation);
        if (joint3Slider != null) joint3Slider.OnValueUpdated.AddListener(UpdateJoint3Rotation);
        if (joint4Slider != null) joint4Slider.OnValueUpdated.AddListener(UpdateJoint4Rotation);
        if (joint5Slider != null) joint5Slider.OnValueUpdated.AddListener(UpdateJoint5Rotation);
    }

    private void UpdateJoint2Rotation(SliderEventData data)
    {
        float angle = Mathf.Lerp(minRotation, maxRotation, data.NewValue);
        joint2DynamicElements.transform.localRotation =
            Quaternion.Euler(basePosJoint2.eulerAngles.x,
                             basePosJoint2.eulerAngles.y,
                             basePosJoint2.eulerAngles.z + angle);
    }

    private void UpdateJoint3Rotation(SliderEventData data)
    {
        float angle = Mathf.Lerp(minRotation, maxRotation, data.NewValue);
        joint3DynamicElements.transform.localRotation =
            Quaternion.Euler(basePosJoint3.eulerAngles.x,
                             basePosJoint3.eulerAngles.y,
                             basePosJoint3.eulerAngles.z + angle);
    }

    private void UpdateJoint4Rotation(SliderEventData data)
    {
        float angle = Mathf.Lerp(minRotation, maxRotation, data.NewValue);
        joint4DynamicElements.transform.localRotation =
            Quaternion.Euler(basePosJoint4.eulerAngles.x,
                             basePosJoint4.eulerAngles.y,
                             basePosJoint4.eulerAngles.z + angle);
    }

    private void UpdateJoint5Rotation(SliderEventData data)
    {
        float angle = Mathf.Lerp(minRotation, maxRotation, data.NewValue);
        joint5DynamicElements.transform.localRotation =
            Quaternion.Euler(basePosJoint5.eulerAngles.x,
                             basePosJoint5.eulerAngles.y,
                             basePosJoint5.eulerAngles.z + angle);
    }


    /// <summary>
    /// Public method you can link to a button to reset all joints sequentially
    /// </summary>
    public void ResetJointsSequentially()
    {
        if (!isResetting)
            StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        isResetting = true;

        // Disable sliders during reset
        SetSlidersInteractable(false);

        yield return ResetJoint(joint2DynamicElements, basePosJoint2, joint2Slider);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return ResetJoint(joint3DynamicElements, basePosJoint3, joint3Slider);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return ResetJoint(joint4DynamicElements, basePosJoint4, joint4Slider);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return ResetJoint(joint5DynamicElements, basePosJoint5, joint5Slider);

        // Re-enable sliders after reset
        SetSlidersInteractable(true);

        isResetting = false;
    }

    private IEnumerator ResetJoint(GameObject joint, Quaternion baseRotation, PinchSlider slider)
    {
        Quaternion startRotation = joint.transform.localRotation;
        Quaternion endRotation = baseRotation;

        float elapsed = 0f;
        while (elapsed < resetDuration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / resetDuration);
            joint.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final
        joint.transform.localRotation = endRotation;

        // Reset slider value visually
        if (slider != null) slider.SliderValue = 0.5f;
    }

    /// <summary>
    /// Enable or disable interaction with all sliders
    /// </summary>
    private void SetSlidersInteractable(bool state)
    {
        if (joint2Slider != null) joint2Slider.enabled = state;
        if (joint3Slider != null) joint3Slider.enabled = state;
        if (joint4Slider != null) joint4Slider.enabled = state;
        if (joint5Slider != null) joint5Slider.enabled = state;
    }
}