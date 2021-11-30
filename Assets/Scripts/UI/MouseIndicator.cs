using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void Emit()
    {
        transform.position = Vehicle.Instance.Board.MouseXYZ;
        // ps.Play();
    }

    void Update()
    {
    }
}
