using UnityEngine;
using UnityEngine.SceneManagement;

//manage the events that occurs in the application
public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject cameraRig;
    
    string userID;

    private void Awake()
    {
        Instance = this;
        //find the steamvr eye and assign it to data collector
        if (DataCollector.Instance != null)
        {
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
            userID = DataCollector.Instance.dataID;
        }
    }

    // Use this for initialization
    void Start ()
 {
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
