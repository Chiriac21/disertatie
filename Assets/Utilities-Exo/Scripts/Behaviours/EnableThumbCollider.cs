using UnityEngine;

public class EnableThumbCollider : MonoBehaviour
{
    public Collider thumbCollider;

    void LateUpdate()
    {
        if (thumbCollider != null && !thumbCollider.enabled)
        {
            thumbCollider.enabled = true;
            enabled = false;
        }
    }
}
