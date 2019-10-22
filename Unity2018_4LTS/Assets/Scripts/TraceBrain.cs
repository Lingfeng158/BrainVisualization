using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using System.Linq;
using Vector3 = UnityEngine.Vector3;

public class TraceBrain : MonoBehaviour
{
    public Vector3 pos;
    public string pathToFile;

    List<Vector3> locations;

    private int vecLength = 9;
    private double magnification = Mathf.Pow(10, 15);
    // Start is called before the first frame update
    void Start()
    {
        var arr = Accord.IO.NpyFormat.LoadMatrix(pathToFile);

        locations = traceBrain(pos, arr, pos);

    }

    /*
     * origin: location start to work with
     * tensorField: entire tensorField working with, i.e. numpy 4d matrix from nrrd
     * internalKeeping: true location without rounding
     * return: a list of locations in image
     */
    List<Vector3> traceBrain(Vector3 origin, System.Array tensorField, Vector3 internalKeeping)
    {
        int xDim = tensorField.GetUpperBound(0);
        int yDim = tensorField.GetUpperBound(1);
        int zDim = tensorField.GetUpperBound(2);
        if (pos.x <= xDim && pos.y <= yDim && pos.z <= zDim)
        {
            List<float> subArr = new List<float>();
            for (int t = 0; t < vecLength; t++)
            {
                long tmp = (long)tensorField.GetValue((int)pos.x, (int)pos.y, (int)pos.z, t);
                subArr.Add((float)(tmp / magnification));
            }

            List<Vector3> vectors = new List<Vector3>();
            list2vector(subArr, vectors);
            //find longest vector

            //Question: how to proceed to next location? What should be the length of each vector?
            
            //find next location and invoke transBrain recursively

            //get result from recursion and add current location AT FRONT





        }
        else
        {
            return new List<Vector3>();
        }

    }

    /*
     * convert 9*1 list to 3*3 vectors
     */
    void list2vector(List<float> subArr, List<Vector3> vectors)
    {
        //if vector stored as column vector
        Vector3 a = new Vector3(subArr[0], subArr[1], subArr[2]);
        Vector3 b = new Vector3(subArr[3], subArr[4], subArr[5]);
        Vector3 c = new Vector3(subArr[6], subArr[7], subArr[8]);
        vectors.Add(a);
        vectors.Add(b);
        vectors.Add(c);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
