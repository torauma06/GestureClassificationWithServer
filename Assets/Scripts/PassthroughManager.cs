using UnityEngine;

public class PassthroughManager : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    public OVRInput.Button button;
    public OVRInput.Controller controller;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(button, controller))
        {
            passthroughLayer.hidden = !passthroughLayer.hidden;
        }
    }
}
