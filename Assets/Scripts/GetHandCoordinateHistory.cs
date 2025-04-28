using Oculus.Interaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity.Hierarchy;
using UnityEngine;

/*
public class Deque<T>
{
    private LinkedList<T> deque = new LinkedList<T>();
    private int maxSize;

    public Deque(int maxSize)
    {
        this.maxSize = maxSize;
    }

    public void LimitMax()
    {
        
    }

    // ���ʿ� ��� �߰�
    public void AddFront(T item)
    {
        if (deque.Count >= maxSize)
        {
            deque.RemoveLast();  // ���� ������ �׸� ���� (�ڿ��� ����)
        }
        deque.AddFirst(item);
    }

    // ���ʿ� ��� �߰�
    public void AddBack(T item)
    {
        if (deque.Count >= maxSize)
        {
            deque.RemoveFirst();  // ���� ������ �׸� ���� (�տ��� ����)
        }
        deque.AddLast(item);
    }

    // ���ʿ��� ��� ����
    public T RemoveFront()
    {
        if (deque.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        T value = deque.First.Value;
        deque.RemoveFirst();
        return value;
    }

    // ���ʿ��� ��� ����
    public T RemoveBack()
    {
        if (deque.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        T value = deque.Last.Value;
        deque.RemoveLast();
        return value;
    }

    // Deque�� ����ִ��� Ȯ��
    public bool IsEmpty()
    {
        return deque.Count == 0;
    }

    // Deque�� ��� ����
    public int Count()
    {
        return deque.Count;
    }

    // Deque�� ���� ��Ҹ� Ȯ�� (�������� ����)
    public T PeekFront()
    {
        if (deque.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        return deque.First.Value;
    }

    // Deque�� ���� ��Ҹ� Ȯ�� (�������� ����)
    public T PeekBack()
    {
        if (deque.Count == 0)
            throw new InvalidOperationException("Deque is empty.");

        return deque.Last.Value;
    }

    // Deque�� ��� ��� ����
    public void PrintAllElements()
    {
        if (deque.Count == 0)
        {
            Debug.Log("Deque is empty.");
        }

        string allItem = "";
        Debug.Log("Deque elements:");
        foreach (var item in deque)
        {
            if (item.GetType() == typeof(Hand))
            {
                string jsonitem = JsonUtility.ToJson(item);
                allItem = allItem + jsonitem + "  ";
            }
            else
            {
                allItem = allItem + item.ToString() + "  ";
            }
        }
        Debug.Log(allItem);
    }
}
*/
class Hand
{
    public Vector3[] handPoints;

    public Hand(Vector3[] handPoints)
    {
        this.handPoints = handPoints;
    }
}

public class GetHandCoordinateHistory : MonoBehaviour
{
    public GameObject targetObject;
    public string filePath = "object_positions_history.csv";

    public int history_record_length = 16;
    Deque<Hand> deque = new Deque<Hand>(16);

    int ji = 0;

    Vector3[] handpointsVec;
    // handpoints without velocity
    int[] handpointsWV;
    Vector3[] handpointsVecWV;

    int index = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingCoordinate();

        if (targetObject == null)
        {
            Debug.LogError("Target object not assigned.");
            return;
        }
        //Vector3 position = targetObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        index = -1;
        ProcessHand();

        Debug.Log("�ѹ� ���� ");
        int j = 0;

        foreach (var h in handpointsVecWV)
        {
            j++;
            Debug.Log(j + "  " + h);
        }

        Hand handhistory = new Hand(handpointsVecWV);
        string jsonhand = JsonUtility.ToJson(handhistory);
        Debug.Log("jsonhand" + jsonhand);

        if (deque.Count() != history_record_length)
        {
            deque.AddBack(handhistory);
            deque.PrintAllElements();
        } 
        else if (deque.Count() == history_record_length)
        {
            deque.RemoveFront();
            deque.AddBack(handhistory);
            deque.PrintAllElements();
        }

    }

    void ProcessHand()
    {
        handpointsVec = new Vector3[52];
        Vector3 standardPosition = targetObject.transform.position;

        GetChildVec(targetObject.transform, standardPosition, handpointsVec);

        handpointsVecWV = new Vector3[handpointsWV.Length];
        for (int i = 0; i < handpointsWV.Length; i++)
        {
            handpointsVecWV[i] = handpointsVec[handpointsWV[i]];
        }
    }

    void GetChildVec(Transform targetTrans, Vector3 standardPosition, Vector3[] handpoints)
    {
        index++;
        Vector3 position = targetTrans.position - standardPosition;
        handpoints[index] = position;
        foreach (Transform child in targetTrans)
        {
            // ��������� �ڼյ��� ��ġ ���
            GetChildVec(child, standardPosition, handpoints);
        }
    }

    void GetHandVector(Transform objTransform, StringBuilder line, string hierarchy, Vector3 standardPosition)
    {
        Vector3 position = objTransform.position - standardPosition;
        double posX = (double)position.x;
        double posY = (double)position.y;
        double posZ = (double)position.z;
        // ���� ������Ʈ�� ��ġ ���
        Debug.Log($"Position of {objTransform.name}: ({posX}, {posY}, {posZ})");
        if (line.Length > 0)
        {
            line.Append(",");
        }
        line.Append($"{posX},{posY},{posZ}");
        Debug.Log(line.ToString());
        // �ڽ� ������Ʈ�� ��ȸ
        foreach (Transform child in objTransform)
        {
            // ��������� �ڼյ��� ��ġ ���
            GetHandVector(child, line, ",", standardPosition);
        }
    }

    // �� ��ǥ ����
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

        handpointsWV = handlist.ToArray();
    }
}
