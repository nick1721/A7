using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S2CAlgo : MonoBehaviour
{
    public Vector3 vectorSource, vectorDestination, prevLocation, prevForwardVector;
    public float accelerateThreshold, decelerateThreshold, howMuchUserRotated, directionUserRotated, distanceFromCenter;
    public float prevYawRelativeToCenter, deltaYawRelativeToCenter, howMuchToAccelerate, translationalGainThreshold;
    public GameObject Camera, VRTrackingOrigin;
    public float d(Vector3 A, Vector3 B, Vector3 C) 
        {
            A.y = 0f;
            B.y = 0f;
            return (A.x-B.x)*(C.y-B.y)-(A.y-B.y)*(C.x-B.x);
        }
    public float angleBetweenVectors(Vector3 A, Vector3 B) 
    {
        
        return (float)(Mathf.Acos(Vector3.Dot(Vector3.Normalize(A), Vector3.Normalize(B))))*Mathf.Rad2Deg; 
    }
    // Start is called before the first frame update
    void Start()
    {
       prevForwardVector = Camera.transform.position;
       prevLocation = Camera.transform.position;
       prevYawRelativeToCenter = angleBetweenVectors(Camera.transform.forward, VRTrackingOrigin.transform.position - Camera.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        howMuchUserRotated = angleBetweenVectors(prevForwardVector, Camera.transform.forward);
        directionUserRotated = (d((Camera.transform.position + prevForwardVector), Camera.transform.position, Camera.transform.position + Camera.transform.forward)< 0) ? 1 : -1;
        deltaYawRelativeToCenter = prevYawRelativeToCenter-angleBetweenVectors(Camera.transform.forward,VRTrackingOrigin.transform.position-Camera.transform.position);
        distanceFromCenter = (Camera.transform.position-VRTrackingOrigin.transform.position).magnitude;
        float longestDimensionOfPE = .4f;
        howMuchToAccelerate = ((deltaYawRelativeToCenter<0)? -decelerateThreshold: accelerateThreshold * howMuchUserRotated * directionUserRotated * Mathf.Clamp(distanceFromCenter/longestDimensionOfPE/2,0,1));
        if (Mathf.Abs(howMuchToAccelerate)>0) 
        {
            VRTrackingOrigin.transform.RotateAround(Camera.transform.position, new Vector3(0f,1f,0f), howMuchToAccelerate);
        }
        prevForwardVector = Camera.transform.forward;
        prevYawRelativeToCenter = angleBetweenVectors(Camera.transform.forward, VRTrackingOrigin.transform.position - Camera.transform.position);
        
        if (Input.GetKeyDown("q")) 
        {
            Debug.Log("Q");
            RaycastHit hit;
            if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hit, 1f))
            {
                Destroy(hit.collider.gameObject);
            }
        }

        // Extra Credit:
        Vector3 trajectoryVector = Camera.transform.position - prevLocation;
        Vector3 howMuchToTranslate = Vector3.Normalize(trajectoryVector) * translationalGainThreshold;
        VRTrackingOrigin.transform.position += howMuchToTranslate;
        prevLocation = Camera.transform.position;
       
        
    }
}
