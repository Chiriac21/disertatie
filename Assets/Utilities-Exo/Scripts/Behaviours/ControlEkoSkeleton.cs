using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControlEkoSkeleton : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The exoskeleton root GameObject")]
    [SerializeField] private GameObject exoskeleton;

    [Header("UI References")]
    [Tooltip("Text label for the Enable/Disable button")]
    [SerializeField] private TMP_Text enableButtonText;
    [Tooltip("Text label for the Follow/Unfollow button")]
    [SerializeField] private TMP_Text followButtonText;

    [Header("Placement Settings")]
    [Tooltip("Distance in front of the user")]
    [SerializeField] private float forwardOffset = 0.5f;

    [Tooltip("Horizontal offset to the right side of the user")]
    [SerializeField] private float rightOffset = 0.3f;

    [Tooltip("Vertical offset (up/down relative to head)")]
    [SerializeField] private float verticalOffset = -0.2f;

    private Transform headTransform;
    private bool isFollowing = false;
    private bool isEnabled = false;

    private void Start()
    {
        headTransform = Camera.main.transform;

        if (exoskeleton != null)
            exoskeleton.SetActive(false); // Start hidden

        UpdateButtonLabels();
    }

    /// <summary>
    /// Toggle visibility of the exoskeleton and place it next to the user
    /// </summary>
    public void ToggleExoskeleton()
    {
        isEnabled = !isEnabled;

        if (exoskeleton != null)
        {
            exoskeleton.SetActive(isEnabled);

            if (isEnabled)
            {
                PlaceNextToUser();
            }
        }

        UpdateButtonLabels();
    }

    /// <summary>
    /// Toggle follow mode (on/off)
    /// </summary>
    public void ToggleFollow()
    {
        isFollowing = !isFollowing;
        UpdateButtonLabels();
    }

    private void Update()
    {
        if (isFollowing && isEnabled && exoskeleton != null)
        {
            PlaceNextToUser();
        }
    }

    /// <summary>
    /// Places the exoskeleton near the right side of the user's gaze
    /// </summary>
    private void PlaceNextToUser()
    {
        if (headTransform == null) return;

        // Start from in front of the head
        Vector3 targetPosition = headTransform.position + headTransform.forward * forwardOffset;

        // Add right-hand side offset
        targetPosition += headTransform.right * rightOffset;

        // Add vertical offset
        targetPosition.y += verticalOffset;

        // Apply position
        exoskeleton.transform.position = targetPosition;

        // Rotate to face the same way as the user
        Vector3 lookDir = headTransform.forward;
        lookDir.y = 0; // keep upright
        if (lookDir.sqrMagnitude > 0.001f)
        {
            exoskeleton.transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }

    /// <summary>
    /// Updates button labels
    /// </summary>
    private void UpdateButtonLabels()
    {
        if (enableButtonText != null)
            enableButtonText.text = isEnabled ? "Disable ExoSkeleton" : "Enable ExoSkeleton";

        if (followButtonText != null)
            followButtonText.text = isFollowing ? "Toggle Exo Unfollow" : "Toggle Exo Follow";
    }
}
