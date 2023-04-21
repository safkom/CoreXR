using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public GameObject laserPrefab;
    public LayerMask laserLayerMask;
    
    private PlaceIt placeItScript;
    private GameObject currentLaser;

    void Start()
    {
        placeItScript = FindObjectOfType<PlaceIt>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentLaser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Destroy(currentLaser);
            
            // Calculate the direction from the button to the center of the screen
            Vector3 direction = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)) - transform.position;
            
            // Cast a ray in the direction of the laser
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, laserLayerMask))
            {
                // Move the spawned object to the position of the hit point
                placeItScript.spawnedObject.transform.position = hit.point;
            }
        }
    }
}
