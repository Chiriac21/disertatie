using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI; // PinchSlider
using System.Collections;

public class ExoMovements : MonoBehaviour
{
    [Header("Arduino")]
    [SerializeField] private ArduinoConnector arduino;

    [Header("Joint References")]
    [SerializeField] private GameObject joint2DynamicElements; // UI: Joint 1
    [SerializeField] private GameObject joint3DynamicElements; // UI: Joint 2
    [SerializeField] private GameObject joint4DynamicElements; // UI: Joint 3 
    [SerializeField] private GameObject joint5DynamicElements; // UI: Joint 4  

    [Header("MRTK Sliders")]
    [SerializeField] private PinchSlider joint2Slider; // Joint 1
    [SerializeField] private PinchSlider joint3Slider; // Joint 2
    [SerializeField] private PinchSlider joint4Slider; // Joint 3
    [SerializeField] private PinchSlider joint5Slider; // Joint 4

    private Quaternion basePosJoint2;
    private Quaternion basePosJoint3;
    private Quaternion basePosJoint4;
    private Quaternion basePosJoint5;

    [Header("Rotation Settings")]
    public float maxRotation = 45f;
    public float minRotation = -45f;

    [Header("Reset Animation Settings")]
    public float resetDuration = 1f;
    public float pauseBetweenJoints = 0.3f;

    [Header("Joint 3 -> Arduino triggers (UI 3rd slider)")]
    [Tooltip("Value sent when Joint 3 slider reaches LEFT (0.0, negative angle).")]
    public int joint3LeftValue = 600;
    [Tooltip("Value sent when Joint 3 is at CENTER (~0.5).")]
    public int joint3ResetValue = 750;
    [Tooltip("Value sent when Joint 3 slider reaches RIGHT (1.0, positive angle).")]
    public int joint3RightValue = 800;

    [Header("Joint 4 -> Arduino triggers (UI 4th slider)")]
    [Tooltip("Value sent when Joint 4 slider reaches LEFT (0.0, negative angle).")]
    public int joint4LeftValue = 350;
    [Tooltip("Value sent when Joint 4 slider reaches RIGHT (1.0, positive angle).")]
    public int joint4RightValue = 512;
    [Tooltip("Value sent for Joint 4 when Reset is pressed.")]
    public int joint4ResetValue = 430;

    [Header("Edge detection")]
    [Range(0.0f, 0.1f)] public float edgeThreshold = 0.02f; // how close to ends to count as min/max

    private bool isResetting = false;

    // Debounce flags for min/max sends
    private bool joint3SentLeftOnce = false;
    private bool joint3SentRightOnce = false;
    private bool joint4SentLeftOnce = false;
    private bool joint4SentRightOnce = false;

    void Start()
    {
        if (arduino == null) arduino = FindObjectOfType<ArduinoConnector>();

        // Save base rotations
        basePosJoint2 = joint2DynamicElements.transform.localRotation;
        basePosJoint3 = joint3DynamicElements.transform.localRotation;
        basePosJoint4 = joint4DynamicElements.transform.localRotation;
        basePosJoint5 = joint5DynamicElements.transform.localRotation;

        // Hook slider events
        if (joint2Slider != null) joint2Slider.OnValueUpdated.AddListener(UpdateJoint2Rotation);
        if (joint3Slider != null) joint3Slider.OnValueUpdated.AddListener(UpdateJoint3Rotation);
        if (joint4Slider != null) joint4Slider.OnValueUpdated.AddListener(UpdateJoint4Rotation); 
        if (joint5Slider != null) joint5Slider.OnValueUpdated.AddListener(UpdateJoint5Rotation); 
    }

    // ---------------- Visual rotations only ----------------
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
        float v = data.NewValue; // 0..1
        float angle = Mathf.Lerp(minRotation, maxRotation, v);
        joint4DynamicElements.transform.localRotation =
            Quaternion.Euler(basePosJoint4.eulerAngles.x,
                             basePosJoint4.eulerAngles.y,
                             basePosJoint4.eulerAngles.z + angle);

