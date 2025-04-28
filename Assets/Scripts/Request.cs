
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
public class Request : MonoBehaviour
{
    void Start()
    {
        
    }

    [System.Serializable]
    public class Hand
    {
        public string name;
        public float coordinate;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            StartCoroutine(GetRequest("http://127.0.0.1:5000/"));
        }

        if (Input.GetKeyUp(KeyCode.J))
        {
            StartCoroutine(PostRequest("http://127.0.0.1:5000/"));
        }
    }

    IEnumerator GetRequest(string url)
    {
        

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if(webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error : " + webRequest.error);
            } else {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }


    IEnumerator PostRequest(string url)
    {
        Hand data = new Hand();
        data.name = "middle";
        data.coordinate = 1f;

        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.SetRequestHeader("Content-Type", "application/json");


            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error : " + webRequest.error);
            }
            else
            {
                Debug.Log("Data uploaded");
            }
        }
    }

}
