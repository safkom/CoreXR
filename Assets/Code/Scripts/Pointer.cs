using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    [SerializeField] private Button _laserButton;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private Transform _raycastOrigin;
    private GameObject originalModel;
    private MeshFilter[] modelParts;
    Vector3 worldPosition;

    private Coroutine _raycastCoroutine;

    private void Start()
    {
        //_laser hidden
        _laser.enabled = false;
    
    }
    private void Update(){
            originalModel = GameObject.Find("spawnedObject");
            modelParts = originalModel.GetComponentsInChildren < MeshFilter > ();

            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(GetComponent<Camera>(), _laserButton.transform.position);

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_laserButton.GetComponent<RectTransform>(), screenPosition, GetComponent<Camera>(), out worldPosition))
            {
                Debug.Log("Button world position: " + worldPosition);
            }

    }
    public void OnPointerDown(BaseEventData eventData)
    {
        _laser.enabled = false;
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        _laser.enabled = true;
        if (_raycastCoroutine != null)
        {
            StopCoroutine(_raycastCoroutine);
        }
        _raycastCoroutine = StartCoroutine(RaycastLaser());
    }

    private IEnumerator RaycastLaser()
    {
        while (true)
        {
            // Cast a ray from the center of the screen
            Vector3 screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));


         

            //user camera
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (_raycastManager.Raycast(screenCenter, hits))
            {
                //if raycast hits the object
               

                // Get the position of the hit point and set the laser's end position
                Vector3 hitPosition = hits[0].pose.position;


                
                _laser.SetPosition(1, worldPosition);
                _laser.SetPosition(0, hitPosition);
            }
            else
            {


                _laser.SetPosition(1, worldPosition);
                _laser.SetPosition(0, Vector3.forward * 1000f);

            }

            yield return null;
        }
    }
}
