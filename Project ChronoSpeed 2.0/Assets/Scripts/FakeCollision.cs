using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FakeCollision : MonoBehaviour
{
    [Header("Adjustable Values")]
    [Range(1f, 200f)]
    [SerializeField] float DysCarMass = 90f;
    [Range(1f, 200f)]
    [SerializeField] float PresCarMass = 60f;
    [Range(1f, 200f)]
    [SerializeField] float UtoCarMass = 30f;

    [Range(0f, 100f)]
    [SerializeField] float bounceFactor = 0.5f;

    [Header("Assignments")]
    [SerializeField] BoxCollider trigger;
    public Class myClass;
    float myMass = 0;
    Rigidbody myRB;
    [HideInInspector] public Transform myTransform;
    
    public struct CollisionRequiredInfo
    {
        public Class theirClass;
        public float theirMass;
        public VerleyVelocity theirVel;
        public CollisionRequiredInfo(Class c, float m, VerleyVelocity rb)
        {
            theirClass = c;
            theirMass = m;
            theirVel = rb;
        }
    }

    private void Awake()
    {
        var carMove = GetComponentInParent<Car_Movement>();
        if (carMove != null)
            myClass = carMove.carClasses;
        myRB = transform.parent.GetComponentInParent<Rigidbody>();

        //Bad but whateves
        myTransform = transform.parent.parent;
        myMass = GetMass(myClass);
    }
    Dictionary<Transform, CollisionRequiredInfo> currentCollidingCars = new Dictionary<Transform, CollisionRequiredInfo>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody"))
        {
            Class theirClass = other.transform.GetComponent<FakeCollision>().myClass;
            currentCollidingCars.Add(other.transform, new CollisionRequiredInfo(theirClass, GetMass(theirClass), other.transform.GetComponent<VerleyVelocity>()));

            ApplyRelVelocity(other);
            Depenetrate(trigger, other);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger)
            return;
        
        if(other.transform.CompareTag("CarBody"))
        {
            Depenetrate(trigger, other);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger)
            return;

        if (other.transform.CompareTag("CarBody"))
            currentCollidingCars.Remove(other.transform);
    }

    

    void Depenetrate(Collider me, Collider other)
    {
        Vector3 dir;
        float dist;

        Physics.ComputePenetration(me, transform.position, transform.rotation, other, other.transform.position, other.transform.rotation, out dir, out dist);
        dir.y = 0; dir.Normalize();

        myTransform.position += dir * dist;
    }

    void ApplyRelVelocity(Collider other)
    {
        CollisionRequiredInfo dontNeedThis;
        if (!currentCollidingCars.TryGetValue(other.transform, out dontNeedThis))
            return;

        float otherMass = currentCollidingCars[other.transform].theirMass;

        Vector3 relativeVelocity = myRB.velocity - currentCollidingCars[other.transform].theirVel.Velocity;
        //TODO: GET FINN TO FIX NORMALS
        Vector3 normal = other.transform.position - transform.position;

        normal.y = 0;
        normal.Normalize();

        Debug.Log(normal);
        float dot = (1f + bounceFactor) * Vector3.Dot(relativeVelocity, normal);

        //IDK why but if truck is 90, light is 30, total is 120, 
        //90 / 120 is 0.7, but 30 / 120 = 0.3. But i want the truck to have reduced velocity not light car so i use othermass
        dot *= otherMass /  (myMass + otherMass);

        Vector3 velChange = normal * dot;

        myRB.AddForce(-velChange, ForceMode.VelocityChange);
    }

    float GetMass(Class c)
    {
        if (c == Class.Heavy)
            return DysCarMass;
        else if (c == Class.Medium)
            return PresCarMass;
        else if (c == Class.Light)
            return UtoCarMass;

        return 0;
    }
}

