// Filename:    ObjectSpawner.cs 
// Summary:     Generate object on the ground based on user selection
// Author:      Zhuoru Zhang
// Date:        2022/11/4

using UnityEngine;
using NRKernal;
using NRKernal.NRExamples;
public class ObjectSpawner : MonoBehaviour
{
    GameObject Object;

    /// <summary> Default prefab of the object </summary>
    public GameObject objectPrefab;

    /// <summary> Prefab of the Coin </summary>
    public GameObject CoinPrefab;

    /// <summary> Prefab of the Cube </summary>
    public GameObject CubePrefab;

    /// <summary> Prefab of the Gem </summary>
    public GameObject GemPrefab;
    /// <summary> select menu canvas </summary>
    public GameObject selectMenu;
    /// <summary> select menu canvas </summary>
    static public string prefabName;
    /// <summary> Flag to restart the whole game </summary>
    static public bool restart = false;

    /// <summary> Manager of the trackable object </summary>
    NRTrackableManager m_NRTrackableManager;


    /// <summary> Get the center of the current camera </summary>
    private Transform CameraCenter { get { return NRInput.CameraCenter; } }



    void Start()
    {
        m_NRTrackableManager = new NRTrackableManager();
    }

    /// <summary> Update is called every frame </summary>
    void Update()
    {
        var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);
        
        RaycastHit hitResult;
        if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hitResult, 10))
        {   
            // Check if laser collides with the ground object
            var hit = hitResult.collider.gameObject; 
            if ( hit != null &&
                hit.GetComponent<NRTrackableBehaviour>()?.Trackable.GetTrackableType() == TrackableType.TRACKABLE_PLANE)
            {
                // Check if user hit the button, instantiate the game object at the instersect point 
                if(WasTapped()){
                    
                    generatePrefab(prefabName);
                    if(objectPrefab!=null){
                        Object = Instantiate(objectPrefab);
                        Debug.Log("Instantiate the new object");
                        Object.transform.position = hitResult.point;
                    }
                }

            }
        }
        if(NRHomeMenu.restart){
            onRestartClick();
            restart = true;
            NRHomeMenu.restart = false;
        }

    }
    /// <summary> Check if APP button is clicked </summary>
    private bool WasTapped(){
        return NRInput.GetButtonDown(ControllerButton.APP);
    }

    /// <summary> Pause GroundDetection and Destroy the existing detect plane </summary>
    void pauseGroundDetection(){
        m_NRTrackableManager.Pause();
        GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");
        foreach(GameObject plane in planes)
        GameObject.Destroy(plane);
    }

    /// <summary> Resume GroundDetection </summary>
    void resumeGroundDetection(){
        // NRSessionManager.Instance.TrackableFactory.Start();
        // GameObject[] planes = GameObject.FindGameObjectsWithTag("Plane");
        // foreach(GameObject plane in planes)
        // plane.SetActive(true);
        m_NRTrackableManager.Resume();
    }

    /// <summary> Corresponding prefab to generate </summary>
    /// <param name = "name"> Generate corresponding Gameobject based on name </param>
    void generatePrefab(string name){
        Debug.Log("name: "+name);
        if (name=="SellectCoin"){
            Debug.Log("get a coin");
            objectPrefab = CoinPrefab;
        }
        else if(name=="SellectGem"){
            Debug.Log("get a gem");
            objectPrefab = GemPrefab;
        }
        else if(name=="SellectCube"){
            Debug.Log("get a cube");
            objectPrefab = CubePrefab;
        }
    }
    /// <summary> 
    /// After start button is clicked, pause ground detection, hide select menu and disable the moveobject 
    /// Component to avoid accidently changing the position of the object
    /// </summary>
    public void onStartClick(){
        Debug.Log("start the game!!");
        pauseGroundDetection();
        selectMenu.SetActive(false);
        GameObject[] rewardObjects = GameObject.FindGameObjectsWithTag("Reward");
        foreach(GameObject reward in rewardObjects)
        reward.GetComponent<MoveObject>().enabled = false;
    }

    /// <summary> 
    /// After restart button is clicked, resume ground detection, show select menu in front of the game center
    /// Reset score of the game
    /// </summary>
    void onRestartClick(){
        GameObject[] rewardObjects = GameObject.FindGameObjectsWithTag("Reward");
        foreach(GameObject reward in rewardObjects)
        GameObject.Destroy(reward);
        resumeGroundDetection();
        selectMenu.transform.position = CameraCenter.position;
        selectMenu.transform.rotation = CameraCenter.rotation;
        selectMenu.SetActive(true);
        ArucoDetector.reset_score = true;
    }
}
