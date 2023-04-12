using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Reset : MonoBehaviour
{
 
    // Update is called once per frame
    public void onResetScene_click()
    {
        PlaceIt placeitF = FindObjectOfType<PlaceIt>();
        placeitF.ResetScene();
    
    }
}
