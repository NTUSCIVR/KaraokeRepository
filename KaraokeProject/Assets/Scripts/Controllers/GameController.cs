using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

//manage the events that occurs in the application
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject cameraRig;
    public GameObject TV;

    [Header("Video Name")]
    public string firstName;
    public string secondName;

    [Header("TV Position")]
    public Transform TopTvTransform;
    public Transform CenterTvTransform;
    public Transform BottomTvTransform;

    private VideoPlayer videoPlayer;
    private string SongID;
    private string PositionID;

    private void Awake()
    {
        Instance = this;
        //find the steamvr eye and assign it to data collector
        if (DataCollector.Instance != null)
        {
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            SongID = DataCollector.Instance.dataSongID;
            PositionID = DataCollector.Instance.dataPositionID;
        }
        videoPlayer = TV.transform.Find("Screen").GetComponent<VideoPlayer>();

        switch(SongID)
        {
            case "1":
                videoPlayer.url = "C:/KaraokeVideos/" + firstName;
                break;
            case "2":
                videoPlayer.url = "C:/KaraokeVideos/" + secondName;
                break;
        }
        videoPlayer.Play();

        switch(PositionID)
        {
            case "1":
                TV.transform.position = TopTvTransform.position;
                TV.transform.rotation = TopTvTransform.rotation;
                break;
            case "2":
                TV.transform.position = CenterTvTransform.position;
                TV.transform.rotation = CenterTvTransform.rotation;
                break;
            case "3":
                TV.transform.position = BottomTvTransform.position;
                TV.transform.rotation = BottomTvTransform.rotation;
                break;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("StartScene");
            Destroy(DataCollector.Instance.gameObject);
        }
    }
}
