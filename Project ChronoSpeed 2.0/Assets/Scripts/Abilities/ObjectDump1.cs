using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ObjectDump1: Ability
{
    public GameObject itemDropped;
    public float lifetime;
    public float limitRemoveSpeedNow;
    [SerializeField] float temp;
    public override void Activate(GameObject parent)
    {
        Rigidbody rb = parent.GetComponentInChildren<Rigidbody>();
      
        Car_Movement movement = parent.GetComponent<Car_Movement>();
       // movement.maxSpeed = limitRemoveSpeedNow;
      

      
       
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();

        movement.maxSpeed = temp;
    }
}
