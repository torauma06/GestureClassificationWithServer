
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;

public class RequestInferenceUpdateAndGet : MonoBehaviour
{

    [SerializeField]
    public GameObject targetObject;
    public TMP_Text handText;
    public float interval = 0.1f;
    int index = -1;

    public string filePath = "continuous_gesture_history.csv";
    static int history_record_length = 24;

    //손프리팹 전체 좌표
    HandPoint[] handpoints;
    int[] handpointlist;
    // velocity 빠진 손 좌표
    HandPoint[] hand;
    
    public Deque<int> history_deque = new Deque<int>(history_record_length);
    int history;

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
                handText.text = responseText;

                if(responseText != null) {
                    switch(responseText)
                    {
                        case "zero":
                            history = 0;
                            break;
                        case "one":
                            history = 1;
                            break;
                        case "two":
                            history = 2;
                            break;
                        case "three":
                            history = 3;
                            break;
                        case "four":
                            history = 4;
                            break;
                        case "five":
                            history = 5;
                            break;
                        case "good":
                            history = 6;
                            break;
                        case "okay":
                            history = 7;
                            break;
                        default:
                            break;
                    }

                    if (history_deque.Count() != history_record_length)
                    {
                        history_deque.AddBack(history);
                        history_deque.PrintAllElements();
                    }
                    else if (history_deque.Count() == history_record_length)
                    {
                        history_deque.RemoveFront();
                        history_deque.AddBack(history);
                        
                        history_deque.PrintAllElements();
                    }
                }
            }
        }
    }



    IEnumerator RequestUpdate()
    {
        while (true)
        {

            GetCoordinate();

            index = -1;

            StartCoroutine(PostRequest("http://127.0.0.1:5000/"));

            yield return new WaitForSeconds(interval);
        }
    }

    void SavePositionsToCSV(string num)
    {
        // CSV 파일에 위치 데이터 추가
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            StringBuilder line = new StringBuilder();
            line.Append($"{num}, ");
            history_deque.PrintAllElements();
            line.Append(history_deque.GetAllItem());
            
            writer.WriteLine(line.ToString());
        }
        Debug.Log($"Positions saved to {filePath}");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha0) || Input.GetKeyUp(KeyCode.Keypad0))
        {
            SavePositionsToCSV("0");
        }
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
        {
            SavePositionsToCSV("1");
        }
        if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
        {
            SavePositionsToCSV("2");
        }
        if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
        {
            SavePositionsToCSV("3");
        }
        if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4))
        {
            SavePositionsToCSV("4");
        }
        if (Input.GetKeyUp(KeyCode.Alpha5) || Input.GetKeyUp(KeyCode.Keypad5))
        {
            SavePositionsToCSV("5");
        }
        if (Input.GetKeyUp(KeyCode.Alpha6) || Input.GetKeyUp(KeyCode.Keypad6))
        {
            SavePositionsToCSV("6");
        }
        if (Input.GetKeyUp(KeyCode.Alpha7) || Input.GetKeyUp(KeyCode.Keypad7))
        {
            SavePositionsToCSV("7");
        }
        if (Input.GetKeyUp(KeyCode.Alpha8) || Input.GetKeyUp(KeyCode.Keypad8))
        {
            SavePositionsToCSV("8");
        }
        if (Input.GetKeyUp(KeyCode.Alpha9) || Input.GetKeyUp(KeyCode.Keypad9))
        {
            SavePositionsToCSV("9");
        }
    }
}

public class Deque<T>
{
    private LinkedList<T> linkedList = new LinkedList<T>();
    private int maxListSize;
    private string allitem;

    public Deque(int maxSize)
    {
        this.maxListSize = maxSize;
    }

    public void LimitMax()
    {

    }

    // 앞쪽에 요소 추가
    public void AddFront(T item)
    {
        if (linkedList.Count >= maxListSize)
        {
            linkedList.RemoveLast();  // 가장 오래된 항목 제거 (뒤에서 제거)
        }
        linkedList.AddFirst(item);
    }

    // 뒤쪽에 요소 추가
    public void AddBack(T item)
    {
        if (linkedList.Count >= maxListSize)
        {
            linkedList.RemoveFirst();  // 가장 오래된 항목 제거 (앞에서 제거)
        }
        linkedList.AddLast(item);
    }

    // 앞쪽에서 요소 제거
    public T RemoveFront()
    {
        if (linkedList.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        T value = linkedList.First.Value;
        linkedList.RemoveFirst();
        return value;
    }

    // 뒤쪽에서 요소 제거
    public T RemoveBack()
    {
        if (linkedList.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        T value = linkedList.Last.Value;
        linkedList.RemoveLast();
        return value;
    }

    // Deque가 비어있는지 확인
    public bool IsEmpty()
    {
        return linkedList.Count == 0;
    }

    // Deque의 요소 개수
    public int Count()
    {   
        return linkedList.Count;
    }

    // Deque의 앞쪽 요소를 확인 (제거하지 않음)
    public T PeekFront()
    {
        if (linkedList.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        return linkedList.First.Value;
    }

    // Deque의 뒤쪽 요소를 확인 (제거하지 않음)
    public T PeekBack()
    {
        if (linkedList.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        return linkedList.Last.Value;
    }

    // Deque의 모든 요소 찍어내기
    public void PrintAllElements()
    {
        if (linkedList.Count == 0)
        {
            Debug.Log("Deque is empty.");
        }

        string allItem = "";
        //Debug.Log("Deque elements:");
        int index = 1;
        foreach (var item in linkedList)
        {
            if (item.GetType() == typeof(Hand))
            {
                string jsonitem = JsonUtility.ToJson(item);
                if (linkedList.Count == index)
                    allItem = allItem + jsonitem;
                else
                    allItem = allItem + jsonitem + ",  ";
                index++;
            }
            else
            {
                if(linkedList.Count == index)
                    allItem = allItem + item.ToString();
                else
                    allItem = allItem + item.ToString() + ",  ";
                index++;
            }
        }
        Debug.Log(allItem);
        this.allitem = allItem;
    }

    public string GetAllItem()
    {
        return allitem;
    }

    public LinkedList<T> values
    { 
        get { return linkedList; } 
    }
}