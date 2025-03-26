using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : MonoBehaviour
{

    Vector3 objectEnter;
    Vector3 objectExit;
    Vector3 objectEnterP2;
    Plane? slicingPlane;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent < Cutter > () != null && slicingPlane == null)
        {
            objectEnter = transform.position;
            objectEnterP2 = transform.position + transform.rotation.eulerAngles;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        
        if (collision.gameObject.GetComponent<Cuttable>() != null && slicingPlane == null)
        {
            objectExit = transform.position;
        }
        slicingPlane = new Plane(objectEnter, objectEnterP2, objectExit);
        collision.gameObject.GetComponent<Cuttable>().splitMesh((Plane)slicingPlane);


    }

    void resetPositionalVariables()
    {
        objectEnter = Vector3.zero;
        objectExit = Vector3.zero;
        slicingPlane = null;
    }
}
