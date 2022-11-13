// Filename:    ObjectSpawner.cs 
// Summary:     Select object within the menu
// Author:      Zhuoru Zhang
// Date:        2022/11/2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;

public class SelectObject : MonoBehaviour
{
    /// <summary> Flag to roatet the object </summary>
    bool rotate=true;

    // Update is called once per frame
    void Update()
    {
        var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        Transform laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);
        
        RaycastHit hit;

        if (Physics.Raycast(new Ray(laserAnchor.transform.position, laserAnchor.transform.forward), out hit, 10))
            {   var hitResult = hit.collider.gameObject; 
                if(hitResult == this.gameObject){
                    rotate = false;
                    // if tapped down, the object is selected, send object name to ObjectSpawner
                    if(WasTappedDown()) ObjectSpawner.prefabName = this.gameObject.name;

                }
                else{
                    rotate = true;
                }
            }
        if(rotate){
            this.transform.Rotate (Vector3.up * 10 * Time.deltaTime, Space.World);

        }
        
    }


    /// <summary> Detect whether user press the trigger button </summary>
    /// <returns> true if trigger is pressed </returns>

    private bool WasTappedDown(){        
        return NRInput.GetButtonDown(ControllerButton.TRIGGER);

    }
}
