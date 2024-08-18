using Cinemachine;
using System.Collections;
using System.Collections.Generic;
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
        Transform spawner = movement.spawnpointer;
       // itemDropped = parent.gameObject; 
        GameObject tempCar = Instantiate(itemDropped, spawner.position, rb.transform.rotation);
       // tempCar.GetComponent<Car_Movement>().maxSpeed = 500;
        tempCar.GetComponent<AbilityManager>().enabled = false; 
        tempCar.GetComponentInChildren<Rigidbody>().velocity = rb.velocity;
        Destroy(tempCar, lifetime);

      
       
    }

    public override void BeginCooldown(GameObject parent)
    {
        Car_Movement movement = parent.GetComponent<Car_Movement>();

        movement.maxSpeed = temp;
    }
}
