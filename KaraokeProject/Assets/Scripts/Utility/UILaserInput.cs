//--------------------------------------------------------------------------------
/*
 * This Script is used for interacting with TV Control Canvas in Main Scene.
 * Allow Inputs to : Interact with Menu to choose song to play
 * 
 * Used in Main Scene, attached to "Controller(right)" of [CameraRig] in Hierarchy.
 * Used in End Scene, attached to "Controller(left/right)" of [CameraRig] in Hierarchy.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (RequireComponent, MonoBehaviour, GetComponent, GetComponentInParent Debug)
using UnityEngine.UI; // For Button
using UnityEngine.EventSystems; // For EventSystem, ExecuteEvents, PointerEventData

// This automatically create and add the Component stated to the attached GameObject
[RequireComponent(typeof(SteamVR_LaserPointer))]
public class UILaserInput : MonoBehaviour
{
    // Reference to LaserPointer Component
    private SteamVR_LaserPointer laserPointer;

    // Reference to Controller
    private SteamVR_TrackedController trackedController;

    private void OnEnable()
    {
        // Initialise LaserPointer reference
        laserPointer = GetComponent<SteamVR_LaserPointer>();

        // Listeners for laser pointer moving in/out of object and calls Relevant Functions as needed
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;

        // Initialise Controller reference
        trackedController = GetComponent<SteamVR_TrackedController>();

        // Get Compont in Parent in case attached to wrong GameObject(Model, child of Controller(left/right) in the [CameraRig] prefab)
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }

        // Listeners for Trigger click and calls Relevant Function as needed
        trackedController.TriggerClicked -= HandleTriggerClicked;
        trackedController.TriggerClicked += HandleTriggerClicked;
    }

    // Executes the event added in the Button UI
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        // Ensure the selected GameObject has Event call set
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            // Execute the Event call set in Inspector and call related function/s
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    // Selects the Button UI when Laser Pointer hit it
    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
            Debug.Log("HandlePointerIn", e.target.gameObject);
        }
    }

    // De-Select object when Laser Pointer moved out
    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }
}