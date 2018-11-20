//--------------------------------------------------------------------------------
/*
 * This Script is used to Record Video Rating Data.
 * 
 * Used in End Scene, attached to "Script GameObject" in Hierarchy.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (MonoBehaviour, Header, GameObject)
using System.IO; // For StreamWriter, File

public class QuestionnaireInput : MonoBehaviour
{
    // Questionnaire objects
    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Questionnaire;
    private string videoUrl;

    // Reference to DataCollector
    private DataCollector dataCollector;

    public GameObject Ending;

    // Runs at the start of first frame
    private void Start()
    {
        // If DataCollector is alive
        if (DataCollector.Instance != null)
        {
            // Set reference to DataCollector Instance
            dataCollector = DataCollector.Instance;

            // Applies Video Url chosen to play in Main Scene
            videoUrl = dataCollector.videoUrl;
        }
    }

    // Records Rating Data into csv file
    public void RecordData(int ChoiceIndex)
    {
        // Open csv file at the path return from GetCSVPath()
        StreamWriter sw = File.AppendText(dataCollector.GetCSVPath(DataCollector.DataToRecord.RatingVideo, dataCollector.currentFolderPath));
        string rating = ChoiceIndex.ToString();

        // Write onto the file
        sw.WriteLine(videoUrl + "," + rating);

        // Close the file
        sw.Close();

        // Hide Questionnaire
        Questionnaire.SetActive(false);

        // Show Ending
        Ending.SetActive(true);
    }
}
