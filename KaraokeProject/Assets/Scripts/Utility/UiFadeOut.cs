//--------------------------------------------------------------------------------
/*
 * This Script is used to Fade out Ending Canvas.
 * Allow Input to : Skip Ending.
 * 
 * Used in End Scene, attached to "Script GameObject" in Hierarchy.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (MonoBehaviour, GameObject, Header, Tooltip, Debug, Color, Time, Input, KeyCode)
using UnityEngine.UI; // For Image, Text

public class UiFadeOut : MonoBehaviour
{
    // Rate of Fading out
    [Tooltip("Rate of Fading. Higher means fade out faster;Lower means fade out slower. Default: 0.15f")]
    public float FadingRate = 0.15f;

    // Reference to Ending and its child/components
    public GameObject Ending;
    private Image Ending_Panel;
    private Text Ending_Text;

    // Runs at the start of first frame
    private void Start ()
    {
        // Ensure there's Ending to set
        if (!Ending)
        {
            Debug.LogError("Ending GameObject not found.");
            return;
        }
        else
        {
            // Get the Components
            Ending_Panel = Ending.transform.Find("Ending Panel").GetComponent<Image>();
            Ending_Text = Ending.transform.Find("Ending Panel").Find("Text").GetComponent<Text>();
        }
    }

    // Returns decreased Alpha Component of Color
    private Color Fade(Color color)
    {
        Color Temp = color;
        if (Temp.a > 0.0f)
        {
            // Fade accoding to Fading Rate
            Temp.a -= FadingRate * Time.deltaTime;
        }
        else
        {
            Temp.a = 0.0f;
        }
        return Temp;
    }

	// Update is called once per frame
	void Update ()
    {
        // When Ending is active
        if (Ending.activeSelf)
        {
            // Proceed to Fade
            Ending_Panel.color = Fade(Ending_Panel.color);
            Ending_Text.color = Fade(Ending_Text.color);

            // When finish fading out
            if (Ending_Panel.color.a == 0.0f && Ending_Text.color.a == 0.0f)
            {
                // Hide Ending
                Ending.SetActive(false);
#if UNITY_EDITOR
                // Stops running Application
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quit Application
                Application.Quit();
#endif
            }

            // Skip Ending and Quit Application
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Hide Ending
                Ending.SetActive(false);
#if UNITY_EDITOR
                // Stops running Application
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // Quit Application
                Application.Quit();
#endif
            }
        }
    }
}
