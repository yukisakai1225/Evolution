using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTorque : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 Axis;
    public float Magnitude;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.maxAngularVelocity = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {
        Force();
    }

    void Force()
    {
        Axis = Vector3.Normalize(Axis);
        rb.AddTorque(Axis * Magnitude, ForceMode.Force);
    }
}
