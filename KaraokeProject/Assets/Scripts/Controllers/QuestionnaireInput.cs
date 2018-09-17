using UnityEngine;
using System.IO;

public class QuestionnaireInput : MonoBehaviour
{
    // Questionnaire objects
    [Header("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public GameObject Questionnaire;
    private DataCollector dataCollector;
    private string videoUrl;

    public GameObject Ending;

    // Use this for initialization
    private void Start()
    {
        if(DataCollector.Instance != null)
        {
            dataCollector = DataCollector.Instance;
            videoUrl = dataCollector.videoUrl;
        }
    }

    public void RecordData(int ChoiceIndex)
    {
        StreamWriter sw = File.AppendText(dataCollector.GetCSVPath(DataCollector.DataToRecord.RatingVideo));
        string rating = ChoiceIndex.ToString();
        sw.WriteLine(videoUrl + "," + rating);
        sw.Close();

        // Show Ending
        Questionnaire.SetActive(false);
        Ending.SetActive(true);
    }
}
