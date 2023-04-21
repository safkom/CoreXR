using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Reset : MonoBehaviour
{
 
    // Update is called once per frame
    public void onResetScene_click()
    {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
