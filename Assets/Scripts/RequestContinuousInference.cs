using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class RequestContinuousInference : MonoBehaviour
{

    public TMP_Text handText;
    public float interval = 0.1f;

    Deque<int> deque;
    int[] array;

    void Start()
    {
        deque = GameObject.Find("Inferencer").GetComponent<RequestInferenceUpdateAndGet>().history_deque;
        
        StartCoroutine(RequestUpdate());
    }

    void Update()
    {
        
    }

    IEnumerator RequestUpdate()
    {
        while (true)
        {
            StartCoroutine(PostRequest("http://127.0.0.1:8000/"));

            yield return new WaitForSeconds(interval);
        }
    }
    
    IEnumerator PostRequest(string url)
    {
        array = deque.values.ToArray();
        string json = JsonConvert.SerializeObject(array);

        //Debug.Log(json);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error : " + webRequest.error);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                //Debug.Log("¾ÆÁ÷ ¸Á X"); //responseText
                handText.text = responseText;
            }
        }
    }
}
