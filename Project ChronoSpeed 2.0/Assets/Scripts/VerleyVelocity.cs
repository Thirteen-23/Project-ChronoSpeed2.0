using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerleyVelocity : MonoBehaviour
{
    public Vector3 Velocity;

    private Vector3 previousPos = new Vector3(0,0,0);

    // Update is called once per frame
    void FixedUpdate()
    {
        Velocity = transform.position - previousPos;
        previousPos = transform.position;
    }
}
