using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mesh Set", menuName = "ScriptableObjects/Mesh Set")]
public class MeshSet : ScriptableObject
{
    public Mesh[] content;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Mesh GetRandom()
    {
        return content[Mathf.FloorToInt(Random.value * content.Length)];
    }
}
