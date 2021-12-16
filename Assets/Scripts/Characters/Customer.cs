using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MassedMonoBehaviour
{
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public struct Quest
    {
        public Texture outfit;
        public string name;
        public string text;
        public System.Func<bool> criteria;
    }
}
