using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;
using UnityEngine.Networking;



public class PlaceIt : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    public GameObject pointer;
    //ui button
    public GameObject reset;

    public GameObject spawnedObject;
    private ARSessionOrigin arOrigin;
    private Pose placementPose;
    private bool placementPoseIsValid = false;


    private float initialDistance;
    private Vector3 initialScale;
    private Touch touch;
    private float speedModifier = 0.01f;
    bool isRotationEnabled;


   


      // Public property to access spawnedObject
    public GameObject GetSpawnedObject()
    {
        return spawnedObject;
    }

    public GameObject SetSpawnedObject(GameObject input)
    {
        spawnedObject = input;
        return spawnedObject;
    }
    public  void ResetScene(){
        if(spawnedObject != null){
            Destroy(spawnedObject);
            spawnedObject = null;
            placementIndicator.SetActive(true);
            pointer.SetActive(false);

            
        }
    }
    

  private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && spawnedObject==null)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            var cameraForward = Camera.current.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    void Start()
    {
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        pointer.SetActive(false);
        isRotationEnabled = true;
        StartCoroutine(WaitForObject());

    }
    

        IEnumerator WaitForObject()
        {
            // Wait until the object with the name "1" is spawned
            GameObject obj = null;
            while (obj == null)
            {
                obj = GameObject.Find("1");
                yield return null;
            }
            obj.transform.position = new Vector3(100000.0f, 2.0f, 3.0f);
            objectToPlace = obj;
        // Execute your code here
        Debug.Log("Object with name '1' has been spawned.");
        }


void Update()
{
    UpdatePlacementPose();
    UpdatePlacementIndicator();

    if (spawnedObject == null && placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        PlaceObject();
        GetComponent<OnClickHideOthers>().SetHideObject();
    }

    if (spawnedObject != null)
    {
        if (Input.touchCount == 2)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                return;
            }

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = spawnedObject.transform.localScale;
            }
            else
            {
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                if (Mathf.Approximately(initialDistance, 0))
                {
                    return;
                }

                var factor = currentDistance / initialDistance;
                spawnedObject.transform.localScale = isRotationEnabled ? initialScale * factor : initialScale;
            }
        }

        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (!isRotationEnabled)
                {
                    // Move the object
                    float moveSpeed = 0.001f;
                    Vector3 deltaPos = new Vector3(Input.GetTouch(0).deltaPosition.x, 0, Input.GetTouch(0).deltaPosition.y) * moveSpeed;
                    spawnedObject.transform.position += deltaPos;
                }
                else
                {
                    // Rotate the object
                    float rotationSpeed = 0.1f;
                    float deltaX = Input.GetTouch(0).deltaPosition.x * rotationSpeed;

                    Vector3 center = spawnedObject.transform.position;
                    Quaternion rotation = Quaternion.Euler(0, -deltaX, 0);

                    spawnedObject.transform.RotateAround(center, Vector3.up, -deltaX);
                }
            }
        }
    }
}

public void ToggleRotation()
{
    isRotationEnabled = !isRotationEnabled;
}


public void PlaceObject()
{
    spawnedObject = Instantiate(objectToPlace, placementPose.position, placementPose.rotation * objectToPlace.transform.rotation);
    spawnedObject.name = "spawnedObject";


    


    //hide placementIndicator
    placementIndicator.SetActive(false);
    //canvas pointer show
    pointer.SetActive(true);

    MeshRenderer[] modelParts;

    // Cache the MeshRenderer components of the children
    modelParts = spawnedObject.GetComponentsInChildren<MeshRenderer>();

    // Add collider and rigidbody components to each child
    foreach (var modelPart in modelParts)
    {
        modelPart.gameObject.AddComponent<MeshCollider>();
        modelPart.gameObject.AddComponent<Rigidbody>();
        modelPart.gameObject.GetComponent<Rigidbody>().useGravity = false;
        modelPart.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        modelPart.gameObject.GetComponent<MeshCollider>().convex = true;
        modelPart.gameObject.GetComponent<MeshCollider>().isTrigger = true;

    }
}



  


  
}