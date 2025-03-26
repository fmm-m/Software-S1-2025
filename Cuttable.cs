using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    
    public GameObject testPlaneObj;
    Plane testPlane;

    // Start is called before the first frame update
    void Start()
    {
        testPlane = testPlaneObj.GetComponent<plane>().selfPlane;
        
    }

    // Update is called once per frame
    void Update()
    {
        testPlane = testPlaneObj.GetComponent<plane>().selfPlane;




        //Debug.Log(testPlane.transform.position);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Slice");
            splitMesh(testPlane);


        }
        
    }

    public void splitMesh(Plane slicingPlane)
    {
        slicingPlane.Translate(gameObject.transform.position);
        Debug.DrawRay(transform.position, slicingPlane.normal);
        
        
        
        
        Mesh positiveMesh = new Mesh();
        List<Vector3> positiveVertices = new List<Vector3>();
        List<int> positiveTriangles = new List<int>();

        Mesh negativeMesh = new Mesh();
        List<Vector3> negativeVertices = new List<Vector3>();
        List<int> negativeTriangles = new List<int>();

        Mesh selfMesh = GetComponent<MeshFilter>().mesh;

        Vector3[] selfVertices = selfMesh.vertices;
        int[] selfTriangles = selfMesh.triangles;

        List<Vector3> newPointsOnPlane = new List<Vector3>();

        for (int triangleIndex = 0; triangleIndex < selfTriangles.Length / 3; triangleIndex++)
        {
            int[] triangle = { selfTriangles[3 * triangleIndex], selfTriangles[3 * triangleIndex + 1], selfTriangles[3 * triangleIndex + 2], };

            // Sort vertices to positive/negative
            List<int> positivePoints = new List<int>();
            List<int> negativePoints = new List<int>();


            foreach (int verticeIndex in triangle)
            {
                if (slicingPlane.GetSide(selfVertices[verticeIndex]))
                {
                    positivePoints.Add(verticeIndex);
                } else
                {
                    negativePoints.Add(verticeIndex);
                }
            }

            Debug.Log($"Positive Points: {positivePoints.Count}");
            Debug.Log($"Negative Points: {negativePoints.Count}");

            int positiveVerticeCount = positivePoints.Count;
            //Debug.Log(positiveVerticeCount);
            //Debug.Log(negativeVertices.Count);

            // Handle the first case (All vertices fall on one side). This would theoretically work by itself, but any triangles cut in half wouldn't be there which would give it a weird jaggered look.
            int negVertCount = negativeVertices.Count;
            int posVertCount = positiveVertices.Count;
            //Debug.Log(posVertCount);

            if (positiveVerticeCount == 0)
            {
                for (int i = 0; i <= 2; i++)
                {
                    negativeVertices.Add(selfVertices[triangle[i]]);
                    negativeTriangles.Add(negVertCount + i);

                }
            } else if (positiveVerticeCount == 3)
            {

                for (int i = 0; i <= 2; i++)
                {
                    positiveVertices.Add(selfVertices[triangle[i]]);
                    positiveTriangles.Add(posVertCount + i);

                }

            } else
            {
                Vector3 A;
                Vector3 B;
                

                if (positiveVerticeCount == 1)
                {
                    Vector3 positivePoint = selfVertices[positivePoints[0]];

                    Vector3 dirA = selfVertices[negativePoints[1]] - positivePoint;
                    slicingPlane.Raycast(new Ray(positivePoint, dirA), out float intersectDistA);
                    Vector3 dirB = selfVertices[negativePoints[0]] - positivePoint;
                    slicingPlane.Raycast(new Ray(positivePoint, dirB), out float intersectDistB);

                    A = positivePoint + intersectDistA * dirA.normalized;
                    B = positivePoint + intersectDistB * dirB.normalized;

                    positiveVertices.Add(positivePoint);
                    positiveVertices.Add(A);
                    positiveVertices.Add(B);

                    newPointsOnPlane.Add(A);
                    newPointsOnPlane.Add(B);
                    

                    positiveTriangles.Add(posVertCount + 2);
                    positiveTriangles.Add(posVertCount + 1);
                    positiveTriangles.Add(posVertCount + 0);
                    positiveTriangles.Add(posVertCount + 2);
                    positiveTriangles.Add(posVertCount + 0);
                    positiveTriangles.Add(posVertCount + 1);



                    negativeVertices.Add(selfVertices[negativePoints[0]]);
                    negativeVertices.Add(selfVertices[negativePoints[1]]);
                    negativeVertices.Add(A);
                    negativeVertices.Add(B);

                    negativeTriangles.Add(negVertCount + 0);
                    negativeTriangles.Add(negVertCount + 1);
                    negativeTriangles.Add(negVertCount + 2);
                    negativeTriangles.Add(negVertCount + 0);
                    negativeTriangles.Add(negVertCount + 2);
                    negativeTriangles.Add(negVertCount + 1);

                    negativeTriangles.Add(negVertCount + 3);
                    negativeTriangles.Add(negVertCount + 0);
                    negativeTriangles.Add(negVertCount + 2);
                    negativeTriangles.Add(negVertCount + 3);
                    negativeTriangles.Add(negVertCount + 2);
                    negativeTriangles.Add(negVertCount + 0);





                }
                else
                {
                    Vector3 negativePoint = selfVertices[negativePoints[0]];

                    Vector3 dirA = selfVertices[positivePoints[0]] - negativePoint;
                    slicingPlane.Raycast(new Ray(negativePoint, dirA), out float intersectDistA);
                    Vector3 dirB = selfVertices[positivePoints[1]] - negativePoint;
                    slicingPlane.Raycast(new Ray(negativePoint, dirB), out float intersectDistB);

                    A = negativePoint + intersectDistA * dirA.normalized;
                    B = negativePoint + intersectDistB * dirB.normalized;

                    negativeVertices.Add(negativePoint);
                    negativeVertices.Add(A);
                    negativeVertices.Add(B);

                   

                  
                    negativeTriangles.Add(negVertCount + 2);
                    negativeTriangles.Add(negVertCount + 1);
                    negativeTriangles.Add(negVertCount + 0);
                    negativeTriangles.Add(negVertCount + 2);
                    negativeTriangles.Add(negVertCount + 0);
                    negativeTriangles.Add(negVertCount + 1);

                    positiveVertices.Add(selfVertices[positivePoints[0]]);
                    positiveVertices.Add(selfVertices[positivePoints[1]]);
                    positiveVertices.Add(A);
                    positiveVertices.Add(B);
                    newPointsOnPlane.Add(A);
                    newPointsOnPlane.Add(B);

                    positiveTriangles.Add(posVertCount + 0);
                    positiveTriangles.Add(posVertCount + 2);
                    positiveTriangles.Add(posVertCount + 1);
                    positiveTriangles.Add(posVertCount + 0);
                    positiveTriangles.Add(posVertCount + 1);
                    positiveTriangles.Add(posVertCount + 2);


                    positiveTriangles.Add(posVertCount + 1);
                    positiveTriangles.Add(posVertCount + 2);
                    positiveTriangles.Add(posVertCount + 3);
                    positiveTriangles.Add(posVertCount + 1);
                    positiveTriangles.Add(posVertCount + 3);
                    positiveTriangles.Add(posVertCount + 2);








                }
            }
            /*
            if (newPointsOnPlane.Count != 0) // Mesh slicing
            {
                Vector3 averagePosition = Vector3.zero;

                foreach (Vector3 point in newPointsOnPlane)
                {
                    averagePosition += point;
                }
                averagePosition /= newPointsOnPlane.Count;

                positiveVertices.Add(averagePosition);
                int positiveCenterIndex = positiveVertices.Count - 1;

                negativeVertices.Add(averagePosition);
                int negativeCenterIndex = negativeVertices.Count - 1;

                for (int tIndex = 0; tIndex < (int)newPointsOnPlane.Count / 2; tIndex++)
                {
                    positiveVertices.Add(newPointsOnPlane[2 * tIndex]);
                    positiveVertices.Add(newPointsOnPlane[2 * tIndex + 1]);

                    positiveTriangles.Add(positiveCenterIndex);
                    positiveTriangles.Add(positiveVertices.Count - 1);
                    positiveTriangles.Add(positiveVertices.Count - 2);

                    
                    positiveTriangles.Add(positiveCenterIndex);
                    positiveTriangles.Add(positiveVertices.Count - 2);
                    positiveTriangles.Add(positiveVertices.Count - 1);
                    
                    negativeVertices.Add(newPointsOnPlane[2 * tIndex]);
                    negativeVertices.Add(newPointsOnPlane[2 * tIndex + 1]);

                    
                    negativeTriangles.Add(negativeCenterIndex);
                    negativeTriangles.Add(negativeVertices.Count - 1);
                    negativeTriangles.Add(negativeVertices.Count - 2);
                    
                    negativeTriangles.Add(negativeCenterIndex);
                    negativeTriangles.Add(negativeVertices.Count - 2);
                    negativeTriangles.Add(negativeVertices.Count - 1);
                }

            } */

        }

        
        if (positiveVertices.Count != 0)
        {

            positiveMesh.vertices = positiveVertices.ToArray();
            positiveMesh.triangles = positiveTriangles.ToArray();
            positiveMesh.RecalculateNormals();
            //positiveMesh.RecalculateTangents();
     
            
            GameObject positiveCopy = gameObject;
            positiveCopy.GetComponent<MeshFilter>().mesh = positiveMesh;
            positiveCopy.GetComponent<MeshCollider>().sharedMesh = positiveMesh;
            Instantiate(positiveCopy);
        }

        if (negativeVertices.Count != 0)
        {
            negativeMesh.vertices = negativeVertices.ToArray();
            negativeMesh.triangles = negativeTriangles.ToArray();
           negativeMesh.RecalculateNormals();
           // negativeMesh.RecalculateTangents();

            GameObject negativeCopy = gameObject;
            negativeCopy.GetComponent<MeshFilter>().mesh = negativeMesh;
            negativeCopy.GetComponent<MeshCollider>().sharedMesh = negativeMesh;
            Instantiate(negativeCopy);
        }

        
        
        
        Destroy(gameObject);

    }

    
}
