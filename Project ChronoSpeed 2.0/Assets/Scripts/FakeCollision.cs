using System.Collections.Generic;
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
    public Transform myTransform;

    //if true, cant do collisions
    public bool Intangible = false;
    
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
        myRB = transform.parent.GetComponent<Rigidbody>();

        //Bad but whateves
        myTransform = transform.parent;
        myMass = GetMass(myClass);
    }
    Dictionary<Transform, CollisionRequiredInfo> currentCollidingCars = new Dictionary<Transform, CollisionRequiredInfo>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger || myRB == null)
            return;

        if (other.transform.CompareTag("CarBody"))
        {
            Class theirClass = other.transform.GetComponent<FakeCollision>().myClass;
            currentCollidingCars.Add(other.transform, new CollisionRequiredInfo(theirClass, GetMass(theirClass), other.transform.GetComponent<VerleyVelocity>()));

            Vector3 direction;
            float distance;

            if(Physics.ComputePenetration(trigger, transform.position, transform.rotation, other, other.transform.position, other.transform.rotation, out direction, out distance))
            {
                direction.y = 0; direction.Normalize();
                Depenetrate(direction, distance);

                CollisionRequiredInfo otherCarInfo;
                currentCollidingCars.TryGetValue(other.transform, out otherCarInfo);
                ApplyRelVelocity(otherCarInfo, direction);
            }
            
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger || myRB == null)
            return;
        
        if(other.transform.CompareTag("CarBody"))
        {
            Vector3 dir;
            float dist;

            if(Physics.ComputePenetration(trigger, transform.position, transform.rotation, other, other.transform.position, other.transform.rotation, out dir, out dist));
            {
                Depenetrate(dir, dist);
            }

        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger || myRB == null)
            return;

        if (other.transform.CompareTag("CarBody"))
            currentCollidingCars.Remove(other.transform);
    }

    

    void Depenetrate(Vector3 direction, float distance)
    {
        myTransform.position += direction * distance;
    }

    void ApplyRelVelocity(CollisionRequiredInfo CRI, Vector3 collisionNorm)
    {
        float otherMass = CRI.theirMass;
        Vector3 relativeVelocity = myRB.velocity - CRI.theirVel.Velocity;

        //collisionNorm.y = 0; collisionNorm.Normalize();
        float dot = (1f + bounceFactor) * Vector3.Dot(relativeVelocity, collisionNorm);

        //dot *= otherMass /  (myMass + otherMass);

        Vector3 velChange = collisionNorm * dot;

        velChange = Vector3.ClampMagnitude(velChange, 5);
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

