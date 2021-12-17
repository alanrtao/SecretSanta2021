using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Camera mCam;

    public OverheadCamera oCam;

    // taking a picture or not
    public bool cameraMode { get { return _cameraMode; } set {  } }
    bool _cameraMode = false;

    public delegate void MethodSlot();

    public MethodSlot PhotoShoot = () => { print("photoshoot"); };

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

    public void GameEnd()
    {
        SceneManager.LoadScene(1); // go to game end screen
    }

}


