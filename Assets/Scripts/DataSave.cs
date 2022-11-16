using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using NRKernal;
public class DataSave : MonoBehaviour
{

    private List<string> timestamp = new List<string>();
    private List<int> pos = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        timestamp.Add("10.10");
        pos.Add(123);
        pos.Add(456);
        timestamp.Add("12.3");
        saveData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void saveData(){
        string filename = string.Format("pace_result_{0}.txt", NRTools.GetTimeStamp().ToString());
        string folder = string.Format("{0}/PaceData", Application.persistentDataPath);
        if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        string[] result = new string[pos.Count];

        for(int i=0;i<pos.Count;i++){
            result[i] = timestamp[i]+" "+ pos[i].ToString();
        }
        File.WriteAllLines(string.Format("{0}/{1}", folder, filename), result);
        Debug.Log(string.Format("InsertPaceData: {0}/{1}", filename, folder));

    }
}
