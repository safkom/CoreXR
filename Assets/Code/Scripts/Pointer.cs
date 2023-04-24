using UnityEngine;

public class Pointer : MonoBehaviour
{
    public float maxDistance = 10f;
    public LayerMask layerMask;
    public GameObject laserPrefab;
    public Transform laserSpawnPoint;
    public KeyCode laserButton = KeyCode.Mouse0;

    private GameObject currentLaser;
    private Transform pointerTransform;

    private void Start()
    {
        pointerTransform = transform.GetChild(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(laserButton))
        {
            currentLaser = Instantiate(laserPrefab, laserSpawnPoint.position, laserSpawnPoint.rotation);
        }
        else if (Input.GetKey(laserButton))
        {
            if (currentLaser != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(pointerTransform.position, pointerTransform.forward, out hit, maxDistance, layerMask))
                {
                    currentLaser.transform.LookAt(hit.point);
                }
                else
                {
                    currentLaser.transform.rotation = pointerTransform.rotation;
                    currentLaser.transform.position = laserSpawnPoint.position + pointerTransform.forward * maxDistance;
                }
            }
        }
        else if (Input.GetKeyUp(laserButton))
        {
            if (currentLaser != null)
            {
                Destroy(currentLaser);
            }
        }
    }
}
