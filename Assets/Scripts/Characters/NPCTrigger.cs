using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    public Customer NPC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 dist = Player.instance.transform.position - transform.position;
        if (dist.magnitude < transform.lossyScale.x * 0.5)
        {
            NPC.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    } 
}
