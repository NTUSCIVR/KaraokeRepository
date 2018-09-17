using UnityEngine;
using System.IO;

public class QuestionnaireInput : MonoBehaviour
{
    public GameObject Ending;

    // Questionnaire objects
    public GameObject QuestionnaireObject;
    private DataCollector dataCollector;
    private string videoUrl;

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
        QuestionnaireObject.SetActive(false);
        Ending.SetActive(true);
    }
}
