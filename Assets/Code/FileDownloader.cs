using UnityEngine;
using System.Collections;
using System.IO;

public class FileDownloader : MonoBehaviour
{
    public delegate void FileDownloadedEventHandler(string filePath);
    public static event FileDownloadedEventHandler OnFileDownloaded;

    public string url;
    public string fileName;


    public void Awake()
    {
        url = PlayerPrefs.GetString("url");
        fileName = PlayerPrefs.GetString("file");
}

    IEnumerator Start()
    {
        using (WWW www = new WWW(url))
        {
            yield return www;

            if (www.error == null)
            {
                string filePath = Application.dataPath + "/" + fileName;
                File.WriteAllBytes(filePath, www.bytes);

                if (OnFileDownloaded != null)
                {
                    OnFileDownloaded(filePath);
                }
            }
            else
            {
                Debug.LogError("Download error: " + www.error);
            }
        }
    }
}
