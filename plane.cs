using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plane : MonoBehaviour
{
    public Plane selfPlane;

    // Start is called before the first frame update
    void Start()
    {
        selfPlane = new Plane(transform.up, transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        selfPlane.SetNormalAndPosition(transform.up, transform.position);

    }


}
