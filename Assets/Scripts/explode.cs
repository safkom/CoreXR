using UnityEngine;

public class explode: MonoBehaviour {

  private GameObject originalModel;
  
  public float explodeDistance = 20f;

  private MeshFilter[] modelParts;
  private Vector3[] originalPositions;
  private bool exploded = false;
  private int clickCount = 0;

  void Update() {


    originalModel = GameObject.Find("spawnedObject");
    //if empty then return
    if (originalModel == null)
        return;

    
    modelParts = originalModel.GetComponentsInChildren < MeshFilter > ();

    if(clickCount%2==0){
        originalPositions = new Vector3[modelParts.Length];
        for (int i = 0; i < modelParts.Length; i++)
            originalPositions[i] = modelParts[i].transform.localPosition;
    
    }
  }

    public void onExplodeClick()
    {
        
       if(exploded == false){
           Explode();
           exploded = true;
       }
       else{
           Reset();
           exploded = false;
       }
       clickCount++;
    }


  private void Explode() {
    Vector3 center = originalModel.transform.position;

    for (int i = 0; i < modelParts.Length; i++) {

      Vector3 direction = originalPositions[i].normalized;

      modelParts[i].transform.localPosition *= explodeDistance;

    }
  }

  private void Reset() {
    for (int i = 0; i < modelParts.Length; i++) {
      modelParts[i].transform.localPosition = originalPositions[i];
    }
  }
}