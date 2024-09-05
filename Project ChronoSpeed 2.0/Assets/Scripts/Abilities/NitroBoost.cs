using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CreateAssetMenu]
public class NitroBoost : Ability
{
    public float boost;
  
    public override void Activate(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        Debug.Log("ability used");
        movement.meBoosting = true;
        

    }

    
    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();
        movement.meBoosting = false;
    }
}
