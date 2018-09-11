using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataCollector : MonoBehaviour
{
    public InputField UserIdInputField;
    public string dataID = "";

    public GameObject SongUI;
    public InputField SongIdInputField;
    public string dataSongID = "";

    public GameObject PositionUI;
    public InputField PositionIdInputField;
    public string dataPositionID = "";

    public static DataCollector Instance;

    public bool startRecording = false;
    public float dataRecordInterval = 1f;
    float time = 0f;
    
    public GameObject user;

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
                StreamWriter sw = File.AppendText(GetCSVPath(DataToRecord.HeadMovement));
                sw.WriteLine(GenerateData());
                sw.Close();
            }
        }
	}

    public void ProceedToChoosePosition()
    {
        dataID = UserIdInputField.text;
        CreateFolder();
        PositionUI.SetActive(true);
    }

    public void ProceedToChooseSong()
    {
        dataPositionID = PositionIdInputField.text;
        SongUI.SetActive(true);
        PositionUI.SetActive(false);
    }

    public void EnterRoom()
    {
        dataSongID = SongIdInputField.text;
        startRecording = true;
        SceneManager.LoadScene("MainScene");
    }

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

    private string GetFolderPath()
    {
        string Folder = "/Data/";
#if UNITY_EDITOR
        return Application.dataPath + Folder + dataID + "/";
#elif UNITY_STANDALONE_WIN
        return Application.dataPath + Folder + dataID + "/";
#endif
    }

    // Replace existing folder or Create new folder
    private void CreateFolder()
    {
        // Found same folder
        if(Directory.Exists(GetFolderPath()))
        {
            // Delete files inside the folder first
            foreach(FileInfo file in new DirectoryInfo(GetFolderPath()).GetFiles())
            {
                file.Delete();
            }
            // Delete the folder
            Directory.Delete(GetFolderPath());
        }

        // Create new folder
        Directory.CreateDirectory(GetFolderPath());
        CreateCSV(DataToRecord.HeadMovement);
        CreateCSV(DataToRecord.RatingVideo);
    }

    public string GetCSVPath(DataToRecord dataToRecord)
    {
        string Folder = "/Data/";
        string File = "";

        switch(dataToRecord)
        {
            case DataToRecord.HeadMovement:
                File += "/HeadMovement";
                break;
            case DataToRecord.RatingVideo:
                File += "/RatingVideo";
                break;
        }
#if UNITY_EDITOR
        return Application.dataPath + Folder + dataID + File + ".csv";
#elif UNITY_STANDALONE_WIN
        return Application.dataPath + Folder + dataID + File + ".csv";
#endif
    }

    // Replace existing csv or Create new csv
    private void CreateCSV(DataToRecord dataToRecord)
    {
        // Found same csv
        if (File.Exists(GetCSVPath(dataToRecord)))
        {
            // Delete the csv
            File.Delete(GetCSVPath(dataToRecord));
        }

        // Create new csv
        StreamWriter output = File.CreateText(GetCSVPath(dataToRecord));
        switch(dataToRecord)
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
