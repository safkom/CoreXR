using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Download : MonoBehaviour
{
    public string url;
    public string fileName;


    public void Awake()
    {
        
    }
    public void Start()
    {
        url = PlayerPrefs.GetString("url");
        StartCoroutine(DownloadFile());
    }

    private IEnumerator DownloadFile()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading file: {webRequest.error}");
            }
            else
            {
                byte[] bytes = webRequest.downloadHandler.data;
                string filePath = Path.Combine(Application.dataPath, "Assets", fileName);
                File.WriteAllBytes(filePath, bytes);

                Debug.Log($"File downloaded and saved to: {filePath}");
            }
        }
    }
}
