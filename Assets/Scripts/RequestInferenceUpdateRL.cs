
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class RequestInferenceUpdateRL: MonoBehaviour
{

    [SerializeField]
    public GameObject targetObjectLeft;
    public GameObject targetObjectRight;
    public TMP_Text lefthandText;
    public TMP_Text righthandText;

    public float interval = 0.1f;
    int index = -1;

    //손프리팹 전체 좌표
    HandPoint[] handpoints;
    int[] handpointlist;
    // velocity 빠진 손 좌표
    HandPoint[] hand;

    void Start()
    {
        StartCoroutine(RequestUpdate());
    }

    [System.Serializable]
    class HandPoint
    {
        public string name;
        public Vector3 coordinate;

        public HandPoint(string name, Vector3 coordinate)
        {
            this.name = name;
            this.coordinate = coordinate;
        }
    }

    class HandList
    {
        public HandPoint[] handpoints;

        public HandList(HandPoint[] handpoints)
        {
            this.handpoints = handpoints;
        }
    }

    void GetCoordinate(GameObject targetObject)
    {
        handpoints = new HandPoint[52];
        Vector3 standardPosition = targetObject.transform.position;

        GetChildCoordinate(targetObject.transform, standardPosition, handpoints);
        SettingCoordinate();

        hand = new HandPoint[handpointlist.Length];
        for (int i = 0; i < handpointlist.Length; i++)
        {
            hand[i] = handpoints[handpointlist[i]];
        }
    }

    void GetChildCoordinate(Transform targetTrans, Vector3 standardPosition, HandPoint[] handpoints)
    {
        index++;
        Vector3 position = targetTrans.position - standardPosition;
        handpoints[index] = new HandPoint(targetTrans.gameObject.name, position);
        foreach (Transform child in targetTrans)
        {
            // 재귀적으로 자손들의 위치 출력
            GetChildCoordinate(child, standardPosition, handpoints);
        }
    }

    void SettingCoordinate()
    {
        // 1
        int[] wrist = { 0 };
        // 5
        int[] index = { 1, 2, 3, 4, 5 };
        // 5
        int[] little = { 11, 12, 13, 14, 15 };
        // 5
        int[] middle = { 21, 22, 23, 24, 25 };
        // 1
        int[] palm = { 31 };
        // 5
        int[] ring = { 33, 34, 35, 36, 37 };
        // 4
        int[] thumb = { 43, 44, 45, 46 };

        List<int> handlist = new List<int>();
        handlist.AddRange(wrist);
        handlist.AddRange(index);
        handlist.AddRange(little);
        handlist.AddRange(middle);
        handlist.AddRange(palm);
        handlist.AddRange(ring);
        handlist.AddRange(thumb);

        handpointlist = handlist.ToArray();            
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


    IEnumerator PostRequest(string handRL, string url)
    {

        HandList handList = new HandList(hand);
        string jsonhand = JsonUtility.ToJson(handList);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonhand);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.SetRequestHeader("Content-Type", "application/json");


            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("Error : " + webRequest.error);
            }
            else
            {
                //Debug.Log("Data uploaded");
                string responseText = webRequest.downloadHandler.text;

                if (handRL.Equals("Left"))
                {
                    Debug.Log(handRL +"   " + responseText);
                    lefthandText.text = responseText;
                }
                else
                {
                    Debug.Log(handRL + "   " + responseText);
                    righthandText.text = responseText;
                }
            }
        }
    }

    IEnumerator RequestUpdate()
    {
        while (true)
        {

            GetCoordinate(targetObjectLeft);
            index = -1;
            StartCoroutine(PostRequest("Left","http://127.0.0.1:5000/"));

            GetCoordinate(targetObjectRight);
            index = -1;
            StartCoroutine(PostRequest("Right", "http://127.0.0.1:5000/"));

            yield return new WaitForSeconds(interval);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.D)) {

            Debug.Log("D pressed");
        }
    }
}
