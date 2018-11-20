//--------------------------------------------------------------------------------
/*
 * This Script is used for playing song in Television Screen in Main Scene with User Input.
 * It also Change Scene to End Scene when song has finished playing.
 * Allow Inputs to : Skip song,
 *                   Restart(Load StartScene and Input User ID and position choice again)
 * 
 * Used in Main Scene, attached to Empty GameObject "GameController"
 * Require 1 Image variable, 5 GameObject variable : [CameraRig], Controls, TV, Vertical/Horizontal Tracks, and 3 Transform variables : Top, Center, Bottom.
 * Where Image can be found as "BackgroundImage" under Canvas in Camera(eye) of [CameraRig].
 * TV can be found as "TV set N010614" under Environment.
 * Tracks can be found as "Track_X", where "X" is "Vertical/Horizontal" under Environment.
 * Transforms can be found under TV Transforms.
 * Controls can be found as "TVCanvas".
 * [CameraRig], Environment, TV Transforms, TVCanvas can be found in Hierarchy.
 * Among the variables, only the Tracks and Controls's GameObject no need to be active.
 */
//--------------------------------------------------------------------------------

using System.IO; // For DirectoryInfo, SearchOption
using UnityEngine;  // Default Unity Script (MonoBehaviour, GameObject, Transform, Animator, Header, Tooltip, HideInInspector, Time, FindObjectOfType, Color, Input, KeyCode, GetComponent)
using UnityEngine.UI; // For Image, Text
using UnityEngine.Video; // For VideoPlayer
using System.Collections.Generic; // For List<>
using UnityEngine.SceneManagement; // For SceneManager

// Manage the events that occurs in the application
public class GameController : MonoBehaviour
{
    // For other scripts to access GameController
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

    // For Choosing/Playing song
    [HideInInspector]
    public VideoPlayer videoPlayer;
    private List<string> videoUrls;
    private List<string> videoNames;
    private Text songTitle;
	private GameObject songPanel;
    private int songIndex = 0;

    // For Positioning/Moving Television Screen
    private string PositionID;
	private Animator TV_anim;
	private string Anim_Clip = "";

    // Runs before Start()
    private void Awake()
    {
        // Set this instance of GameController to allow other scripts to access its variables and data
        Instance = this;

        // If DataCollector is alive
        if (DataCollector.Instance != null)
        {
            // Find the steamvr eye and assign it to data collector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;

            // Applies Position ID - For Positioning/Moving TV Screen
            PositionID = DataCollector.Instance.dataPositionID;
        }
		
		// Initialise variables
        videoPlayer = TV.transform.Find("Screen").GetComponent<VideoPlayer>();
		songPanel = videoPlayer.transform.Find("SongCanvas").Find("Panel").gameObject;
        songTitle = videoPlayer.transform.Find("SongCanvas").Find("Text").GetComponent<Text>();
		TV_anim = TV.GetComponent<Animator>();
        videoUrls = new List<string>();
        videoNames = new List<string>();
		
		// Initialise TV - Load Songs and Positioning/Moving TV Screen
		LoadSongs();
        LoadTVPosition();

        // Reset to invisible first - Prepare for Fading to End Scene
        Color temp = BackgroundImage.GetComponent<Image>().color;
        temp.a = 0.0f;
        BackgroundImage.GetComponent<Image>().color = temp;

        // Listen for finish playing
        videoPlayer.loopPointReached += FinishedPlayingMV;
    }

    // Load videos from C:/KaraokeVideos/
    private void LoadSongs()
    {
        DirectoryInfo directory = new DirectoryInfo(@"C:\KaraokeVideos\");

        // Loads every mp4 file path into List of string, videoUrls
        foreach (var file in directory.GetFiles("*.mp4", SearchOption.AllDirectories))
        {
            videoUrls.Add(directory.FullName + file.Name);

            // Get the Name of videos - Prepare for Song Selection Screen
            videoNames.Add(file.Name.Remove(file.Name.Length - "Official MV.mp4".Length));
        }

        // Trim Excess to prevent extra space in List
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

        // Update Text Shown on TV Screen based on Index in VideoNames list
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

    // Applies Setting for TV Screen Position/Movement based on User Input in Start Scene
    // 1 / 2 / 3 Loads TV Fixed Position & Rotation
    // 4 / 5 / 6 / 7 Allows TV Movements(Animation), also shows respective Tracks
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

    // Returns increased Alpha Component of Color in Background Image
    // Once Alpha reach 1f, Change Scene to End Scene
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

    // Stops Playing song, TV Movement, Recording Head Movement
    // Reset TV Position & Rotation to Center
    // Show TV Background
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

    // Loads StartScene
    private void Restart()
    {
        SceneManager.LoadScene("StartScene");

        // Destroy current DataCollector Instance, as StartScene will have its new instance of DataCollector
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

        // Proceed to Skip video if 'Spacebar' is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            FinishedPlayingMV(videoPlayer);
        }

        // Proceed to Restart if 'R' is pressed
        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }
    }
}
