using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;

public class NPYIO : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Note data read in are int64 values that are 10^15 times larger than original data
        //to accommodate limitation of Accord.IO that it doesn't support float
        var arr=Accord.IO.NpyFormat.LoadMatrix(@"C:\Users\lil18\Desktop\BrainVisualization\Unity2018_4LTS\Assets\tmp\sample.npy");
        int[] dims = new int[] { 0,0,0,0};
        for (int i = 0; i < arr.Rank; i++)
        {
            dims[i]= arr.GetLength(i);
        }
        print(arr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
