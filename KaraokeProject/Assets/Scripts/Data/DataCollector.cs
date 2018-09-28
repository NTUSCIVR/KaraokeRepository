using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataCollector : MonoBehaviour
{
    public static DataCollector Instance;

    [Tooltip("Time interval to collect Headset Position & Rotation. Default: 1.0f")]
    public float dataRecordInterval = 1f;
    
    [Header("ID Input Field. Under Canvas")]
    public InputField UserIdInputField;

    public GameObject PositionUI;
    [Tooltip("PositionID Input Field. Under PositionUI")]
    public InputField PositionIdInputField;

    [HideInInspector]
    public string dataID = "";
    [HideInInspector]
    public string dataPositionID = "";
    [HideInInspector]
    public bool startRecording = false;
    [HideInInspector]
    public GameObject user;
    [HideInInspector]
    public string videoUrl;
    private float time = 0f;
    [HideInInspector]
    public string currentFolderPath;

    public enum DataToRecord
    {
        HeadMovement,
        RatingVideo
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    private void Start ()
    {
        Instance = this;
		UserIdInputField.Select();
    }

    // Update is called once per frame
    private void Update ()
    {
        if(startRecording)
        {
            time += Time.deltaTime;
            if (time > dataRecordInterval)
            {
                time = 0;
                StreamWriter sw = File.AppendText(GetCSVPath(DataToRecord.HeadMovement, currentFolderPath));
                sw.WriteLine(GenerateData());
                sw.Close();
            }
        }
	}

    // Finished input userId, proceed to input positionId
    public void ProceedToChoosePosition()
    {
        dataID = UserIdInputField.text;
        CreateFolder();
        PositionUI.SetActive(true);
        PositionIdInputField.Select();
    }

    // Finished input positionId, change scene to mainScene
    public void EnterRoom()
    {
        dataPositionID = PositionIdInputField.text;
        startRecording = true;
        SceneManager.LoadScene("MainScene");
    }

    // Generate Data for Head Movement
    private string GenerateData()
    {
        string data = "";
        data += System.DateTime.Now.ToString("HH");
        data += ":";
        data += System.DateTime.Now.ToString("mm");
        data += ":";
        data += System.DateTime.Now.ToString("ss");
        data += ":";
        data += System.DateTime.Now.ToString("FFF");
        data += ",";
        string posstr = user.GetComponent<SteamVR_Camera>().head.transform.position.ToString("F3");
        data += ChangeLetters(posstr, ',', '.');
        data += ",";
        string rotstr = user.GetComponent<SteamVR_Camera>().head.transform.rotation.ToString("F3");
        data += ChangeLetters(rotstr, ',', '.');
        return data;
    }

    // Duplicates Folder with a (Duplicate Count) at the back of ID
    private string GetFolderPath()
    {
        string Folder = "/Data/";

#if UNITY_EDITOR
        string filePath = Application.dataPath + Folder + dataID;
        int duplicateCounts = 0;
        while (true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + Folder + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
#elif UNITY_STANDALONE_WIN
        string filePath = Application.dataPath + Folder + dataID;
        int duplicateCounts = 0;
        while (true)
        {
            if (Directory.Exists(filePath))
            {
                ++duplicateCounts;
                filePath = Application.dataPath + Folder + dataID + "(" + duplicateCounts.ToString() + ")";
            }
            else
                break;
        }
        return filePath;
#endif
    }

    // Duplicate or Create new folder
    private void CreateFolder()
    {
        currentFolderPath = GetFolderPath();
        // Create new folder
        Directory.CreateDirectory(currentFolderPath);
        CreateCSV(DataToRecord.HeadMovement, currentFolderPath);
        CreateCSV(DataToRecord.RatingVideo, currentFolderPath);
    }

    public string GetCSVPath(DataToRecord dataToRecord, string folderPath)
    {
        string File = "";

        switch (dataToRecord)
        {
            case DataToRecord.HeadMovement:
                File += "/HeadMovement";
                break;
            case DataToRecord.RatingVideo:
                File += "/RatingVideo";
                break;
        }
#if UNITY_EDITOR
        return folderPath + File + ".csv";
#elif UNITY_STANDALONE_WIN
        return folderPath + File + ".csv";
#endif
    }

    // Create new CSV
    private void CreateCSV(DataToRecord dataToRecord, string folderPath)
    {
        // Create new CSV and Write in Data Headers on first line
        StreamWriter output = File.CreateText(GetCSVPath(dataToRecord, folderPath));
        switch (dataToRecord)
        {
            case DataToRecord.HeadMovement:
                output.WriteLine("Time, Position, Rotation");
                break;
            case DataToRecord.RatingVideo:
                output.WriteLine("Video Url, Rating(-2 to 2)");
                break;
        }
        output.Close();
    }

    // Change "letter" in "str" to "toBeLetter"
    private string ChangeLetters(string str, char letter, char toBeLetter)
    {
        char[] ret = str.ToCharArray();
        for(int i = 0; i < ret.Length; ++i)
        {
            if(ret[i] == letter)
            {
                ret[i] = toBeLetter;
            }
        }
        return new string(ret);
    }
}
