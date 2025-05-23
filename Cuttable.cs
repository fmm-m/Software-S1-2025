using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class Cuttable : MonoBehaviour
{
    

    private Vector3 planeNormal;
    private Vector3 planePosition;
    List<Plane> cuttingPlanes = new List<Plane>();
    public GameObject[] planeObjs;
    public float m = 2f;

    public GameObject button;
    public GameObject validCutTrigger;

    bool lastButtonState = false;


        Vector3 DebugRay;
    public int cutNum;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    bool playerInBounds;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "InCutter")
        {
            playerInBounds = true;
            //Debug.Log(playerInBounds);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "InCutter")
        {
            playerInBounds = false;
            Debug.Log(playerInBounds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject planeObj in planeObjs)
        {
            cuttingPlanes.Add(planeObj.GetComponent<cutPlane>().selfPlane);
        }
        //Debug.DrawLine(transform.position, DebugRay, Color.red);


        //Debug.Log(testPlane.transform.position);
        Debug.Log(playerInBounds);
        if (button.GetComponent<HoverButton>().engaged && lastButtonState == false && playerInBounds)
        {
            
            Debug.Log("Slice");
            cutNum += 1;
            Plane chosenPlane = cuttingPlanes[(int)Random.Range(0f, (float)cuttingPlanes.Count)];
            Debug.Log(chosenPlane);
            //gameObject.GetComponent<Rigidbody>().velocity += new Vector3(Random.Range(-m, m), Random.Range(-m, m), Random.Range(-m, m));
            splitMesh(chosenPlane);


        }
        lastButtonState = button.GetComponent<HoverButton>().engaged;
    }
    public Vector3 calculateNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 dir = Vector3.Cross((p2 - p1), (p3 - p1));
        Vector3 normal = dir.normalized;
        return normal;
    }

   
    public void splitMesh(Plane slicingPlane)
    {
        
        Vector3 originalPos = slicingPlane.ClosestPointOnPlane(transform.position);

        Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        rotation = Quaternion.Inverse(rotation);
        Vector3 rotatedPos = (rotation * (originalPos - transform.position)) + transform.position;
        Vector3 rotatedNormal = rotation * slicingPlane.normal;
        slicingPlane.SetNormalAndPosition(rotatedNormal, rotatedPos);
        slicingPlane.Translate(transform.position);
        
        //Debug.Log($"RotatedPos {rotatedPos}");

       



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
        //Debug.Log(newPointsOnPlane.Count);

        for (int triangleIndex = 0; triangleIndex < selfTriangles.Length / 3; triangleIndex++)
        {
            int[] triangle = { selfTriangles[3 * triangleIndex], selfTriangles[3 * triangleIndex + 1], selfTriangles[3 * triangleIndex + 2], };

            // Sort vertices to positive/negative
            List<int> positivePoints = new List<int>();
            List<int> negativePoints = new List<int>();


            Vector3 normal = calculateNormal(selfVertices[triangle[0]], selfVertices[triangle[1]], selfVertices[triangle[2]]);

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
                    
                    if ((calculateNormal(positivePoint, A, B) - normal).magnitude <= 0.2)
                    {
                        positiveTriangles.Add(posVertCount + 0);
                        positiveTriangles.Add(posVertCount + 1);
                        positiveTriangles.Add(posVertCount + 2);
                    } else
                    {
                        positiveTriangles.Add(posVertCount + 0);
                        positiveTriangles.Add(posVertCount + 2);
                        positiveTriangles.Add(posVertCount + 1);
                    }
                    
              



                    negativeVertices.Add(selfVertices[negativePoints[0]]);
                    negativeVertices.Add(selfVertices[negativePoints[1]]);
                    negativeVertices.Add(A);
                    negativeVertices.Add(B);

                    if ((calculateNormal(selfVertices[negativePoints[0]], selfVertices[negativePoints[1]], A) - normal).magnitude <= 0.2)
                    {
                        negativeTriangles.Add(negVertCount + 0);
                        negativeTriangles.Add(negVertCount + 1);
                        negativeTriangles.Add(negVertCount + 2);
                    } else
                    {
                        negativeTriangles.Add(negVertCount + 0);
                        negativeTriangles.Add(negVertCount + 2);
                        negativeTriangles.Add(negVertCount + 1);
                    }


                    if ((calculateNormal(selfVertices[negativePoints[0]], A, B) - normal).magnitude <= 0.2)
                    {
                        negativeTriangles.Add(negVertCount + 0);
                        negativeTriangles.Add(negVertCount + 2);
                        negativeTriangles.Add(negVertCount + 3);
                    } else
                    {
                        negativeTriangles.Add(negVertCount + 0);
                        negativeTriangles.Add(negVertCount + 3);
                        negativeTriangles.Add(negVertCount + 2);
                    }
                    
                        
                 




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
                    



                    if ((calculateNormal(negativePoint, A, B) - normal).magnitude <= 0.2)
                    {
                        negativeTriangles.Add(negVertCount + 0);
                        negativeTriangles.Add(negVertCount + 1);
                        negativeTriangles.Add(negVertCount + 2);
                    }
                    else
                    {
                        negativeTriangles.Add(negVertCount + 0);
                        negativeTriangles.Add(negVertCount + 2);
                        negativeTriangles.Add(negVertCount + 1);
                    }
                    
                  

                    positiveVertices.Add(selfVertices[positivePoints[0]]);
                    positiveVertices.Add(selfVertices[positivePoints[1]]);
                    positiveVertices.Add(A);
                    positiveVertices.Add(B);
                    newPointsOnPlane.Add(A);
                    newPointsOnPlane.Add(B);
                    if ((calculateNormal(selfVertices[positivePoints[0]], selfVertices[positivePoints[1]], A) - normal).magnitude < 0.2) {
                        positiveTriangles.Add(posVertCount + 0);
                        positiveTriangles.Add(posVertCount + 1);
                        positiveTriangles.Add(posVertCount + 2);
                    } else
                    {
                        positiveTriangles.Add(posVertCount + 0);
                        positiveTriangles.Add(posVertCount + 2);
                        positiveTriangles.Add(posVertCount + 1);
                    }



                    if ((calculateNormal(selfVertices[positivePoints[1]], A, B) - normal).magnitude < 0.2)
                    {
                        positiveTriangles.Add(posVertCount + 1);
                        positiveTriangles.Add(posVertCount + 2);
                        positiveTriangles.Add(posVertCount + 3);
                    } else
                    {
                        positiveTriangles.Add(posVertCount + 1);
                        positiveTriangles.Add(posVertCount + 3);
                        positiveTriangles.Add(posVertCount + 2);
                    }






                }
            }
            
            
            

        }
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

            for (int tIndex = 0; tIndex < newPointsOnPlane.Count - 1; tIndex++)
            {
                positiveVertices.Add(newPointsOnPlane[tIndex]);
                positiveVertices.Add(newPointsOnPlane[tIndex + 1]);

                if ((slicingPlane.normal - calculateNormal(positiveVertices[positiveVertices.Count - 1], positiveVertices[positiveVertices.Count - 2], positiveVertices[positiveCenterIndex])).magnitude <= 0.2)
                {
                    positiveTriangles.Add(positiveCenterIndex);
                    positiveTriangles.Add(positiveVertices.Count - 2);
                    positiveTriangles.Add(positiveVertices.Count - 1);
                }
                else
                {
                    positiveTriangles.Add(positiveCenterIndex);
                    positiveTriangles.Add(positiveVertices.Count - 1);
                    positiveTriangles.Add(positiveVertices.Count - 2);
                }





                negativeVertices.Add(newPointsOnPlane[tIndex]);
                negativeVertices.Add(newPointsOnPlane[tIndex + 1]);

                if ((slicingPlane.normal - calculateNormal(negativeVertices[negativeVertices.Count - 1], negativeVertices[negativeVertices.Count - 2], negativeVertices[negativeCenterIndex])).magnitude <= 0.2)
                {
                    negativeTriangles.Add(negativeCenterIndex);
                    negativeTriangles.Add(negativeVertices.Count - 1);
                    negativeTriangles.Add(negativeVertices.Count - 2);
                }
                else
                {
                    negativeTriangles.Add(negativeCenterIndex);
                    negativeTriangles.Add(negativeVertices.Count - 2);
                    negativeTriangles.Add(negativeVertices.Count - 1);
                }




            }

        }
        string ogName = name;
        

        newPointsOnPlane = new List<Vector3>();
        if (positiveVertices.Count != 0)
        {

            positiveMesh.vertices = positiveVertices.ToArray();
            positiveMesh.triangles = positiveTriangles.ToArray();
            positiveMesh.RecalculateNormals();
            name = $"{ogName}Positive";
            positiveMesh.RecalculateTangents();
            
            


            GameObject positiveCopy = gameObject;
            //positiveCopy.GetComponent<Rigidbody>().velocity += new Vector3(Random.Range(-m, m), Random.Range(-m, m), Random.Range(-m, m));
            positiveCopy.GetComponent<MeshFilter>().mesh = positiveMesh;
            positiveCopy.GetComponent<MeshCollider>().sharedMesh = positiveMesh;
            Instantiate(positiveCopy);
            
        }

        if (negativeVertices.Count != 0)
        {
            negativeMesh.vertices = negativeVertices.ToArray();
            negativeMesh.triangles = negativeTriangles.ToArray();
           negativeMesh.RecalculateNormals();
            name = $"{ogName}Negative";
            negativeMesh.RecalculateTangents();

            GameObject negativeCopy = gameObject;
            //negativeCopy.GetComponent<Rigidbody>().velocity += new Vector3(Random.Range(-m, m), Random.Range(-m, m), Random.Range(-m, m));
            negativeCopy.GetComponent<MeshFilter>().mesh = negativeMesh;
            negativeCopy.GetComponent<MeshCollider>().sharedMesh = negativeMesh;
            Instantiate(negativeCopy);
        }

        
        
        
        Destroy(gameObject);

    }

    
}
