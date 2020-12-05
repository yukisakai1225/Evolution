using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ListTorque : MonoBehaviour
{
    private float timeElapsed = 0.0f; //経過時間を計測
    public float timeOut = 0.1f; //0.1秒毎
    private int n;
    public int Length = 100;
    public List<float> list = new List<float>();
    private Rigidbody rb;
    public Vector3 Axis;
    public float Magnitude;
    private int MinTorque = -60;
    private int MaxTorque = 100;


    // Start is called before the first frame update
    void Start()
    {
        n = 0;
        rb = this.GetComponent<Rigidbody>();
        for(var j = 0; j< Length; j++)
        {
            list.Add(Random.Range(MinTorque, MaxTorque));
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= timeOut)
        {
            Force();
            timeElapsed = 0.0f;
            n++;
        }
    }

    void Force()
    {
        Magnitude = list[n];
        //Axis = Vector3.Normalize(Axis);
        rb.AddTorque(Axis * Magnitude, ForceMode.Force);
    }
}
