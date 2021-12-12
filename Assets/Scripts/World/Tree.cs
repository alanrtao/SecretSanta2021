using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public MeshSet all_trees;

    public Material gold, normal;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.sharedMesh = all_trees.GetRandom();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool accomplished
    {
        get { return _acc; }
        set
        {
            _acc = value;

            GetComponent<MeshRenderer>().material = _acc ? gold : normal;
        }
    }

    private bool _acc;

}
