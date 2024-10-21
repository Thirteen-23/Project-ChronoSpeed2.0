using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ObjectSpawn : Ability
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
       // itemDropped = parent.gameObject; 

      

      
       
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();

        movement.maxSpeed = temp;
    }
}
