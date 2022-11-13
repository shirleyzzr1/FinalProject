// Filename:    CreateCube.cs 
// Summary:     Create Cube at the end of laser after clicking the button on the menu
// Author:      Zhuoru Zhang
// Date:        2022/10/29
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using UnityEngine.UI;
public class CreateCube : MonoBehaviour
{
    /// <summary> ObjectPrefab </summary>
    public GameObject objectPrefab;

    /// <summary> Create Button </summary>
    public Button createBtn;

    Transform laserAnchor;
    void Start()
    {
        createBtn.onClick.AddListener(onClickButton);
    }

    // Update is called once per frame
    void Update()
    {
        var handControllerAnchor = NRInput.DomainHand == ControllerHandEnum.Left ? ControllerAnchorEnum.LeftLaserAnchor : ControllerAnchorEnum.RightLaserAnchor;
        laserAnchor = NRInput.AnchorsHelper.GetAnchor(NRInput.RaycastMode == RaycastModeEnum.Gaze ? ControllerAnchorEnum.GazePoseTrackerAnchor : handControllerAnchor);
    }

    /// <summary> Instantiate cube and reset laser length </summary>
    void onClickButton(){
        GameObject cube1 = Instantiate(objectPrefab,createBtn.transform.position,Quaternion.identity);
        cube1.GetComponent<MoveObject>().laserLen = Vector3.Distance(createBtn.transform.position,laserAnchor.transform.position);
        cube1.GetComponent<MoveObject>().isDragging = true;
    }
}
