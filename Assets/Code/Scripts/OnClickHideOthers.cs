using UnityEngine;
using UnityEngine.UI;

public class OnClickHideOthers : MonoBehaviour
{
    // The parent game object whose children will be toggled
    private GameObject parentObject;

    // The UI button that will trigger the toggle action

    private MeshRenderer[] modelParts;
    private GameObject clickedObject;

    private float lastTapTime = 0f;
    private const float tapDelay = 0.5f; // The maximum time between taps in seconds
    private int tapCount = 0;

    public void SetHideObject(){

        parentObject = GameObject.Find("spawnedObject");
        modelParts = parentObject.GetComponentsInChildren<MeshRenderer>();

    }
    private void Update()
    {
        //if empty then return
        if (parentObject == null)
            return;

        

           // Check if the user double tapped on a child object
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        tapCount++;

        // Check if the user tapped twice in quick succession
        if (tapCount == 2 && (Time.time - lastTapTime) < tapDelay)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.IsChildOf(parentObject.transform))
                {
                    clickedObject = hit.transform.gameObject;
                    Vector3 center = clickedObject.GetComponent<Renderer>().bounds.center;
                    transform.LookAt(center);
                    //set rotation center inside

               

                    tapCount = 0; // Reset tap count
                    ShowOnlyClickedObject();

                }
            }
        }

        // Reset tap count and timer if too much time has passed between taps
        if ((Time.time - lastTapTime) > tapDelay)
        {
            tapCount = 1;
        }

        lastTapTime = Time.time;
    }
    }

    private void ShowOnlyClickedObject()
    {
        foreach (var modelPart in modelParts)
        {
            if (modelPart.gameObject == clickedObject)
            {
                modelPart.gameObject.SetActive(true);
            }
            else
            {
                modelPart.gameObject.SetActive(false);
            }
        }
    }

     public void UnhideChildren(){
        //modelParts exist
        if (modelParts != null)            
        foreach (var modelPart in modelParts)
                modelPart.gameObject.SetActive(true);

    }
    

  
}
