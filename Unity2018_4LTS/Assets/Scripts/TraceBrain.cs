using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using System.Linq;
using Vector3 = UnityEngine.Vector3;

public class TraceBrain : MonoBehaviour
{
    public Vector3Int pos;
    public string pathToFile;

    List<Vector3Int> locations;

    private int vecLength = 9;
    private double magnification = Mathf.Pow(10, 15);
    private double step = 0.5;
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
    List<Vector3Int> traceBrain(Vector3Int origin, System.Array tensorField, Vector3 internalKeeping)
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
            float maxMag = -1;
            Vector3 major=new Vector3();
            int maxCounter = 1;
            foreach(Vector3 i in vectors)
            {
                float tmp = i.magnitude;
                if (tmp > maxMag)
                {
                    maxMag = tmp;
                    major = i;
                    maxCounter = 1;
                }else if(tmp == maxMag)
                {
                    maxCounter += 1;
                }
            }

            // empty voxel
            if (maxMag == 0)
            {
                return new List<Vector3Int>();
            }

            // no major
            if (maxCounter == 3)
            {
                return new List<Vector3Int>();
            }
            //Question: how to proceed to next location? What should be the length of each vector?

            //find next location and invoke transBrain recursively
            Vector3 nextVoxel = internalKeeping;
            nextVoxel.x = nextVoxel.x + (float)(major.x * step);
            nextVoxel.y = nextVoxel.y + (float)(major.y * step);
            nextVoxel.z = nextVoxel.z + (float)(major.z * step);
            Vector3Int externalVoxel = new Vector3Int((int)(nextVoxel.x), (int)(nextVoxel.y), (int)(nextVoxel.z));
            //get result from recursion and add current location AT FRONT
            if (externalVoxel.Equals(origin))
            {
                return traceBrain(externalVoxel, tensorField, nextVoxel);
            }
            else
            {
                var result = traceBrain(externalVoxel, tensorField, nextVoxel);
                result.Insert(0, externalVoxel);
                return result;
            }




        }
        else
        {
            return new List<Vector3Int>();
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
