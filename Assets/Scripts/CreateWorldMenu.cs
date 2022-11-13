using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreateWorldMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public Button coinBtn;
    public Button DiamondBtn;



    void Start()
    {
        coinBtn.onClick.AddListener(OncoinBtnClick);
        DiamondBtn.onClick.AddListener(DiamondBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OncoinBtnClick(){

    }

    void DiamondBtnClick(){
        
    }
}
