// Filename:    ScoreCounting.cs 
// Summary:     Count score based on the type of object, this script is attached to the user
// Author:      Zhuoru Zhang
// Date:        2022/10/29
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal.NRExamples;
public class scoreCounting : MonoBehaviour
{
    /// <summary> total score of the game </summary>
    public int scores;
    /// <summary> Canvas for the finish </summary>
    GameObject m_Canvas;


    // Start is called before the first frame update
    void Start()
    {
        scores = 0;
        // m_Canvas = GameObject.Find("Finish");
        // m_Canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary> After collision, trigger will be entered automatically </summary>
    void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Reward") {
            var m_script = other.GetComponent<SimpleCollectibleScript>();
			if(m_script.CollectibleType==SimpleCollectibleScript.CollectibleTypes.Positive){
                scores+=10;
            }
            else if(m_script.CollectibleType==SimpleCollectibleScript.CollectibleTypes.Negative){
                scores-=10;
            }
            else if(m_script.CollectibleType==SimpleCollectibleScript.CollectibleTypes.Finish){
                // m_Canvas.SetActive(true);
                NRHomeMenu.Show();
            }
            else{
                
            }
		}
	}
}
