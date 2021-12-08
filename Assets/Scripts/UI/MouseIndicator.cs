using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void Emit()
    {

        ps = GetComponent<ParticleSystem>();
        transform.position = Vehicle.Instance.Board.MouseXYZ;
        ps.Play();
    }

    void Update()
    {
    }
}
