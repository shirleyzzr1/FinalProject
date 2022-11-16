using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
public class LineDrawer : MonoBehaviour
{   
    public static LineDrawer Instance;

    List<Vector3> linepoints;

    GameObject newline;
    LineRenderer drawLine;

    float linewidth = 0.02f;

    void Awake(){
         Instance = this;
     }
    // Start is called before the first frame update
    void Start()
    {
        linepoints = new List<Vector3>();
        newline = new GameObject("LineDrawer");
        drawLine = newline.AddComponent<LineRenderer>();
        drawLine.material = new Material(Shader.Find("Sprites/Default"));
        drawLine.startWidth = linewidth;
        drawLine.endWidth  = linewidth;
        drawLine.startColor = Color.red;
        drawLine.endColor = Color.red;
        
    }

    // Update is called once per frame
    void Update()
    {
        // var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        // Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);
        // Vector3 endpoint = laserAnchor.transform.position+laserAnchor.transform.forward*10;
        // AddLinePoint(endpoint);
        drawLine.positionCount = linepoints.Count;
        drawLine.SetPositions(linepoints.ToArray());
    }

    public void AddLinePoint(Vector3 point){
        linepoints.Add(point);
    }

    public void ClearLinePoint(){
        linepoints.Clear();
    }
}
