using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
        // Hide the laser initially
        _laser.enabled = false;
    }

    private void Update()
    {
        // Find the spawned object and its parts
        originalModel = GameObject.Find("spawnedObject");
        modelParts = originalModel.GetComponentsInChildren<MeshFilter>();

        // Get the screen position of the laser button
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(GetComponent<Camera>(), _laserButton.transform.position);

        // Convert the screen position to a world position
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(_laserButton.GetComponent<RectTransform>(), screenPosition, GetComponent<Camera>(), out worldPosition))
        {
            Debug.Log("Button world position: " + worldPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Hide the laser when the button is pressed
        _laser.enabled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Show the laser when the button is released
        _laser.enabled = true;

        // Stop any previous coroutine that may be running
        if (_raycastCoroutine != null)
        {
            StopCoroutine(_raycastCoroutine);
        }

        // Start a new coroutine to update the laser position
        _raycastCoroutine = StartCoroutine(RaycastLaser());
    }

    private IEnumerator RaycastLaser()
    {
        while (true)
        {
            // Get the screen center position
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            // Get the world position of the button
            Vector3 buttonWorldPosition = _laserButton.transform.position;

            // Set the laser's start position to the button's world position
            _laser.SetPosition(1, buttonWorldPosition);

            // Raycast to find the position to set as the laser's end position
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (_raycastManager.Raycast(screenCenter, hits))
            {
                // Set the laser's end position to the hit point if the raycast hits something
                Vector3 hitPosition = hits[0].pose.position;
                _laser.SetPosition(0, hitPosition);
            }
            else
            {
                // Set the laser's end position to the center of the screen if the raycast does not hit anything
                _laser.SetPosition(0, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.nearClipPlane)));
            }

            yield return null;
        }
    }
}
