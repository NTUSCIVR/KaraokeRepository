using UnityEngine;
using System.IO;

public class QuestionnaireInput : MonoBehaviour
{
    // Reference to the object being tracked. // Controller, in this case.
    private SteamVR_TrackedObject trackedObj;
    
    public GameObject Ending;

    // Questionnaire objects
    public GameObject QuestionnaireObject;
    public GameObject Choice;
    public GameObject[] Targets;
    private int ChoiceIndex = 0;
    private DataCollector dataCollector;
    private string videoUrl;


    // Device property to provide easy access to controller.
    // Uses the tracked object's index to return the controller's input
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    // Use this for initialization
    private void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        if(DataCollector.Instance != null)
        {
            dataCollector = DataCollector.Instance;
            videoUrl = dataCollector.videoUrl;
        }
    }

    private void RecordData()
    {
        StreamWriter sw = File.AppendText(dataCollector.GetCSVPath(DataCollector.DataToRecord.RatingVideo));
        string rating = (ChoiceIndex - 2).ToString();
        sw.WriteLine(videoUrl + "," + rating);
        sw.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (QuestionnaireObject.activeSelf)
        {
            // Moving the Circle(To allow rating selection)
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
            {
                Vector2 touchPad = Controller.GetAxis();

                if (touchPad.x > 0.75f)
                {
                    if (ChoiceIndex + 1 <= 4)
                    {
                        ChoiceIndex += 1;
                    }
                }
                else if (touchPad.x < -0.75f)
                {
                    if (ChoiceIndex - 1 >= 0)
                    {
                        ChoiceIndex -= 1;
                    }
                }
                Choice.transform.SetPositionAndRotation(Targets[ChoiceIndex].transform.position, Choice.transform.rotation);
            }

            // Confirm the selection
            if (Controller.GetHairTriggerDown())
            {
                RecordData();
                QuestionnaireObject.SetActive(false);
                Ending.SetActive(true);
            }
        }
    }
}
