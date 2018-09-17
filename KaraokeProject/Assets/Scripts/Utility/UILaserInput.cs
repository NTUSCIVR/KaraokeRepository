using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class UILaserInput : MonoBehaviour
{
    private SteamVR_LaserPointer laserPointer;
    private SteamVR_TrackedController trackedController;

    private void OnEnable()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();
        // Listeners for laser pointer moving in/out of object
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;

        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        // Listeners for Trigger click
        trackedController.TriggerClicked -= HandleTriggerClicked;
        trackedController.TriggerClicked += HandleTriggerClicked;
    }

    // Executes the event added in the Button UI
    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
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