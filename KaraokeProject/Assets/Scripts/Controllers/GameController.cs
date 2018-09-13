using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject cameraRig;
    public GameObject TV;
    public Image BackgroundImage;
    public float FadingRate = 0.15f;

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
        DataCollector.Instance.videoUrl = videoPlayer.url;
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
        videoPlayer.loopPointReached += FinishedPlayingMV;

        // Reset to invisible first
        Color temp = BackgroundImage.GetComponent<Image>().color;
        temp.a = 0.0f;
        BackgroundImage.GetComponent<Image>().color = temp;
    }

    private Color Fade(Color color)
    {
        Color Temp = color;
        if (Temp.a < 1.0f)
        {
            // Fade accoding to Fading Rate
            Temp.a += FadingRate * Time.deltaTime;
        }
        else
        {
            // Once fully visible, change scene to endScene
            Temp.a = 1.0f;
            SceneManager.LoadScene("EndScene");
        }
        return Temp;
    }

    private void FinishedPlayingMV(VideoPlayer _videoPlayer)
    {
        _videoPlayer.Stop();
        DataCollector.Instance.startRecording = false;
    }

    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        Destroy(DataCollector.Instance.gameObject);
    }

	// Update is called once per frame
	void Update ()
    {
        // Finished playing, fade background sphere in
        if(!videoPlayer.isPlaying)
        {
            BackgroundImage.GetComponent<Image>().color =
                Fade(BackgroundImage.GetComponent<Image>().color);
        }

        if(Input.GetKey(KeyCode.Space))
        {
            FinishedPlayingMV(videoPlayer);
        }

        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }
    }
}
