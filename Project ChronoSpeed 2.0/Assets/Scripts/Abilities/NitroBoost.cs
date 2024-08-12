using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class NitroBoost : Ability
{
    public float boost;

    public override void Activate(GameObject parent)
    {
      AbilityManager movement = parent.GetComponent<AbilityManager>();
        Rigidbody rb = parent.GetComponentInChildren<Rigidbody>();
        Debug.Log("ability used");
        rb.AddForce(rb.transform.forward.normalized * 1000 * boost);
    }

   
}
