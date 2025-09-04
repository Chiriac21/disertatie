using UnityEngine;
using UnityEngine.UI; // For standard UI Text
using TMPro;          // For TextMeshPro

public class ToggleMenuFollow : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The menu you want to toggle follow behavior on.")]
    public GameObject menu;

    [Tooltip("The exoskeleton.")]
    public GameObject exoSkeleton;

    [Tooltip("The button text that shows Follow/Unfollow.")]
    public TMP_Text tmpText;

    [Tooltip("ExoSkeleton Manager.")]
    [SerializeField]
    private GameObject exoManager;

    [Header("Settings")]
    [Tooltip("Distance in front of the user.")]
    public float followDistance = 1.5f;

    [Tooltip("Height offset above eye level.")]
    public float verticalOffset = -0.2f;

    [Tooltip("Horizontal offset (left/right relative to head forward).")]
    public float horizontalOffset = 0f;

    private bool isFollowing = false;
    private bool isEnabled = false;
    private Transform headTransform;
    private ControlEkoSkeleton linkToManagerScript;

    void Start()
    {
        linkToManagerScript = exoManager.GetComponent<ControlEkoSkeleton>();
        headTransform = Camera.main.transform;
        UpdateButtonLabel();
        if (menu != null)
            menu.SetActive(false);
    }

    // Call this from your UI button's OnClick event
    public void ToggleFollow()
    {
        isFollowing = !isFollowing;
        UpdateButtonLabel();
    }

    public void ToggleEnabled()
    {
        isEnabled = !isEnabled;
        menu.SetActive(isEnabled);

        if(!exoSkeleton.activeSelf)
            linkToManagerScript.ToggleExoskeleton();

        if (isEnabled && menu != null && headTransform != null)
        {
            // Instantly snap in front of user when enabled
            Vector3 targetPosition = headTransform.position + headTransform.forward * followDistance;
            targetPosition += headTransform.right * horizontalOffset;
            targetPosition.y += verticalOffset;

            menu.transform.position = targetPosition;

            // Rotate to face the user immediately
            Vector3 lookDirection = menu.transform.position - headTransform.position;
            lookDirection.y = 0;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                menu.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }

    void Update()
    {
        if (isFollowing && menu != null && headTransform != null)
        {
            // Base position directly in front
            Vector3 targetPosition = headTransform.position + headTransform.forward * followDistance;

            // Add horizontal offset (relative to head's right vector)
            targetPosition += headTransform.right * horizontalOffset;

            // Apply vertical offset
            targetPosition.y += verticalOffset;

            // Smooth move
            menu.transform.position = Vector3.Lerp(menu.transform.position, targetPosition, Time.deltaTime * 5f);

            // Rotate to face user (keeping upright)
            Vector3 lookDirection = menu.transform.position - headTransform.position;
            lookDirection.y = 0;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                menu.transform.rotation = Quaternion.Slerp(menu.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }
    }

    private void UpdateButtonLabel()
    {
        string label = isFollowing ? "Unfollow" : "Follow";

        if (tmpText != null)
            tmpText.text = label;
    }
}
