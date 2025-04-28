
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class RequestInference: MonoBehaviour
{

    public GameObject targetObject;
    int index = -1;

    //손프리팹 전체 좌표
    HandPoint[] handpoints;
    int[] handpointlist;
    // velocity 빠진 손 좌표
    HandPoint[] hand;

    void Start()
    {
        
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

    void GetCoordinate()
    {
        handpoints = new HandPoint[52];
        Vector3 standardPosition = targetObject.transform.position;

        GetChildCoordinate(targetObject.transform, standardPosition, handpoints);
        SettingCoordinate();

        hand = new HandPoint[handpointlist.Length];
        
        for (int i = 0; i < handpointlist.Length; i++)
        {
            hand[i] = handpoints[handpointlist[i]];
            //Debug.Log("get =  " + i +"  " + hand[i].name + "  "+ hand[i].coordinate);
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

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.J))
        {
            GetCoordinate();

            
            index = -1;
            /*
            for (int i = 0; i < hand.Length; i++)
            {
                Debug.Log("update =  " + i + "  " + hand[i].name + "  " + hand[i].coordinate);
            }
            */

            StartCoroutine(PostRequest("http://127.0.0.1:5000/"));

            //StartCoroutine(GetTimer());
        }
    }

    IEnumerator GetTimer()
    {
        yield return new WaitForSeconds(0.001f);

        StartCoroutine(GetRequest("http://127.0.0.1:5000/"));
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
        /*
        for (int i = 0; i < hand.Length; i++)
        {
            Debug.Log("post =  " + i + "  " + hand[i].name + "  " + hand[i].coordinate);
        }
        */

        HandList handList = new HandList(hand);
        string jsonhand = JsonUtility.ToJson(handList);
        //Debug.Log(jsonhand);

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
                Debug.Log(responseText);
            }
        }
    }
}
