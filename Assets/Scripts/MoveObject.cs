// Filename:    ObjectSpawner.cs 
// Summary:     Move object with the laser of the controller
// Author:      Zhuoru Zhang
// Date:        2022/10/29
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using NRKernal.NRExamples;
public class MoveObject : MonoBehaviour
{

    /// <summary> Detected plane prefab. </summary>
    private Vector3 lastPos;

    /// <summary> Detected plane prefab. </summary>
    private Transform tmp;

    /// <summary> Detected plane prefab. </summary>
    public bool isDragging=false;

    /// <summary> the lenth of laser. </summary>
    public float laserLen = 10.0f;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);
        
        RaycastHit hit;

        //Once user release the trigget panel, stop moving this object
        if(WasTappedUp()){
            isDragging = false;
            Debug.Log("up");
        }
        if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hit, 10))
            {   var hitResult = hit.collider.gameObject; 
                if(hitResult == this.gameObject){
                    if(isDragging){
                        // calculate the position of the object after dragging by laser
                        Vector3 endPos = laserAnchor.transform.position+(laserAnchor.transform.forward*(laserLen+0.1f));
                        this.transform.position = endPos;
                    }
                    else{
                        if(WasTappedDown()){
                            isDragging = true;Debug.Log("down;");
                            // update the laserlenth based on the position of the object
                            laserLen = Vector3.Distance(laserAnchor.transform.position,hit.point);
                        }
                    }
                }
                
            }
    }

    /// <summary> Detect whether user press the trigger button </summary>
    /// <returns> true if trigger is pressed </returns>
    private bool WasTappedDown(){        
        return NRInput.GetButtonDown(ControllerButton.TRIGGER);

    }
    /// <summary> Detect whether user release the trigger button </summary>
    /// <returns> true if trigger is released </returns>
    private bool WasTappedUp(){
        return NRInput.GetButtonUp(ControllerButton.TRIGGER);
    }

}