        HandleJoint3Edges(v);
    }

    private void HandleJoint3Edges(float v)
    {
        // LEFT (0.0) -> U3 left value
        if (v <= edgeThreshold)
        {
            joint3SentRightOnce = false;
            if (!joint3SentLeftOnce)
            {
                joint3SentLeftOnce = true;
                if (arduino != null) arduino.SendToU3(joint3LeftValue);
                Debug.Log($"Joint 3 LEFT -> U3 {joint3LeftValue}");
            }
        }
        // RIGHT (1.0) -> U3 right value
        else if (v >= 1f - edgeThreshold)
        {
            joint3SentLeftOnce = false;
            if (!joint3SentRightOnce)
            {
                joint3SentRightOnce = true;
                if (arduino != null) arduino.SendToU3(joint3RightValue);
                Debug.Log($"Joint 3 RIGHT -> U3 {joint3RightValue}");
            }
        }
        else
        {
            joint3SentLeftOnce = false;
            joint3SentRightOnce = false;
        }
    }

    private void UpdateJoint5Rotation(SliderEventData data)
    {
        float v = data.NewValue; // 0..1
        float angle = Mathf.Lerp(minRotation, maxRotation, v);
        joint5DynamicElements.transform.localRotation =
            Quaternion.Euler(basePosJoint5.eulerAngles.x,
                             basePosJoint5.eulerAngles.y,
                             basePosJoint5.eulerAngles.z + angle);

        HandleJoint4Edges(v);
    }

    private void HandleJoint4Edges(float v)
    {
        // LEFT (0.0) -> U4 left value
        if (v <= edgeThreshold)
        {
            joint4SentRightOnce = false;
            if (!joint4SentLeftOnce)
            {
                joint4SentLeftOnce = true;
                if (arduino != null) arduino.SendToU4(joint4LeftValue);
                Debug.Log($"Joint 4 LEFT -> U4 {joint4LeftValue}");
            }
        }
        // RIGHT (1.0) -> U4 right value
        else if (v >= 1f - edgeThreshold)
        {
            joint4SentLeftOnce = false;
            if (!joint4SentRightOnce)
            {
                joint4SentRightOnce = true;
                if (arduino != null) arduino.SendToU4(joint4RightValue);
                Debug.Log($"Joint 4 RIGHT -> U4 {joint4RightValue}");
            }
        }
        else
        {
            joint4SentLeftOnce = false;
            joint4SentRightOnce = false;
        }
    }

    // ---------------- Reset flow ----------------
    public void ResetJointsSequentially()
    {
        if (!isResetting)
            StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        isResetting = true;
        SetSlidersInteractable(false);

        yield return ResetJoint(joint2DynamicElements, basePosJoint2, joint2Slider);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return ResetJoint(joint3DynamicElements, basePosJoint3, joint3Slider);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return ResetJoint(joint4DynamicElements, basePosJoint4, joint4Slider); // Joint 3
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return ResetJoint(joint5DynamicElements, basePosJoint5, joint5Slider); // Joint 4

        SetSlidersInteractable(true);
        isResetting = false;

        // After animation finishes: send BOTH default/base values again
        if (arduino != null)
        {
            arduino.SendToU3(joint3ResetValue);
            arduino.SendToU4(joint4ResetValue);
            Debug.Log($"Reset -> sent U3 {joint3ResetValue}, U4 {joint4ResetValue}");
        }
        else
        {
            Debug.LogWarning("ArduinoConnector not found; cannot send reset values.");
        }
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

        joint.transform.localRotation = endRotation;
        if (slider != null) slider.SliderValue = 0.5f;

        // Clear edge flags so next min/max triggers will fire again
        joint3SentLeftOnce = false;
        joint3SentRightOnce = false;
        joint4SentLeftOnce = false;
        joint4SentRightOnce = false;
    }

    private void SetSlidersInteractable(bool state)
    {
        if (joint2Slider != null) joint2Slider.enabled = state;
        if (joint3Slider != null) joint3Slider.enabled = state;
        if (joint4Slider != null) joint4Slider.enabled = state;
        if (joint5Slider != null) joint5Slider.enabled = state;
    }
}