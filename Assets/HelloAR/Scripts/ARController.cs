using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

public class ARController : MonoBehaviour
{
    //We will fill the list with planes that ARCore dected in the current frame
    private List<TrackedPlane> m_NewTrackedPlanes = new List<TrackedPlane>();
    // Start is called before the first frame update
    public GameObject GridPrefab;
    public GameObject Portal;
    public GameObject ARCamera;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ARCore session status
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        //The following function will fill m_NewTrackedPlanes with the planes that ARCore detected in the current frame
        Session.GetTrackables<TrackedPlane>(m_NewTrackedPlanes, TrackableQueryFilter.New);
        //Instantiate a Grid for each Tracked Plane in m_NewTrackedPlanes
        for(int i =0; i <m_NewTrackedPlanes.Count; i++)
        {
             GameObject grid =Instantiate(GridPrefab,Vector3.zero,Quaternion.identity,transform);

             //This function will set the postition of the grid and modify the vertices of the attached mesh
             grid.GetComponent<GridVisualiser>().Initialize(m_NewTrackedPlanes[i]);        
        }

        //Check if  the user touches the screeen
        Touch touch;
        if(Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        //Let's now check if the  user touched  any of the tracked plane
        TrackableHit hit;
        if(Frame.Raycast(touch.position.x,touch.position.y, TrackableHitFlags.PlaneWithinPolygon,out hit))
        {
            //let's now place the portal on top of the tracked plane that we touched

            //Enable  the portal

            Portal.SetActive(true);

            //Create a new Anchor
            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

            // Set the  position of the portal  to be the same as the hit position 
            Portal.transform.position = hit.Pose.position;
            Portal.transform.rotation = hit.Pose.rotation;

            //We want the portal to face the camera  
            Vector3 cameraPosition = ARCamera.transform.position;

            //The portal should only rotate on the y axis
            cameraPosition.y = hit.Pose.position.y;

            //Rotate the portal to face the camera
            Portal.transform.LookAt(cameraPosition, Portal.transform.up);
            
            //ARCore will keep understanding the world and update the anchors accordingly hence we need to attach out portal to anchor
            Portal.transform.parent = anchor.transform;

            
        }
    }
}
