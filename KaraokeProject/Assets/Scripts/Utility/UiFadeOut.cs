using UnityEngine;
using UnityEngine.UI;

public class UiFadeOut : MonoBehaviour
{
    // Rate of Fading out
    public float FadingRate = 0.15f;
    
    // Reference to Instruction and its child/components
    public GameObject Instruction;
    private Image Instruc_Panel;
    private Text Instruc_Title;
    private Image Instruc_Image;
    private Text Instruc_Text;

    // Reference to Questionnaire
    public GameObject Questionnaire;

    // Reference to Ending and its child/components
    public GameObject Ending;
    private Image Ending_Panel;
    private Text Ending_Text;

    // Use this for initialization
    private void Start ()
    {
        // Ensure there's Instruction to set
        if (!Instruction)
        {
            Debug.LogError("Instructions GameObject not found.");
            return;
        }
        else
        {
            // Get the Components
            Instruc_Panel = Instruction.transform.Find("Instructions Panel").GetComponent<Image>();
            Instruc_Title = Instruction.transform.Find("Instructions Panel").Find("Title Text").GetComponent<Text>();
            Instruc_Image = Instruction.transform.Find("Instructions Panel").Find("Controls Image").GetComponent<Image>();
            Instruc_Text = Instruction.transform.Find("Instructions Panel").Find("Content Text").GetComponent<Text>();
        }

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
        // When Insturction is active
        if(Instruction.activeSelf)
        {
            // Proceed to Fade
            Instruc_Panel.color = Fade(Instruc_Panel.color);
            Instruc_Title.color = Fade(Instruc_Title.color);
            Instruc_Image.color = Fade(Instruc_Image.color);
            Instruc_Text.color = Fade(Instruc_Text.color);

            // When finish fading out
            if (Instruc_Panel.color.a == 0.0f &&
                Instruc_Title.color.a == 0.0f && Instruc_Image.color.a == 0.0f && Instruc_Text.color.a == 0.0f)
            {
                Instruction.SetActive(false);
                Questionnaire.SetActive(true);
            }

            // Skip Insturction
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Instruction.SetActive(false);
                Questionnaire.SetActive(true);
            }
        }
        // When Ending is active
        else if (Ending.activeSelf)
        {
            // Proceed to Fade
            Ending_Panel.color = Fade(Ending_Panel.color);
            Ending_Text.color = Fade(Ending_Text.color);

            // When finish fading out
            if (Ending_Panel.color.a == 0.0f && Ending_Text.color.a == 0.0f)
            {
                // Quit Application
                Ending.SetActive(false);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            // Skip Ending and Quit Application
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Ending.SetActive(false);
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
