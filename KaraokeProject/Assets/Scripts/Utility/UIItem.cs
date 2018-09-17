using UnityEngine;

// Automatically creates BoxCollider component of the Object size and add onto itself
[RequireComponent(typeof(RectTransform))]
public class UIItem : MonoBehaviour
{
    private BoxCollider boxCollider;
    private RectTransform rectTransform;

    private void OnEnable()
    {
        ValidateCollider();
    }

    private void OnValidate()
    {
        ValidateCollider();
    }

    private void ValidateCollider()
    {
        rectTransform = GetComponent<RectTransform>();

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Fit size
        boxCollider.size = rectTransform.sizeDelta;
    }
}