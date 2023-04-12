using UnityEngine;
using UnityEngine.UI;

public class CreateLabels : MonoBehaviour
{
    private GameObject labelPrefab;
    private MeshFilter[] modelParts;

    public float yOffset = 10f;

    public void Create()
    {
        GameObject originalModel = GameObject.Find("spawnedObject");
        modelParts = originalModel.GetComponentsInChildren<MeshFilter>();

        for (int i = 0; i < modelParts.Length; i++) 
        {

                //Vector3 direction = originalPositions[i].normalized;

                modelParts[i].transform.localPosition *= 2;


        }
    }
}
