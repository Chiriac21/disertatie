using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoGrabApple : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The exoskeleton root GameObject")]
    [SerializeField] private GameObject exoskeleton;

    [Tooltip("The exoskeleton joints that should move")]
    [SerializeField] private GameObject joint2DynamicElements;
    [SerializeField] private GameObject joint3DynamicElements;
    [SerializeField] private GameObject joint4DynamicElements;
    [SerializeField] private GameObject joint5DynamicElements;

    [Tooltip("Apple prefab to spawn")]
    [SerializeField] private GameObject applePrefab;

    [Tooltip("Spawn point for apple (child of exoskeleton)")]
    [SerializeField] private Transform appleSpawnPoint;

    [Header("Placement Settings")]
    [Tooltip("Distance in front of the user (for exo initial placement)")]
    [SerializeField] private float forwardOffset = 0.5f;
    [Tooltip("Horizontal offset to the right side of the user (for exo initial placement)")]
    [SerializeField] private float rightOffset = 0.3f;
    [Tooltip("Vertical offset (up/down relative to head)")]
    [SerializeField] private float verticalOffset = -0.2f;

    [Header("Joint Target Angles (degrees)")]
    public float joint2TargetAngle = 30f;
    public float joint3TargetAngle = 45f;
    public float joint4TargetAngle = 60f;
    public float joint5TargetAngle = 20f;

    [Header("Animation Settings")]
    [Tooltip("Duration of each joint rotation")]
    public float jointMoveDuration = 1.0f;
    [Tooltip("Pause time between joints")]
    public float pauseBetweenJoints = 0.3f;

    private Quaternion basePosJoint2;
    private Quaternion basePosJoint3;
    private Quaternion basePosJoint4;
    private Quaternion basePosJoint5;

    private Transform headTransform;
    private bool isGrabbing = false;
    private bool isInitialized = false; // track if exo + apple were placed
    private GameObject currentApple;

    void Start()
    {
        headTransform = Camera.main.transform;

        // Store initial rotations
        basePosJoint2 = joint2DynamicElements.transform.localRotation;
        basePosJoint3 = joint3DynamicElements.transform.localRotation;
        basePosJoint4 = joint4DynamicElements.transform.localRotation;
        basePosJoint5 = joint5DynamicElements.transform.localRotation;

        if (exoskeleton != null)
            exoskeleton.SetActive(false); // start hidden
    }

    /// <summary>
    /// Called by your menu button
    /// </summary>
    public void TriggerGrabSequence()
    {
        if (!isGrabbing)
        {
            StartCoroutine(GrabRoutine());
        }
    }

    private IEnumerator GrabRoutine()
    {
        isGrabbing = true;

        // First time only: enable exo and place it
        if (!isInitialized && exoskeleton != null)
        {
            exoskeleton.SetActive(true);
            PlaceExoskeleton();
            isInitialized = true;
        }

        // First time only: spawn apple
        if (currentApple == null && applePrefab != null && appleSpawnPoint != null)
        {
            currentApple = Instantiate(applePrefab, appleSpawnPoint.position, appleSpawnPoint.rotation, exoskeleton.transform);
        }

        // Move each joint sequentially
        yield return RotateJoint(joint2DynamicElements, basePosJoint2, joint2TargetAngle);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return RotateJoint(joint3DynamicElements, basePosJoint3, joint3TargetAngle);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return RotateJoint(joint4DynamicElements, basePosJoint4, joint4TargetAngle);
        yield return new WaitForSeconds(pauseBetweenJoints);

        yield return RotateJoint(joint5DynamicElements, basePosJoint5, joint5TargetAngle);
        yield return new WaitForSeconds(0.5f); // hold grab pose

        // Return to base pose
        yield return RotateJoint(joint5DynamicElements, joint5DynamicElements.transform.localRotation, -joint5TargetAngle, basePosJoint5);
        yield return RotateJoint(joint4DynamicElements, joint4DynamicElements.transform.localRotation, -joint4TargetAngle, basePosJoint4);
        yield return RotateJoint(joint3DynamicElements, joint3DynamicElements.transform.localRotation, -joint3TargetAngle, basePosJoint3);
        yield return RotateJoint(joint2DynamicElements, joint2DynamicElements.transform.localRotation, -joint2TargetAngle, basePosJoint2);

        isGrabbing = false;
    }

    private IEnumerator RotateJoint(GameObject joint, Quaternion baseRotation, float targetAngle, Quaternion? finalRotationOverride = null)
    {
        Quaternion startRotation = joint.transform.localRotation;
        Quaternion endRotation = finalRotationOverride ?? Quaternion.Euler(
            baseRotation.eulerAngles.x,
            baseRotation.eulerAngles.y,
            baseRotation.eulerAngles.z + targetAngle
        );

        float elapsed = 0f;
        while (elapsed < jointMoveDuration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / jointMoveDuration);
            joint.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        joint.transform.localRotation = endRotation;
    }

    /// <summary>
    /// Places the exoskeleton near the right side of the user's gaze
    /// (only once, on first use)
    /// </summary>
    private void PlaceExoskeleton()
    {
        if (headTransform == null || exoskeleton == null) return;

        Vector3 targetPosition = headTransform.position + headTransform.forward * forwardOffset;
        targetPosition += headTransform.right * rightOffset;
        targetPosition.y += verticalOffset;

        exoskeleton.transform.position = targetPosition;

        Vector3 lookDir = headTransform.forward;
        lookDir.y = 0; // keep upright
        if (lookDir.sqrMagnitude > 0.001f)
        {
            exoskeleton.transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}
