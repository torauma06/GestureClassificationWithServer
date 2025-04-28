using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public class GetHandCoordinate : MonoBehaviour
{
    public GameObject targetObject;
    public string filePath = "object_positions.csv";
    // Start is called before the first frame update
    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object not assigned.");
            return;
        }
        Vector3 position = targetObject.transform.position;
        // 위치 출력
        //Debug.Log($"Position of {targetObject.name}: {position}");
        //PrintObjectPositions(targetObject.transform);
        // CSV 파일에 헤더 쓰기
        //using (StreamWriter writer = new StreamWriter(filePath)){}
    }
    void SavePositionsToCSV(string num)
    {
        // CSV 파일에 위치 데이터 추가
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            StringBuilder line = new StringBuilder();
            line.Append($"{num}");
            Vector3 standardPosition = targetObject.transform.position;
            SaveObjectPositions(targetObject.transform, line, "", standardPosition);
            writer.WriteLine(line.ToString());
        }
        Debug.Log($"Positions saved to {filePath}");
    }
    void SaveObjectPositions(Transform objTransform, StringBuilder line, string hierarchy, Vector3 standardPosition)
    {
        Vector3 position = objTransform.position - standardPosition;
        double posX = (double)position.x;
        double posY = (double)position.y;
        double posZ = (double)position.z;
        // 현재 오브젝트의 위치 출력
        Debug.Log($"Position of {objTransform.name}: ({posX}, {posY}, {posZ})");
        if (line.Length > 0)
        {
            line.Append(",");
        }
        line.Append($"{posX},{posY},{posZ}");
        Debug.Log(line.ToString());
        // 자식 오브젝트들 순회
        foreach (Transform child in objTransform)
        {
            // 재귀적으로 자손들의 위치 출력
            SaveObjectPositions(child, line, ",", standardPosition);
        }
    }
    // Update is called once per frame
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