//--------------------------------------------------------------------------------
/*
 * This Script is used on the Buttons of TV Control Canvas in Main Scene.
 * It automatically Create/Re-Create BoxCollider Component and fit it to the Size of the Button onto itself.
 * 
 * Used in Main Scene, attached to "UP/Down/Play Buttons" of TVCanvas in Hierarchy.
 * Used in End Scene, attached to "Ring Image X", where "X" is "-2 / -1 / 0 / 1 / 2" that can be found under Questionnaire in Camera(eye) of [CameraRig] in Hierarchy.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (RequireComponent, RectTransform, MonoBehaviour, BoxCollider, GetComponent, GameObject)

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

        // Creates and Add BoxCollider Component onto itself IF there is no BoxCollider Component found
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        // Fit Collder to the Size of the Button
        boxCollider.size = rectTransform.sizeDelta;
    }
}