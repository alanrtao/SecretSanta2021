using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class Manager : MonoBehaviour
{
    public static Manager Instance
    {
        get { return _instance; }
    }
    private static Manager _instance;

    public Globe Globe;

    public Sun Sun;

    public GameObject TreePrototype;

    public Camera mCam;

    public bool cameraMode = false; // taking a picture or not

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



}


