using UnityEngine;
using Unity.Sentis;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using System;
using Unity.Collections;

public class Inference : MonoBehaviour
{
    [SerializeField]
    ModelAsset modelAsset;
    public GameObject targetObject;


    //IWorker worker;
    float[] coordinate;
    //TensorFloat outputTensor;

    List<float> coordinateNow;
    float[] hand;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Inference Class Loaded");

        var model = ModelLoader.Load(modelAsset);
        Debug.Log("Model successfully Loaded");

        //worker = WorkerFactory.CreateWorker(BackendType.CPU, model);
    }

    void GetCoordinate()
    {
        coordinateNow = new List<float>();
        Vector3 standardPosition = targetObject.transform.position;
        
        GetChildCoordinate(targetObject.transform, standardPosition);

        SettingCoordinate();
        //Debug.Log("hand   " + hand.Length);
        
        //InferenceCoordinate();
    }

    void GetChildCoordinate(Transform objTransform, Vector3 standardPosition)
    {
        Vector3 position = objTransform.position - standardPosition;
        float posX = (float)position.x;
        float posY = (float)position.y;
        float posZ = (float)position.z;

        coordinateNow.Add(posX);
        coordinateNow.Add(posY);
        coordinateNow.Add(posZ);

        foreach (Transform child in objTransform)
        {
            // 재귀적으로 자손들의 위치 출력
            GetChildCoordinate(child, standardPosition);
        }
    }

    void SettingCoordinate()
    {
        float[] coordinateArray = coordinateNow.ToArray();
        
        // 3
        float[] wrist = coordinateArray[..3];
        // 15
        float[] index = coordinateArray[4..19];
        //15
        float[] little = coordinateArray[34..49];
        // 15
        float[] middle = coordinateArray[64..79];
        //3
        float[] palm = coordinateArray[94..97];
        //15
        float[] ring = coordinateArray[100..115];
        // 12
        float[] thumb = coordinateArray[130..142];

        List<float> handlist = new List<float>();
        handlist.AddRange(wrist);
        handlist.AddRange(index);
        handlist.AddRange(little);
        handlist.AddRange(middle);
        handlist.AddRange(palm);
        handlist.AddRange(ring);
        handlist.AddRange(thumb);

        hand = handlist.ToArray();
    }

    /*
    void InferenceCoordinate()
    {
        TensorShape shape = new TensorShape(1, 78);
        TensorFloat inputTensor = new TensorFloat(shape, hand);

        worker.Execute(inputTensor);
        outputTensor = worker.PeekOutput() as TensorFloat;
        float[] outputData = outputTensor.ToReadOnlyArray();

        
        float maxvalue = Mathf.Max(outputData);
        int maxIndex = Array.IndexOf(outputData, maxvalue);
        Debug.Log(maxIndex);
        
        //worker.Dispose();
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            GetCoordinate();
        }
    }

    /*
    void OnDestroy()
    {
        // Worker 해제
        worker.Dispose();
    }

    void OnDisable()
    {
        worker.Dispose();
    }
    */
}
