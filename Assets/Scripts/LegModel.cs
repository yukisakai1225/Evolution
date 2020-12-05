using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LegModel
{
    public List<float> DNA { get; set; }
    System.Random random = new System.Random();
    public float Result { get; set; }
    public Rigidbody SockerBallRigidBody;
    public float Evaluation;
    public MoveTorque MoveTorque;
    public TorqueController TorqueController;

    public LegModel ()
    {
        DNA = new List<float>();
        Result = 0;
    }

    public String toString() {
        String str = "res: " + Result + ", [" + string.Join(", ", DNA) + "]";
        return str;
    }

    public LegModelMonoBehaviour LegModelMonoBehaviour;
}
