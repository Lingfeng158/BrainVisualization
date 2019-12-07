using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    float rotSpeed = 20;

    void OnMouseDrag()
    {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        GameObject[] gos = GameObject.FindGameObjectsWithTag("RotationCenter");
        foreach (GameObject go in gos)
        {
            go.transform.RotateAround(Vector3.up, -rotX);
            go.transform.RotateAround(Vector3.right, rotY);
        }

        
    }

}
