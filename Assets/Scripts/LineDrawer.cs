using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
public class LineDrawer : MonoBehaviour
{   
    public static LineDrawer Instance;

    List<Vector3> linepointsLeft;
    List<Vector3> linepointsRight;



    GameObject newlineLeft;
    LineRenderer drawLineLeft;

    GameObject newlineRight;
    LineRenderer drawLineRight;

    float linewidth = 0.02f;

    void Awake(){
         Instance = this;
     }
    // Start is called before the first frame update
    void Start()
    {
        linepointsLeft = new List<Vector3>();
        newlineLeft = new GameObject("LineDrawerLeft");
        drawLineLeft = newlineLeft.AddComponent<LineRenderer>();
        drawLineLeft.material = new Material(Shader.Find("Sprites/Default"));
        drawLineLeft.startWidth = linewidth;
        drawLineLeft.endWidth  = linewidth;
        drawLineLeft.startColor = Color.red;
        drawLineLeft.endColor = Color.red;

        linepointsRight = new List<Vector3>();
        newlineRight = new GameObject("LineDrawerRight");
        drawLineRight = newlineRight.AddComponent<LineRenderer>();
        drawLineRight.material = new Material(Shader.Find("Sprites/Default"));
        drawLineRight.startWidth = linewidth;
        drawLineRight.endWidth  = linewidth;
        drawLineRight.startColor = Color.red;
        drawLineRight.endColor = Color.red;

    }

    // Update is called once per frame
    void Update()
    {
        // var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        // Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);
        // Vector3 endpoint = laserAnchor.transform.position+laserAnchor.transform.forward*10;
        // AddLinePoint(endpoint);
        drawLineLeft.positionCount = linepointsLeft.Count;
        drawLineLeft.SetPositions(linepointsLeft.ToArray());

        drawLineRight.positionCount = linepointsRight.Count;
        drawLineRight.SetPositions(linepointsRight.ToArray());
    }

    public void AddLinePoint(Vector3 point,string name){
        if (name=="left")linepointsLeft.Add(point);
        else if(name=="right")linepointsRight.Add(point);
    }

    public void ClearLinePoint(){
        linepointsLeft.Clear();
        linepointsRight.Clear();
    }
}
