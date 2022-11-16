// Filename:    ArucoDetector.cs 
// Summary:     Detect Aruco Code attached to the shoe, and use light white cube to 
//              indicate the detected shoes
// Author:      Zhuoru Zhang
// Date:        2022/10/04 

using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using NRKernal.NRExamples;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ArucoModule;
using UnityEngine.UI;
using System.IO;
using NRKernal.Record;


#if UNITY_ANDROID && !UNITY_EDITOR
    using GalleryDataProvider = NRKernal.NRExamples.NativeGalleryDataProvider;
#else
    using GalleryDataProvider = NRKernal.NRExamples.MockGalleryDataProvider;
#endif
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

    Vector3 last_left;
    Vector3 last_right;


    private List<string> timestamp = new List<string>();
    private List<Vector3> leftPos = new List<Vector3>();

    GalleryDataProvider galleryDataTool;


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

        int max_d = (int)Mathf.Max (width, height);
        double fx = max_d;
        double fy = max_d;
        double cx = width / 2.0f;
        double cy = height / 2.0f;
        camMatrix = new Mat (3, 3, CvType.CV_64FC1);
        camMatrix.put (0, 0, fx);
        camMatrix.put (0, 1, 0);
        camMatrix.put (0, 2, cx);
        camMatrix.put (1, 0, 0);
        camMatrix.put (1, 1, fy);
        camMatrix.put (1, 2, cy);
        camMatrix.put (2, 0, 0);
        camMatrix.put (2, 1, 0);
        camMatrix.put (2, 2, 1.0f);

        distCoeffs = new MatOfDouble (0, 0, 0, 0);


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

            string filename = string.Format("pace_result_{0}.txt", NRTools.GetTimeStamp().ToString());
            string folder = string.Format("{0}/PaceData", Application.persistentDataPath);
            if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            string[] result = new string[leftPos.Count];
            for(int i=0;i<leftPos.Count;i++){
                result[i] = timestamp[i]+" "+ leftPos[i].x.ToString() + " "+leftPos[i].y.ToString()+ " "+ leftPos[i].z.ToString();
            }
            File.WriteAllLines(string.Format("{0}/{1}", folder, filename), result);
            if (galleryDataTool == null)
            {
                galleryDataTool = new GalleryDataProvider();
            }

            galleryDataTool.InserttxtFile(folder, filename, "Documents");
            timestamp.Clear();
            leftPos.Clear();
            LineDrawer.Instance.ClearLinePoint();


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
        Aruco.detectMarkers(rgbMat, dictionary, corners, ids, detectorParams, rejectedCorners);
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
                        leftPos.Add(position);
                        timestamp.Add(NRTools.GetTimeStamp().ToString());
                        LineDrawer.Instance.AddLinePoint(position);
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
