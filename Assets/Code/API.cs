 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class API : MonoBehaviour
{
    public class Object
    {
        public int id { get; set; }
        public string asset_name { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string asset_image { get; set; }
        public int user_id { get; set; }
        public string category_name { get; set; }
    }
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public TextMeshProUGUI assetText;
    public GameObject predmet;
    public GameObject gumbi;
    public TextMeshProUGUI ime;
    public TextMeshProUGUI opis;
    public string TextureURL = "";
    public Image slika;

    void Start()
    {
        StartCoroutine(GetRequest("https://corexr.si/api/api/"));
        predmet.SetActive(false);
    }

    public void OnRefresh()
    {
        Start();
    }
    public void Nazaj()
    {
        SceneManager.LoadScene("Scroll V2");
    }
    public void Go()
    {
        SceneManager.LoadScene("AR");
    }

    public void NewPage(string asset_name, string description, string asset_image, string url)
    {
        Debug.Log(asset_name);
        Debug.Log(description);
        predmet.SetActive(true);
        gumbi.SetActive(false);
        ime.text = asset_name;
        opis.text = description;
        TextureURL = "https://corexr.si/api/uploads/" + asset_image;
        PlayerPrefs.SetString("url", "corexr.si/api/uploads/" + url);
        StartCoroutine(DownloadImage(TextureURL));
    }

    public void ShowDescription(string description)
    {
        Debug.Log(description);
    }



    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            Texture2D webTexture = ((DownloadHandlerTexture)request.downloadHandler).texture as Texture2D;
            Sprite webSprite = SpriteFromTexture2D(webTexture);
            slika.sprite = webSprite;
        }
    }

    private IEnumerator DownloadImages(string url, Action<Sprite> onComplete)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // Create a new sprite from the downloaded texture
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // Call the onComplete callback with the sprite
                onComplete(sprite);
            }
        }
    }

    Sprite SpriteFromTexture2D(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
    }


    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(string.Format("Neki ne dela:{0}", webRequest.error));
                    break;
                case UnityWebRequest.Result.Success:
                    Object[] objects = JsonConvert.DeserializeObject<Object[]>(webRequest.downloadHandler.text);
                    foreach (Object obj in objects)
                    {
                        // Create a new button for each object
                        GameObject image = Instantiate(buttonPrefab, buttonContainer);

                        string slika = "https://corexr.si/api/uploads/" + obj.asset_image;

                        StartCoroutine(DownloadImages(slika, sprite =>
                        {
                            image.GetComponent<Image>().sprite = sprite;
                        }));


                        // Add a listener to the button that shows the description when clicked
                        image.GetComponent<Button>().onClick.AddListener(() => NewPage(obj.asset_name, obj.description, obj.asset_image, obj.url));
                    }
                    break;
            }
        }
    }

}
