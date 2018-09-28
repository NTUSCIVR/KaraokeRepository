using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    [Tooltip("[CameraRig]")]
    public GameObject cameraRig;
    [Tooltip("Under [CameraRig] -> Camera(head) -> Camera(eye) -> Canvas")]
    public Image BackgroundImage;

    [Tooltip("Rate of Fading. Higher means fade out faster;Lower means fade out slower. Default: 0.3f")]
    public float FadingRate = 0.3f;
    [Tooltip("TVCanvas")]
    public GameObject Controls;

    [Header("TV Transforms")]
    public Transform TopTvTransform;
    public Transform CenterTvTransform;
    public Transform BottomTvTransform;

    [Header("Under Environment")]
    public GameObject TV;
    public GameObject Track_Vertical;
    public GameObject Track_Horizontal;

    [HideInInspector]
    public VideoPlayer videoPlayer;
    private string PositionID;
    private List<string> videoUrls;
    private List<string> videoNames;
    private Text songTitle;
	private GameObject songPanel;
	private Animator TV_anim;
	private string Anim_Clip = "";
    private int songIndex = 0;

    private void Awake()
    {
        Instance = this;
        if (DataCollector.Instance != null)
        {
            //find the steamvr eye and assign it to data collector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            PositionID = DataCollector.Instance.dataPositionID;
        }
		
		// Initialise variables
        videoPlayer = TV.transform.Find("Screen").GetComponent<VideoPlayer>();
		songPanel = videoPlayer.transform.Find("SongCanvas").Find("Panel").gameObject;
        songTitle = videoPlayer.transform.Find("SongCanvas").Find("Text").GetComponent<Text>();
		TV_anim = TV.GetComponent<Animator>();
        videoUrls = new List<string>();
        videoNames = new List<string>();
		
		// Initialise TV
		LoadSongs();
        LoadTVPosition();

        // Reset to invisible first
        Color temp = BackgroundImage.GetComponent<Image>().color;
        temp.a = 0.0f;
        BackgroundImage.GetComponent<Image>().color = temp;

        // Listen for finish playing
        videoPlayer.loopPointReached += FinishedPlayingMV;
    }

    private void LoadSongs()
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\KaraokeVideos\");

        // Load .mp4 videos from C:\KaraokeVideos\ and add into Url list
        foreach (var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);
            // Get the Name of videos
            videoNames.Add(file.Name.Remove(file.Name.Length - "Official MV.mp4".Length));
        }
        videoUrls.TrimExcess();
        videoNames.TrimExcess();
        
        // Sets default songTitle
		SelectSong("");
        // Allow Controls
        Controls.SetActive(true);
    }

	// Scroll through videoNames list while showing changes in the text on TV
    public void SelectSong(string Direction)
    {
        switch(Direction)
        {
            case "Up":
				if(songIndex > 0)
				{
					songIndex -= 1;
				}
                break;
            case "Down":
				if(songIndex < videoNames.Count)
				{
					songIndex += 1;
				}
                break;
			default:
				break;
        }
		songTitle.text = videoNames[songIndex];
    }

	// Play the selected song
    public void ConfirmSong()
    {
        // Apply url to what we found in videoUrls based on songTitle.text
		videoPlayer.url = videoUrls.Find(x => x.Contains(songTitle.text));
        
		// Save the url down for Questionnaire to Record
        DataCollector.Instance.videoUrl = videoPlayer.url;
		
		// Hide songTitle, TV Background and Controls
		songTitle.gameObject.SetActive(false);
		songPanel.SetActive(false);
		Controls.SetActive(false);
		
		// Play the song
        videoPlayer.Play();

        // Start Animation(TV Movements) if Anim_Clip is set
		if(!Anim_Clip.Equals(""))
		{
			TV_anim.Play(Anim_Clip);
		}
    }

    // 1,2,3 Loads TV Fixed Position & Rotation
    // 4,5,6,7 Allows TV Movements(Animation)
    private void LoadTVPosition()
    {
        switch (PositionID)
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
			case "4":
				Anim_Clip = "Plane_UpDown";
                Track_Vertical.SetActive(true);
                break;
			case "5":
				Anim_Clip = "Plane_LeftRight";
                Track_Horizontal.SetActive(true);
                break;
			case "6":
				Anim_Clip = "Curved_UpDown";
                Track_Vertical.SetActive(true);
                break;
			case "7":
				Anim_Clip = "Curved_LeftRight";
                Track_Horizontal.SetActive(true);
                break;
        }
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
        // Stop Video player
        _videoPlayer.Stop();

        // Stop TV Movement
        TV_anim.StopPlayback();
        
        // Reset TV Position & Rotation to Center
        TV.transform.position = CenterTvTransform.position;
        TV.transform.rotation = CenterTvTransform.rotation;
        
        // Set TV Background visible
        songPanel.SetActive(true);
        
        // Stop recording Head Movement
        DataCollector.Instance.startRecording = false;
    }

    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        Destroy(DataCollector.Instance.gameObject);
    }

	// Update is called once per frame
	private void Update ()
    {
        // Finished playing, fade background sphere in
        if(!songTitle.gameObject.activeSelf && !videoPlayer.isPlaying)
        {
            BackgroundImage.GetComponent<Image>().color =
                Fade(BackgroundImage.GetComponent<Image>().color);
        }

        // Skip video
        if(Input.GetKey(KeyCode.Space))
        {
            FinishedPlayingMV(videoPlayer);
        }

        // Restart from start scene
        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }
    }
}
