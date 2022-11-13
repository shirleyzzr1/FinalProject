// Filename:    ArucoDetector.cs 
// Summary:     Detect Aruco Code attached to the shoe, and use light white cube to 
//              indicate the detected shoes
// Author:      Zhuoru Zhang
// Date:        2022/10/04 

using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ArucoModule;
using UnityEngine.UI;


public class ArucoDetector : MonoBehaviour
{
    /// <summary> Camera Texture of RGB Camera </summary>
    public NRRGBCamTexture RGBCamTexture { get; set; }

    /// <summary> GameObject of Cube representing left foot </summary>
    GameObject leftCube;
    /// <summary> GameObject of Cube representing left foot </summary>
    GameObject rightCube;
    /// <summary>Prefab of leftCube </summary>
    public GameObject leftPrefab;
    /// <summary>Prefab of rightCube </summary>
    public GameObject rightPrefab;

    /// <summary>RGB Camera Object </summary>
    GameObject RGBcamObject;
    Camera RGBCamera;

    Mat rgbMat;

    /// <summary> The hight and width of the real RGB Camera texture </summary>

    int height;
    int width;

    /// <summary> The identifiers </summary>
    Mat ids;
    /// <summary> The cameraparam matrix </summary>
    Mat camMatrix;
    
    /// <summary> The distortion coeffs.</summary>
    MatOfDouble distCoeffs;
    DetectorParameters detectorParams;
    /// <summary> The dictionary </summary>
    Dictionary dictionary;

    /// <summary> The corners </summary>
    List<Mat> corners;

    /// <summary> The rejected corners </summary>
    List<Mat> rejectedCorners;

    /// <summary> The default lenth of the marker </summary>
    public float markerLength = 0.1f;

    /// <summary> The rotation vectors and translation vector </summary>
    Mat rvecs;
    Mat tvecs;

    /// <summary> The transformation matrix for AR </summary>
    Matrix4x4 ARM;

    /// <summary> The canvas for score display </summary>
    GameObject m_Canvas;
    SimpleCollectibleScript m_SimpleCollectibleScript;

    /// <summary> The flag for resetting current score </summary>
    public static bool reset_score;

    /// <summary> Get the center of the current camera </summary>
    private Transform CameraCenter { get { return NRInput.CameraCenter; } }

    /// <summary> Start is called before the first frame update. </summary>
    void Start()
    {
        RGBcamObject = GameObject.Find("RGBCamera");
        RGBCamera = RGBcamObject.GetComponent<Camera>();

        RGBCamTexture = new NRRGBCamTexture();
        RGBCamTexture.Play();

        Texture2D m_texture = RGBCamTexture.GetTexture();
        height = m_texture.height;
        width = m_texture.width;
        rgbMat = new Mat(m_texture.height, m_texture.width, CvType.CV_8UC3);

        // Match the current marker with dictionary
        dictionary = Aruco.getPredefinedDictionary(Aruco.DICT_6X6_50);
        ids = new Mat ();
        corners = new List<Mat> ();
        rejectedCorners = new List<Mat> ();
        rvecs = new Mat ();
        tvecs = new Mat ();
        detectorParams = DetectorParameters.create ();

        m_Canvas = GameObject.Find("Score");
        m_SimpleCollectibleScript = new SimpleCollectibleScript();
    }

    /// <summary> Update is called once per frame. </summary>
    void Update()
    {
        if(leftCube ==null){
            leftCube = Instantiate(leftPrefab);
        }
        if(rightCube==null){
            rightCube = Instantiate(rightPrefab);
        }
        detect();
        var score = 0.0f;
        if(reset_score){
            score = 0.0f;
            leftCube.GetComponent<scoreCounting>().scores = 0;
            rightCube.GetComponent<scoreCounting>().scores = 0;
            reset_score = false;

        }
        else{
            score = leftCube.GetComponent<scoreCounting>().scores+ rightCube.GetComponent<scoreCounting>().scores;
            }

        // Move the score together with the movement of the camera
        m_Canvas.transform.position = CameraCenter.position;
        m_Canvas.transform.rotation = CameraCenter.rotation;
        // Update the text of score
        m_Canvas.GetComponentInChildren<Text>().text = "SCORE: "+ score.ToString();
        
    }

    /// <summary> Detect the aruco tag in the current frame </summary>
    void detect(){
        Utils.texture2DToMat(RGBCamTexture.GetTexture(),rgbMat,false);
        Aruco.detectMarkers(rgbMat, dictionary, corners, ids, detectorParams, rejectedCorners, camMatrix, distCoeffs);
        if(ids.total()>0){
            Aruco.estimatePoseSingleMarkers (corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);
            for(int i=0; i<rvecs.total();i++){
                //Make sure only two marker is detected in the current texture
                if(i==2){
                    Debug.Log("More than 2 tags are detected!");
                    break;
                }
                using (Mat rvec = new Mat (rvecs, new OpenCVForUnity.CoreModule.Rect (0, i, 1, 1)))
                using (Mat tvec = new Mat (tvecs, new OpenCVForUnity.CoreModule.Rect (0, i, 1, 1))){
                    // Convert to unity pose data.
                    double[] rvecArr = new double[3];
                    rvec.get (0, 0, rvecArr);
                    double[] tvecArr = new double[3];
                    tvec.get (0, 0, tvecArr);
                    PoseData poseData = ARUtils.ConvertRvecTvecToPoseData (rvecArr, tvecArr);
                    // Convert to transform matrix.
                    ARM = ARUtils.ConvertPoseDataToMatrix (ref poseData, true);
                    ARM = RGBCamera.transform.localToWorldMatrix*ARM;
                    var position = new Vector3(ARM[0,3], ARM[1,3], ARM[2,3]);
                    if(i==0){
                        leftCube.transform.position = position;
                        leftCube.transform.rotation = ARM.rotation;
                    }
                    else{
                        rightCube.transform.position = position;
                        rightCube.transform.rotation = ARM.rotation;
                    }

                }
            }
        }
    }

}
